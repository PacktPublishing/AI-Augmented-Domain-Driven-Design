# Phase 0 Research: Sales Order Confirmation

This document resolves the technical unknowns for the plan. The domain decisions were resolved in the spec's Clarifications (Session 2026-06-27); the items below are technical/structural decisions only and do **not** move decision authority (BC-000…BC-011, AR-000…AR-018 preserved).

## R1 — Is Payment in scope, and how is it implemented?

- **Decision**: Payment is **in scope** and implemented as a **full BrewUp module** `src/Payment/` with the standard 6 projects.
- **Rationale**: The feature requires payment-authorization outcomes (`PaymentAuthorized` / `PaymentDeclined`) and a command to authorize. The architecture memory "When Payment must be created" criteria are all met (separate authority, behavior required, not declared external). Constitution II and AR-001/AR-002/AR-016 mandate a full module rather than folders inside Sales or the Saga.
- **Alternatives considered**: (a) Treat Payment as an external already-existing system — rejected because the spec does not declare it external and the feature must demonstrate the authorization outcome end-to-end. (b) Put Payment commands/events in Sales or Sagas — rejected, violates AR-016.

## R2 — Coordination mechanism (OQ-5/OQ-6 resolved → coordinator dispatches Confirm)

- **Decision**: Extend the **existing `SalesOrderSaga`** and its `SalesOrderSagaOrchestrator` as the coordinator. It requests payment authorization and stock reservation in parallel, reacts to outcomes, and raises a `SagaSalesOrderReadyToConfirm` gate event when both evidences are present; the Saga `ReadModel` publishes an integration event that the Sales ACL turns into a `ConfirmSalesOrder` command.
- **Rationale**: A saga is the sanctioned BrewUp coordination mechanism (AR-018) and already orchestrates the Sales Order lifecycle (budget → availability → placement → acceptance). Reusing it avoids a competing coordinator. The saga coordinates without owning Payment/Warehouse decisions.
- **Alternatives considered**: A new dedicated confirmation saga (more moving parts, duplicate correlation handling) — rejected for simplicity. An application service in Sales — rejected because it would pull cross-context coordination into Sales.

## R3 — How does Sales store external decision references without coupling? (BC-009, AR-015)

- **Decision**: Sales stores the references as its **own** value objects `PaymentAuthorizationReference` and `StockReservationReference` (each derived from `Muflone.Core.DomainId`) in `BrewUp.Sales.SharedKernel/CustomTypes`. The raw id strings travel inside the `SagaSalesOrderReadyToConfirmIntegrationEvent` and the `ConfirmSalesOrder` command; Sales wraps them.
- **Rationale**: Keeps Sales free of any compile dependency on `Payment.*` or `Warehouse.*` projects (AR-015) while remaining strongly typed (Constitution I — no naked primitives across boundaries). The references are evidence, not handles into another aggregate (BC-009).
- **Alternatives considered**: (a) Reference `Payment.SharedKernel.PaymentAuthorizationId` and `Warehouse.SharedKernel.StockReservationId` directly from Sales — allowed (SharedKernel is the contract surface) but introduces cross-module compile coupling; rejected to keep Sales maximally decoupled. (b) Store raw `string` — rejected, violates strongly-typed-ID rule.

## R4 — Event/command naming and placement (AR-004…AR-007, existing conventions)

- **Decision**:
  - Payment **domain** events/commands live in `BrewUp.Payment.SharedKernel/Messages` (`AuthorizePayment`, `PaymentAuthorized`, `PaymentDeclined`).
  - Warehouse reservation command/events live in `BrewUp.Warehouse.SharedKernel/Messages` (`ReserveStock`, `StockReserved`, `StockReservationRejected`).
  - Sales confirmation command/event live in `BrewUp.Sales.SharedKernel/Messages` (`ConfirmSalesOrder`, `SalesOrderConfirmed`).
  - **Saga-facing integration events** live in `BrewUp.Shared/Messages/Events/Sagas` with the `…IntegrationEvent` suffix (matching existing `SalesOrderSagaStartedIntegrationEvent`): `PaymentAuthorizedIntegrationEvent`, `PaymentDeclinedIntegrationEvent`, `StockReservedIntegrationEvent`, `StockReservationRejectedIntegrationEvent`, `SagaSalesOrderReadyToConfirmIntegrationEvent`.
- **Rationale**: Mirrors the established pattern where a module raises a `DomainEvent` and its `ReadModel` publishes a corresponding cross-context `IntegrationEvent` consumed by the saga / ACL handlers (e.g., `SalesOrderAccepted` → `SagasSalesOrderAccepted`).
- **Alternatives considered**: Putting integration events in each module's SharedKernel — rejected; the repository centralizes saga-facing integration contracts in `BrewUp.Shared`.

## R5 — Partial reservation representation (OQ-2 resolved → partial allowed)

- **Decision**: `StockReserved` carries the reserved rows (the subset Warehouse actually reserved) and a single `StockReservationId`. The saga marks stock as reserved with that id. Sales stores the `StockReservationId` reference; it does not recompute or decide the subset.
- **Rationale**: Warehouse owns which beers are reservable (BC-005); a single durable reservation id is sufficient evidence for confirmation (BC-006). Handling of the unreserved remainder is out of scope (FR-011/OQ-1).
- **Alternatives considered**: Per-line reservation ids — rejected as unnecessary for the confirmation evidence and out of scope.

## R6 — Idempotent confirmation (FR-009)

- **Decision**: The `SalesOrder` aggregate guards the `Confirmed` transition: if already `Confirmed`, `ConfirmOrder(...)` is a no-op (no event raised). The saga's gate marks ReadyToConfirm only once.
- **Rationale**: Event-sourced replay + at-least-once messaging require idempotent state transitions. Guarding in the aggregate keeps the invariant in the domain (Constitution I).
- **Alternatives considered**: Dedup at the handler — rejected; the invariant belongs in the aggregate.

## R7 — Status modeling

- **Decision**: Add `Confirmed` to `SalesOrderStatus` (Enumeration smart-enum) with the next free id (`6`). The aggregate transitions to `Confirmed` only inside `Apply(SalesOrderConfirmed)`.
- **Rationale**: Follows the existing `SalesOrderStatus` pattern; no naked status strings.

## R8 — Testing approach (Constitution III & V)

- **Decision**: For each behavioral change, write a failing `CommandSpecification<T>` first (Given prior events / When command / Expect events). Add architecture fitness tests for the new Payment module and for the new cross-module boundaries. Express the confirmation invariant (Confirmed ⇒ both references present) as a property-based test; include Payment/Sales domain in the mutation-testing scope.
- **Rationale**: Mandated by the constitution; matches existing `*Successfully` command specs in each module's `Tests/Domain`.

## Resolved unknowns

All Technical Context items are resolved; no `NEEDS CLARIFICATION` remains. Outstanding **domain** open questions OQ-4 (reservation lifetime — Warehouse) and OQ-7 (notification/downstream) remain intentionally unresolved and are **not** implemented by this plan.
