# BrewUp Order Confirmation with BMAD Harness — Design

## Goal

Implement Sales Order confirmation in BrewUp using native BMAD workflows constrained by the committed BMAD harness.

A Sales Order can become `Confirmed` only after:

1. Payment has authorized the customer payment; and
2. Warehouse has reserved every requested beer.

The implementation must preserve the decision ownership and module structure defined by `BC-000` through `BC-011` and `AR-000` through `AR-018`.

## Confirmed Domain Decisions

### Payment authorized, reservation failed

If Payment authorizes the payment but Warehouse cannot reserve all requested stock:

- the Sales Order remains unconfirmed;
- the authorization remains recorded;
- the saga records a terminal reservation failure;
- no automatic void, refund, retry, timeout interpretation, or notification is performed.

### Reservation semantics

Stock reservation is all-or-nothing for the complete Sales Order. Warehouse either reserves all requested rows or reserves none.

### Payment-provider boundary

No real payment provider is introduced. Payment owns a pending `PaymentAuthorization` and accepts a simulated provider callback that records the authorized outcome. The saga does not supply or manufacture the authorization result.

### Coordination

The existing `SalesOrderSaga` coordinates the process in this order:

```text
request payment authorization
        ↓
PaymentAuthorized
        ↓
request complete stock reservation
        ↓
StockReserved
        ↓
ConfirmSalesOrder with both evidence IDs
```

The saga coordinates messages and retains evidence. It does not authorize payment, decide availability, reserve stock, or confirm the Sales Order itself.

## Architecture

### Payment module

Payment is a separate business authority and must be implemented as a complete BrewUp module:

```text
src/Payment/
├── BrewUp.Payment.SharedKernel/
├── BrewUp.Payment.Domain/
├── BrewUp.Payment.ReadModel/
├── BrewUp.Payment.Infrastructure/
├── BrewUp.Payment.Facade/
└── BrewUp.Payment.Tests/
```

All six projects are added to `src/BrewUp.slnx`. `PaymentModule` is registered explicitly in the REST host.

Payment owns:

- `PaymentAuthorizationId`;
- the `PaymentAuthorization` aggregate;
- authorization request and outcome commands;
- payment domain events;
- the callback boundary used by the simulated provider;
- the Payment read model and module wiring.

The authorization begins pending. A provider-simulator callback records the authorized outcome through Payment's facade. Only Payment can emit `PaymentAuthorized`.

### Warehouse reservation

Warehouse adds a `StockReservation` aggregate that evaluates the complete order request and emits one outcome:

- `StockReserved`, including the Warehouse-owned `StockReservationId`; or
- `StockReservationFailed`, with no reserved rows.

The aggregate never produces a partial reservation. Warehouse projections account for active reservations when reporting availability.

The current availability-to-`AcceptSalesOrder` shortcut is disconnected from the confirmation flow. Shipment timing remains unchanged because the feature does not define a new shipment policy.

### Sales confirmation

Sales adds a `ConfirmSalesOrder` command. The `SalesOrder` aggregate stores:

- `PaymentAuthorizationId?`;
- `StockReservationId?`.

`SalesOrder.Confirm()` requires both external decision references. If either is absent, confirmation is invalid and no `SalesOrderConfirmed` event is emitted. With both present, the aggregate emits `SalesOrderConfirmed` and transitions exactly once to `Confirmed`.

Sales stores evidence only. It does not load, mutate, or embed Payment or Warehouse models.

### Saga coordination

The existing saga retains the current customer-budget and order-placement steps, then:

1. creates a `PaymentAuthorizationId`;
2. sends `RequestPaymentAuthorization`;
3. waits for `PaymentAuthorized`;
4. creates a `StockReservationId`;
5. sends `ReserveStock` with all order rows;
6. waits for `StockReserved`;
7. sends `ConfirmSalesOrder` with both evidence IDs;
8. completes after `SalesOrderConfirmed`.

On `StockReservationFailed`, the saga records a terminal failure and sends no confirmation or compensation command.

## Contracts

### Payment-owned contracts

