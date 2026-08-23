---
title: BrewUp Order Confirmation PRD
status: final
created: 2026-07-27
updated: 2026-07-27
---

# BrewUp Order Confirmation PRD

## Product objective

Make the BrewUp Sales Order `Confirmed` state evidence-backed. Confirmation
occurs only after Payment records an authorized outcome and Warehouse reserves
all requested rows. The feature must preserve bounded-context authority and the
existing modular-monolith dependency rules.

## Scope and authority

The approved design,
`docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`,
is the authoritative requirements input. Its **Confirmed Domain Decisions**
section resolves all-or-nothing reservation, retained authorization plus
terminal reservation failure without compensation, and coordination by the
existing `SalesOrderSaga` in Payment -> Warehouse -> Sales order. BC-000
through BC-011 and AR-000 through AR-018 remain binding constraints on those
decisions.

- Sales owns the Sales Order aggregate, commercial state, and final
  confirmation transition.
- Payment owns authorization requests, pending authorization state, and the
  authorized outcome supplied through a provider-simulator callback.
- Warehouse owns physical-stock truth, complete-order reservation, and its
  success or failure outcome.
- `SalesOrderSaga` coordinates messages in the established application flow. It
  retains external IDs but owns none of the producer decisions.

Payment is implemented as a complete module. Sales and the saga consume
Payment- and Warehouse-owned contracts through SharedKernel/integration
boundaries; they never reference another module's Domain or Infrastructure.

## Functional requirements

### Payment authorization

- **FR-001:** Payment shall own `PaymentAuthorizationId`,
  `PaymentAuthorization`, authorization commands, domain events, integration
  events, provider callback, projection, and module wiring.
- **FR-002:** `RequestPaymentAuthorization` shall create one pending
  authorization for a stable ID and Sales Order reference.
- **FR-003:** The provider-simulator callback shall supply an authorized outcome
  to Payment through `RecordPaymentAuthorized`; only Payment shall emit
  `PaymentAuthorized`.
- **FR-004:** Replaying the same authorization outcome shall be an idempotent
  no-op.

### Complete stock reservation

- **FR-005:** Warehouse shall assess every requested row for the complete Sales
  Order before issuing a single reservation decision.
- **FR-006:** Warehouse shall emit either one `StockReserved` containing its
  `StockReservationId` and all rows, or one `StockReservationFailed` and no
  reserved rows.
- **FR-007:** Warehouse availability projections shall subtract active,
  successful reservation quantities; failed reservations shall subtract
  nothing.
- **FR-008:** Replaying a reservation request for the stable reservation ID
  shall not reserve stock twice.

### Evidence-gated confirmation

- **FR-009:** Sales shall store nullable `PaymentAuthorizationId` and
  `StockReservationId` external references on the Sales Order.
- **FR-010:** `ConfirmSalesOrder` shall require both references before
  `SalesOrder.Confirm()` emits `SalesOrderConfirmed` and transitions to
  `Confirmed`.
- **FR-011:** Missing either reference shall leave the Sales Order unconfirmed
  and emit no confirmation event.
- **FR-012:** Replaying a confirmation request after the Sales Order is
  `Confirmed` shall emit no second confirmation.

### Saga coordination

- **FR-013:** After the existing budget and placement stages, the saga shall
  request Payment authorization, wait for `PaymentAuthorized`, request complete
  stock reservation, wait for `StockReserved`, then request Sales confirmation
  with both evidence IDs.
- **FR-014:** On `StockReservationFailed`, the saga shall record terminal
  reservation failure, leave the Sales Order unconfirmed, retain the recorded
  authorization, and send no confirmation or compensation command.
- **FR-015:** Replayed outcomes shall not repeat downstream commands or complete
  the saga twice.

### Structure and verification

- **FR-016:** Payment shall use exactly the six standard projects under
  `src/Payment/`: SharedKernel, Domain, ReadModel, Infrastructure, Facade, and
  Tests.
- **FR-017:** All six Payment projects shall be included in
  `src/BrewUp.slnx`, and `src/BrewUp.Rest/Module/PaymentModule.cs` shall
  register the module.
- **FR-018:** Project references shall follow the allowed layer directions;
  cross-module behavior shall use explicit SharedKernel contracts, ACL
  handlers, integration events, or facades.
