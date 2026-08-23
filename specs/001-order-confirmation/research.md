# Phase 0 Research: Sales Order Confirmation

All Technical Context items are known from the existing codebase; no `NEEDS CLARIFICATION` remained after spec clarification. This document records the design decisions, rationale, and rejected alternatives, plus the domain-carrier compliance mapping.

## Existing-System Findings

- **Framework**: Muflone provides `AggregateRoot`, `CommandHandlerAsync<T>`, event sourcing, and `Muflone.SpecificationTests` (`CommandSpecification` with `Given()/When()/Expect()`). Sagas use `Muflone.Saga` (`ISagaStartedByAsync`, `IIntegrationEventHandlerAsync`).
- **Sales Order aggregate** (`BrewUp.Sales.Domain.Entities.SalesOrder`) already supports `Create`, `AddBeers`, `AcceptOrder`, `CloseSalesOrder`, raising domain events applied to mutate state. Status is `BrewUp.Sales.SharedKernel.Enums.SalesOrderStatus` (`Accepted`, `WorkInProgress`, `Completed`, `Closed`, `Rejected`).
- **Saga** (`BrewUp.Sagas.Domain.Orchestrators.SalesOrderSagaOrchestrator` + `SalesOrderSaga`) already coordinates a flow reacting to `CustomerBudgetVerified`/`CustomerBudgetUnVerified` and `RequestBeersAvailabilityChecked`, then emits `SagasSalesOrderAccepted` and marks completion.
- **No Payment context exists** — repository-wide search for `Payment`/`Authoriz` returns zero matches. The closest existing concept is `CustomerBudgetVerified`, which is a budget check, **not** a payment authorization.
- **No stock reservation exists yet** — Warehouse has `Availability` (read of stock) and an availability check (`RequestBeersAvailabilityChecked`), and `RequestBeerAvailablityRaisedEventHandler` contains an explicit `// At least we need a command to reserve the quantity // ReserveBeer command` TODO. Availability is computed, not reserved; consistent with the carrier's "availability is not durable".
- **Strongly-typed ids**: `BrewUp.Shared.DomainIds.*` derive from `Muflone.Core.DomainId(string)`. Existing shared contract types: `ItemRequested(BeerId, Quantity QuantityOrdered, Quantity QuantityAvailable)`, `SalesOrderRowJson`.
- **Testing**: xUnit + `Muflone.SpecificationTests`; architecture fitness via NetArchTest (`SalesArchitectureTests`, `SagasArchitectureTests`). No property-based (CsCheck/FsCheck) or mutation (Stryker) tooling present.

## Decisions

### D1 — Payment authorization is an external decision reached via a `BrewUp.Shared` integration event; no Payment module is built here

- **Decision**: Model the Payment outcome as integration-event contracts in `BrewUp.Shared` (`PaymentAuthorized` carrying a `PaymentAuthorizationId`, and `PaymentAuthorizationFailed`). Sales/Sagas react to these. Producing the outcome (the actual Payment authority/provider integration) is **out of scope** for this feature and is supplied later or by a test double.
- **Rationale**: BC-003/BC-004 forbid Sales from authorizing payment or interpreting provider behavior. The bounded-context split requires Sales to depend only on a decision delivered through a boundary. A contract is the minimum surface that honors this without inventing Payment internals.
- **Alternatives considered**:
  - *Reuse `CustomerBudgetVerified` as payment evidence* — rejected: a budget check is not a payment authorization; conflating them invents Payment semantics inside Sales/Sagas and violates the ubiquitous language.
  - *Build a full Payment bounded context now* — rejected: large scope expansion beyond "order confirmation"; the feature only needs the decision reference and a reaction point.

### D2 — Stock reservation is a new Warehouse-owned decision exposed via `BrewUp.Shared`; Sales never reserves stock

- **Decision**: Define `StockReserved` (carrying a `StockReservationId`) and `StockReservationFailed` integration-event contracts in `BrewUp.Shared`, plus a reservation **request** contract the saga emits toward Warehouse. The saga requests the reservation; Warehouse produces the outcome. Warehouse-side production of the reservation (the `ReserveBeer`/`ReserveStock` command handler and aggregate behavior) is acknowledged as a follow-up but the contract is defined now so Sales can react.
- **Rationale**: BC-005/BC-006/BC-007 — Warehouse owns stock and reservation; only a durable reservation supports confirmation. Sales stores `StockReservationId` as evidence only.
- **Alternatives considered**:
  - *Treat availability-checked as sufficient for confirmation* — rejected: availability is explicitly non-durable (carrier); confirming on it risks confirming stock that is no longer reservable.
  - *Have Sales call Warehouse availability and decide* — rejected: makes Sales the authority of truth for stock (BC-007 violation).

