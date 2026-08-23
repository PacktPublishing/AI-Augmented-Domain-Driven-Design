# Phase 1 Data Model: Sales Order Confirmation

Entities are grouped by owning bounded context. Strongly-typed IDs derive from `Muflone.Core.DomainId`; status values use the `Enumeration` smart-enum. Aggregates enforce invariants; no aggregate may exist in an invalid state (Constitution I).

## Sales context

### SalesOrder *(aggregate root — existing, extended)*

| Field | Type | Notes |
|---|---|---|
| Id | `SalesOrderId` | existing |
| _salesOrderStatus | `SalesOrderStatus` | **add** `Confirmed` value |
| _paymentAuthorizationReference | `PaymentAuthorizationReference?` | **new** — external decision reference (evidence) |
| _stockReservationReference | `StockReservationReference?` | **new** — external decision reference (evidence) |

**New value objects** (`BrewUp.Sales.SharedKernel/CustomTypes`):

- `PaymentAuthorizationReference : DomainId` — evidence that Payment authorized the customer payment.
- `StockReservationReference : DomainId` — evidence that Warehouse reserved the requested beers (possibly a subset).

**New behavior** `SalesOrder.ConfirmOrder(PaymentAuthorizationReference paymentRef, StockReservationReference stockRef, Guid correlationId)`:

- **Invariant (BC-010)**: both references MUST be non-null/non-empty; otherwise the order is not confirmed (raise no `SalesOrderConfirmed`, optionally raise an error event — no compensation invented, FR-011).
- **Idempotency (FR-009)**: if status is already `Confirmed`, no event is raised.
- On success: `RaiseEvent(new SalesOrderConfirmed(new SalesOrderId(Id.Value), correlationId, paymentRef, stockRef))`.
- `Apply(SalesOrderConfirmed)`: sets `_salesOrderStatus = SalesOrderStatus.Confirmed`, stores both references.

**State transition**: `Accepted/WorkInProgress → Confirmed` (only when both references present).

### SalesOrderStatus *(enumeration — extended)*

Add: `Confirmed = new(6, "confirmed")`; include in `List()`.

## Payment context *(new module)*

### PaymentAuthorization *(aggregate root — new)*

| Field | Type | Notes |
|---|---|---|
| Id | `PaymentAuthorizationId` | strongly-typed |
| _status | `PaymentAuthorizationStatus` | Authorized / Declined / Pending |
| _salesOrderId | `string` | correlation back to the originating Sales Order |
| _amount | `Price` (Shared) | authorized amount |

**Behavior** `PaymentAuthorization.Authorize(...)`: produces the authorization **decision** (Payment owns it). On approve → `RaiseEvent(PaymentAuthorized)`; on decline → `RaiseEvent(PaymentDeclined)`. Payment owns timeout interpretation (not implemented here beyond definitive outcomes; OQ-3).

### PaymentAuthorizationStatus *(enumeration — new)*

`Authorized(1)`, `Declined(2)`, `Pending(3)`.

### PaymentAuthorizationId *(DomainId — new)*

`public sealed class PaymentAuthorizationId(string value) : DomainId(value);`

## Warehouse context *(existing, extended)*

### Availability *(aggregate root — existing, extended)*

**New behavior** `Availability.ReserveStock(...)`: attempts to reserve the requested beers; Warehouse decides the reservable subset (partial allowed, OQ-2). On success → `RaiseEvent(StockReserved(stockReservationId, reservedRows, ...))`; on failure (nothing reservable) → `RaiseEvent(StockReservationRejected(reason, ...))`. Sales never calls this.

### StockReservationId *(DomainId — new)*

`public sealed class StockReservationId(string value) : DomainId(value);` (owned by Warehouse).

## Sagas context *(existing, extended)*

### SalesOrderSaga *(aggregate root — existing, extended)*

| Field | Type | Notes |
|---|---|---|
| _paymentAuthorizationId | `string` | evidence collected from Payment |
| _stockReservationId | `string` | evidence collected from Warehouse |
| _paymentAuthorized | `bool` | gate flag |
| _stockReserved | `bool` | gate flag |

**New behavior**:

- `MarkPaymentAuthorized(string paymentAuthorizationId, Guid correlationId)` → `RaiseEvent(...)`, sets flag; then evaluates the gate.
- `MarkStockReserved(string stockReservationId, Guid correlationId)` → sets flag; then evaluates the gate.
- `MarkPaymentDeclined(...)` / `MarkStockReservationRejected(...)` → record negative outcome; **no** automatic compensation (FR-011/OQ-1); saga stays unconfirmed.
- **Gate**: when `_paymentAuthorized && _stockReserved`, `RaiseEvent(SagaSalesOrderReadyToConfirm(salesOrderId, paymentAuthorizationId, stockReservationId, correlationId))` exactly once.

## Evidence / reference summary (BC-009)

| Reference | Produced by | Stored by (evidence) | Carried in |
|---|---|---|---|
| `PaymentAuthorizationId` | Payment | Sales (`PaymentAuthorizationReference`), Saga | `PaymentAuthorizedIntegrationEvent`, gate event, `ConfirmSalesOrder` |
| `StockReservationId` | Warehouse | Sales (`StockReservationReference`), Saga | `StockReservedIntegrationEvent`, gate event, `ConfirmSalesOrder` |

## Confirmation invariant (authoritative)

```text
SalesOrder.Status = Confirmed  ⇒  PaymentAuthorizationReference present  ∧  StockReservationReference present
```

Both negations (either reference missing) MUST NOT yield `Confirmed`. This is enforced inside the `SalesOrder` aggregate and verified by a property-based test (Constitution V).