- **FR-019:** New behavior shall begin with failing
  `CommandSpecification<T>` or equivalent saga/architecture tests and shall
  finish with targeted module, saga, architecture, solution-build, and harness
  verification.

## Acceptance criteria

- **AC1: Sales Order confirmation requires PaymentAuthorizationId and StockReservationId.**
- **AC2: Warehouse reserves all requested rows or none.**
- **AC3: Reservation failure leaves the Sales Order unconfirmed and retains the authorization.**
- **AC4: No void, refund, retry, timeout, notification, or expiry behavior is introduced.**
- **AC5: Duplicate outcomes cannot reserve or confirm twice.**

### Testable scenarios

1. Given a placed order and a Payment-owned authorized outcome, when Warehouse
   reserves every requested row, then Sales receives both evidence IDs and
   transitions exactly once to `Confirmed`.
2. Given any missing or insufficient requested row, when Warehouse evaluates
   the complete request, then it emits one failure, reserves no rows, and Sales
   remains unconfirmed.
3. Given Payment is authorized and reservation fails, when the saga records the
   failure, then the authorization remains recorded and no confirm, void,
   refund, retry, release, notification, timeout, or expiry action occurs.
4. Given only one evidence ID, when confirmation is requested, then Sales
   rejects the transition and emits no `SalesOrderConfirmed`.
5. Given a command or outcome is replayed, when the owning aggregate or saga has
   already processed the stable ID/state transition, then no duplicate
   reservation, confirmation, authorization, or completion occurs.

## Non-functional requirements

- **NFR-001 — Boundary integrity:** Domain projects shall have no references to
  infrastructure frameworks, Facade, or ReadModel, and modules shall not
  reference another module's Domain or Infrastructure.
- **NFR-002 — Determinism:** Stable external IDs and aggregate state guards
  shall make duplicate handling deterministic.
- **NFR-003 — Consistency model:** Read projections may be eventually
  consistent and rebuildable from events. This feature adds no stronger
  cross-order concurrency guarantee.
- **NFR-004 — Compatibility:** Existing budget verification, order placement,
  and shipment timing shall remain unchanged.
- **NFR-005 — Implementation discipline:** Use .NET 10/C# project conventions,
  `Guid.CreateVersion7()` for new GUIDs, and `ConfigureAwait(false)` on awaited
  tasks.

## Out of scope and open decisions

The increment does not define real provider execution, declined or unknown
outcomes, provider-timeout interpretation, compensation, void/refund, retry,
notification, reservation release/expiration, partial reservation, shipment
changes, invoicing, payment terms, or stronger cross-order concurrency.

No listed item is a hidden implementation requirement. If later work needs one,
it is a new domain decision and must be resolved by its owning bounded context
before implementation.

## Requirement traceability

| Requirement | Acceptance criteria | Governance |
|---|---|---|
| FR-001 through FR-004 | AC4, AC5 | BC-003; BC-004; BC-008; AR-004 through AR-008; AR-017 |
| FR-005 through FR-008 | AC2, AC5 | BC-005 through BC-008; AR-004 through AR-009; AR-017 |
| FR-009 through FR-012 | AC1, AC5 | BC-001; BC-002; BC-004; BC-006; BC-009; BC-010 |
| FR-013 through FR-015 | AC3, AC4, AC5 | BC-008; BC-011; AR-017; AR-018 |
| FR-016 through FR-018 | AC1 through AC5 | AR-001; AR-002; AR-004 through AR-017 |
| FR-019 | AC1 through AC5 | AR-012 |
| NFR-001 | AC1 through AC5 | AR-015; AR-016; AR-017 |
| NFR-002 through NFR-005 | AC3 through AC5 | BC-000; BC-011; AR-000; AR-009; AR-012 |

## Governance coverage

- BC-000 through BC-011 are implemented by FR-001 through FR-015 or preserved
  by the explicit exclusions above.
- AR-000 through AR-018 are implemented by FR-016 through FR-019, the
  SharedKernel contract requirements in FR-001/FR-005, the projection
  requirement in FR-007, and the saga boundary in FR-013 through FR-015.
- All-or-nothing reservation (FR-005/FR-006), post-authorization reservation
  failure (FR-014), and saga/order selection (FR-013) trace to the approved
  design's **Confirmed Domain Decisions** and are not inferred from BC-011.