### D3 — The confirmation invariant lives inside the `SalesOrder` aggregate

- **Decision**: Add `SalesOrder.Confirm(PaymentAuthorizationId, StockReservationId, correlationId)` that raises `SalesOrderConfirmed` only when **both** references are present; `Apply(SalesOrderConfirmed)` records both references and sets status to `Confirmed`. Add `Confirmed` to `SalesOrderStatus`.
- **Rationale**: Principle I + BC-010 — invariants belong in the aggregate; no aggregate may exist in a `Confirmed`-without-evidence state. Expressing the change as a domain event preserves event sourcing.
- **Alternatives considered**:
  - *Enforce the rule in the command handler or saga* — rejected: leaks an invariant out of the domain (Principle I), allowing the aggregate to be driven into an invalid state by another path.

### D4 — The existing Sales Order saga coordinates; Sales only reacts to `ConfirmSalesOrder`

- **Decision**: Extend `SalesOrderSaga` to track the payment-authorization and stock-reservation evidence as it arrives (`PaymentAuthorized`, `StockReserved`). When both are present, the saga sends a `ConfirmSalesOrder` command to Sales carrying both reference values. `ConfirmSalesOrderCommandHandler` loads the aggregate and calls `Confirm`.
- **Rationale**: FR-012 (resolved Q2) assigns cross-context orchestration to the saga; keeps Sales a pure reactor (BC-008).
- **Alternatives considered**:
  - *A Sales application service orchestrates* — rejected by the resolved clarification (Q2 = saga).

### D5 — Confirmation is idempotent

- **Decision**: `SalesOrder.Confirm` is a no-op when status is already `Confirmed` (no second `SalesOrderConfirmed`); the saga guards against re-sending `ConfirmSalesOrder` once confirmed.
- **Rationale**: FR-009; message buses can redeliver. Prevents duplicate confirmation and duplicate downstream reservation requests.

### D6 — Failure handling stays minimal and within bounds

- **Decision**: On `PaymentAuthorizationFailed` or `StockReservationFailed`, the Sales Order is **not** confirmed and remains in its pre-confirmation status; the saga records the failure. Sales does **not** void/refund payment or release stock (owned by Payment/Warehouse). No new compensation policy is invented.
- **Rationale**: Resolved Q1/Q3 — order stays pre-confirmation; Payment owns release/void; out of Sales scope. FR-011/FR-014.

### D7 — Evidence references modeled as strongly-typed value objects

- **Decision**: Introduce `PaymentAuthorizationId` and `StockReservationId` as `DomainId`-derived shared ids, with Sales-local value-object wrappers for the aggregate's stored evidence.
- **Rationale**: Principle I — no naked primitives crossing boundaries; immutable, value-compared identifiers.

## Domain Carrier Compliance (BC-000 → BC-011)

| Rule | How the plan complies |
|------|----------------------|
| BC-000 Authoritative rules | Carried into spec + this plan; constrain all artifacts. |
| BC-001 Sales owns lifecycle | `SalesOrder` owns the `Confirmed` transition. |
| BC-002 No embedded foreign models | Aggregate stores only `PaymentAuthorizationId`/`StockReservationId`, never Payment/Warehouse models. |
| BC-003 Payment owns authorization | No payment authorization logic in Sales/Sagas; only reaction to `PaymentAuthorized`. |
| BC-004 Payment Authorization is external | Modeled as a `BrewUp.Shared` integration event; Sales stores the id as evidence. |
| BC-005 Warehouse owns stock | No stock truth in Sales; reservation requested from Warehouse. |
| BC-006 Stock Reservation is external | Modeled as a `BrewUp.Shared` integration event; Sales stores the id as evidence. |
| BC-007 Warehouse owns stock mutation | Sales never reserves/releases/decrements stock. |
| BC-008 Reacting is not owning | Saga/Sales react to outcomes; never produce them. |
| BC-009 References, not embedded models | Only ids stored. |
| BC-010 Confirmed requires evidence | Invariant in `SalesOrder.Confirm` requires both ids. |
| BC-011 Clarification preserves authority | Open questions retained in spec; resolved ones recorded explicitly. |

## Open Risks / Follow-ups

- **Property-based + mutation testing** (Principle V) not yet wired — tracked in plan Complexity Tracking.
- **Warehouse-side reservation producer** (`ReserveStock` handler + aggregate behavior) and **Payment-side producer** are out of scope; this feature defines the contracts and the Sales/Sagas reaction. End-to-end validation uses test doubles emitting the outcome integration events (see quickstart.md).