- `PaymentAuthorizationId`
- `RequestPaymentAuthorization`
- `RecordPaymentAuthorized`
- `PaymentAuthorizationRequested`
- `PaymentAuthorized`

### Warehouse-owned contracts

- `StockReservationId`
- `ReserveStock`
- `StockReserved`
- `StockReservationFailed`

### Sales-owned contracts

- `ConfirmSalesOrder`
- `SalesOrderConfirmed`

### Saga-owned contracts

Saga events and state transitions retain the authorization ID, reservation ID, and terminal reservation-failure outcome.

Module-specific IDs are strongly typed and owned by the producing module. Cross-context behavior uses explicit messages and ACL handlers; no module references another module's Domain or Infrastructure project.

## Data and Error Flow

### Successful flow

```text
SalesOrderSaga
  → RequestPaymentAuthorization
Payment
  → PaymentAuthorizationRequested
provider simulator callback
  → RecordPaymentAuthorized
Payment
  → PaymentAuthorized
SalesOrderSaga
  → ReserveStock
Warehouse
  → StockReserved
SalesOrderSaga
  → ConfirmSalesOrder(PaymentAuthorizationId, StockReservationId)
Sales
  → SalesOrderConfirmed
SalesOrderSaga
  → completed
```

### Insufficient stock

```text
PaymentAuthorized
  → ReserveStock
Warehouse
  → StockReservationFailed
SalesOrderSaga
  → terminal reservation failure
SalesOrder
  → remains unconfirmed
PaymentAuthorization
  → remains authorized
```

### Idempotency

Duplicate commands or events must not:

- create a second authorization for the same request;
- create a second stock reservation for the same Sales Order;
- reserve stock twice;
- confirm the Sales Order twice;
- complete the saga twice.

Aggregate state guards and stable external IDs enforce these behaviors.

## Testing Strategy

Behavior is implemented test-first.

### Payment tests

- requesting authorization creates a pending authorization;
- recording the simulated provider outcome emits `PaymentAuthorized`;
- duplicate outcome recording does not authorize twice;
- architecture tests enforce the standard six-project module and reference directions.

### Warehouse tests

- all rows available produces one complete reservation;
- one missing beer produces a failure and no reservation;
- one insufficient quantity produces a failure and no reservation;
- multiple successful rows are all retained;
- duplicate reservation requests do not reserve twice.

### Sales tests

- missing `PaymentAuthorizationId` prevents confirmation;
- missing `StockReservationId` prevents confirmation;
- both IDs produce `SalesOrderConfirmed`;
- duplicate confirmation does not emit a second confirmation;
- the confirmed state retains both external references.

### Saga tests

- reservation is requested only after `PaymentAuthorized`;
- confirmation is requested only after `StockReserved`;
- reservation failure never sends `ConfirmSalesOrder`;
- reservation failure does not send void, refund, retry, or release commands;
- duplicate outcomes do not repeat downstream actions.

### Verification

- run targeted Payment, Warehouse, Sales, and Saga test projects;
- run architecture tests;
- build `src/BrewUp.slnx`;
- run the full test solution and distinguish known infrastructure-dependent failures;
- run `.bmad-harness/tests/validate-harness.tests.ps1`;
- run `.bmad-harness/scripts/validate-harness.ps1`;
- lint generated BMAD artifacts using `-ArtifactRoot`.

## Out of Scope

- real payment-provider integration;
- payment decline and provider-timeout semantics;
- automatic authorization void or refund;
- retry policy;
- reservation expiration or release;
- partial reservation;
- notification policy;
- shipment-timing changes;
- invoicing;
- stronger cross-order concurrency guarantees than the repository's current event/projection consistency model;
- unrelated refactoring.

## BMAD Harness Gates

The native BMAD flow produces and validates:

1. product brief;
2. PRD;
3. architecture;
4. epics and stories;
5. implementation readiness;
6. implementation story;
7. development;
8. code review.

At each applicable phase, the committed harness guard must return `PASS` before advancing. `CONCERNS` requires explicit review; `FAIL` blocks the next phase.

