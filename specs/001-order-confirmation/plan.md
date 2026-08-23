# Implementation Plan: Sales Order Confirmation

**Branch**: `001-order-confirmation` | **Date**: 2026-06-25 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-order-confirmation/spec.md`

## Summary

Add a `Confirmed` outcome to the Sales-owned **Sales Order** aggregate. A Sales Order may only become `Confirmed` when it holds two external decision references — a **payment authorization** reference (produced by the Payment authority) and a **stock reservation** reference (produced by the Warehouse authority). When the commercial preconditions are met, the existing **Sales Order saga** (Sagas context) coordinates the flow: it requests the stock reservation from Warehouse, collects the payment-authorization and stock-reservation outcomes, and instructs Sales to confirm. Sales never authorizes payment, never reserves/mutates stock, and never embeds Payment or Warehouse models — it only records evidence and reacts. Delivered through Muflone CQRS/ES commands, domain events, and cross-context integration events declared in `BrewUp.Shared`.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: Muflone (CQRS + event sourcing — `AggregateRoot`, command/event handlers, `Muflone.SpecificationTests`), Muflone.Saga (`ISagaStartedByAsync`, `IIntegrationEventHandlerAsync`), Lena.Core (`Result<T>`), RabbitMQ (service/event bus), EventStore (write model), MongoDB (read model)

**Storage**: EventStore (aggregate event streams) for the write model; MongoDB for denormalized read models. No new stores introduced.

**Testing**: xUnit; Muflone.SpecificationTests (`CommandSpecification` Given/When/Expect aggregate tests); NetArchTest (architecture fitness functions). Property-based and mutation tooling are **not currently wired** in the repo (see Constitution Check).

**Target Platform**: Linux/Windows server (modular monolith hosted by `BrewUp.Rest`)

**Project Type**: DDD modular monolith organized by bounded context, with CQRS and message/event-driven flows

**Performance Goals**: Not performance-sensitive; correctness/invariant-driven. Confirmation is an event-driven transition, not a hot path.

**Constraints**: Domain layer MUST stay pure (no infrastructure deps); cross-context coupling only through `BrewUp.Shared` contracts; no shared tables or direct calls into other contexts' internals; strongly-typed identifiers (no naked primitives crossing boundaries).

**Scale/Scope**: One new aggregate behavior (`Confirm`), one new status, two new value objects, a small set of commands/events/integration events, and saga wiring. No UI work in scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design.*

| Principle | Assessment | Status |
|-----------|-----------|--------|
| I. Domain-Driven Design | Confirmation invariant (both references present) enforced **inside** the `SalesOrder` aggregate; state change expressed as the `SalesOrderConfirmed` domain event; evidence modeled as immutable, value-compared, strongly-typed value objects (`PaymentAuthorizationId`, `StockReservationId`); domain layer stays pure. | PASS |
| II. Modular Architecture & Bounded Contexts | Sales depends on Payment/Warehouse **decisions** only via integration-event contracts in `BrewUp.Shared`; Sales does not reference Payment/Warehouse internals; no embedded foreign models (BC-002/BC-009). | PASS |
| III. Test-First Discipline | Plan mandates failing aggregate specs (`CommandSpecification`) for confirm-success, missing-evidence rejection, and idempotency **before** implementation; saga reaction tests before wiring. | PASS (enforced in tasks) |
| IV. Architecture Fitness Functions | Existing NetArchTest suites (`SalesArchitectureTests`, `SagasArchitectureTests`) already assert domain purity and forbid cross-module internal deps; new code must keep them green; no suppressions added. | PASS |
| V. Property-Based & Mutation Testing | Constitution requires property-based + mutation testing for new domain invariants. **No tooling (CsCheck/FsCheck/Stryker) currently exists in the repo.** Recorded as a documented deviation with a follow-up; the confirmation invariant is otherwise covered by deterministic example-based aggregate specs. | DEVIATION (documented) |

**Domain Carrier gates (BC-000 → BC-011)**: All preserved — see [research.md](research.md) §"Domain Carrier Compliance". No violations.

## Project Structure

### Documentation (this feature)

```text
specs/001-order-confirmation/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (message contracts)
│   ├── commands.md
│   ├── domain-events.md
│   └── integration-events.md
├── checklists/
│   └── requirements.md  # Spec quality checklist (already present)
└── tasks.md             # Created by /speckit.tasks (NOT this command)
```

### Source Code (repository root)

Changes are confined to the **Sales** and **Sagas** bounded contexts plus shared contracts in `BrewUp.Shared`. Warehouse and Payment are decision producers reached only through `BrewUp.Shared` contracts; producing those outcomes is out of scope for this feature (see research.md D1/D2).

```text
src/
├── BrewUp.Shared/
│   ├── DomainIds/
│   │   ├── PaymentAuthorizationId.cs          # NEW — external decision reference
│   │   └── StockReservationId.cs              # NEW — external decision reference
│   └── Messages/Events/Sagas/
│       ├── PaymentAuthorized.cs               # NEW — Payment outcome (integration event)
│       ├── PaymentAuthorizationFailed.cs      # NEW — Payment outcome (integration event)
│       ├── StockReserved.cs                   # NEW — Warehouse outcome (integration event)
│       └── StockReservationFailed.cs          # NEW — Warehouse outcome (integration event)
│
├── Sales/
│   ├── BrewUp.Sales.SharedKernel/
│   │   ├── Enums/SalesOrderStatus.cs          # EDIT — add Confirmed
│   │   ├── CustomTypes/PaymentAuthorizationId.cs  # NEW — Sales-local VO wrapper
│   │   ├── CustomTypes/StockReservationId.cs      # NEW — Sales-local VO wrapper
│   │   ├── Messages/Commands/ConfirmSalesOrder.cs # NEW
│   │   └── Messages/Events/SalesOrderConfirmed.cs # NEW — Sales-owned domain event
│   ├── BrewUp.Sales.Domain/
│   │   ├── Entities/SalesOrder.cs             # EDIT — Confirm(); invariant; Apply(SalesOrderConfirmed)
│   │   └── CommandHandlers/ConfirmSalesOrderCommandHandler.cs # NEW
│   └── BrewUp.Sales.Tests/Domain/
│       ├── ConfirmSalesOrderSuccessfully.cs       # NEW
│       ├── ConfirmRejectedWhenEvidenceMissing.cs  # NEW
│       └── ConfirmIsIdempotent.cs                 # NEW
│
└── Sagas/
    ├── BrewUp.Sagas.Domain/
    │   ├── Entities/SalesOrderSaga.cs         # EDIT — track payment + reservation evidence; emit ConfirmSalesOrder
    │   └── Orchestrators/SalesOrderSagaOrchestrator.cs # EDIT — handle PaymentAuthorized + StockReserved
    └── BrewUp.Sagas.Tests/Orchestrators/
        └── SalesOrderSagaConfirmationTests.cs # NEW
```

**Structure Decision**: Follow the established per-context layered layout (`Domain` → `SharedKernel` → `Infrastructure`/`ReadModel` → `Facade` → `Tests`). The confirmation invariant lives in `BrewUp.Sales.Domain` (`SalesOrder`); orchestration lives in `BrewUp.Sagas.Domain` (`SalesOrderSaga` + orchestrator); all cross-context messages live in `BrewUp.Shared`. Warehouse-side production of the reservation outcome and any Payment-side production are out of scope and are reached only through the shared contracts.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Property-based + mutation testing not provided for the new invariant (Principle V) | No CsCheck/FsCheck/Stryker tooling exists in the repo today; wiring a mutation pipeline is a cross-cutting infrastructure change beyond this feature's scope | Adding and configuring property-based + mutation tooling now would expand scope well past "order confirmation" and touch CI for all contexts; the invariant is covered by deterministic example-based aggregate specs in the interim. Follow-up: introduce CsCheck for the confirmation invariant and a Stryker mutation gate for `BrewUp.Sales.Domain`. |
| New cross-context outcome contracts (`PaymentAuthorized`, `StockReserved`, …) introduced while their producers are out of scope | The spec/domain carrier require Payment and Warehouse to be the authorities; Sales must react to their decisions through a boundary, so the contracts must exist even though producers are external/follow-up | Letting Sales derive payment/stock conclusions itself would violate BC-003/BC-005/BC-007 (Sales authorizing payment or owning stock truth). Defining decision-reference contracts is the least-coupling way to honor the bounded-context split. |

## Phase 0 / Phase 1 Outputs

- Phase 0 → [research.md](research.md)
- Phase 1 → [data-model.md](data-model.md), [contracts/](contracts/), [quickstart.md](quickstart.md)
- Agent context updated: plan reference set in `.github/copilot-instructions.md`.
