# Implementation Plan: Sales Order Confirmation

**Branch**: `001-order-confirmation` | **Date**: 2026-06-27 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-order-confirmation/spec.md`

## Summary

A Sales Order becomes `Confirmed` only when two **external decisions** exist as evidence: a **Payment Authorization** (produced by Payment) and a **Stock Reservation** (produced by Warehouse). Confirmation is coordinated by the existing `SalesOrderSaga`, which requests both decisions **in parallel**, reacts to their outcomes, and — when both are present — dispatches a `ConfirmSalesOrder` command to Sales. Sales owns and performs the transition, storing `PaymentAuthorizationId` and `StockReservationId` as external decision references (never embedding the producing contexts' models). Partial stock reservation is permitted; the reservation may cover a subset of requested beers (Warehouse decides the subset). Sales never authorizes payment, never reserves/releases stock, and never interprets payment-provider timeouts.

Technical approach: extend three existing modules (`Sales`, `Warehouse`, `Sagas`) and create one new module (`Payment`) following the standard BrewUp module structure, using Muflone CQRS/Event Sourcing with EventStore (write), MongoDB (read), and RabbitMQ (transport). Cross-context coupling flows only through commands, domain events, integration events, and ACL handlers.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: Muflone (`AggregateRoot`, `CommandHandlerAsync<T>`, `DomainEvent`, `IntegrationEvent`, `ISagaStartedByAsync`, `IIntegrationEventHandlerAsync<T>`), `Muflone.Transport.RabbitMQ`, `Muflone.Eventstore.gRPC`, `MongoDB.Driver`

**Storage**: EventStore (gRPC) for the write model; MongoDB for read models

**Testing**: xUnit + Muflone test helpers (`CommandSpecification<T>` Given/When/Expect); NetArchTest for architecture fitness functions; property-based tests for the confirmation invariant; mutation testing wired in CI per Constitution V

**Target Platform**: ASP.NET Core host (`BrewUp.Rest`) — modular monolith

**Project Type**: DDD Modular Monolith (CQRS + Event Sourcing)

**Performance Goals**: Not performance-critical; correctness of the confirmation invariant is the priority. Confirmation must be idempotent under repeated/out-of-order evidence.

**Constraints**: Domain layer pure (no infrastructure references); `Guid.CreateVersion7()` everywhere; `ConfigureAwait(false)` on every non-test await; strongly-typed IDs derive from `Muflone.Core.DomainId`; status/type values use the `Enumeration` smart-enum.

**Scale/Scope**: 1 new module (Payment) + 3 extended modules (Sales, Warehouse, Sagas) + Shared integration events + 1 REST module registration. No UI work in scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Status |
|---|---|---|
| I. Domain-Driven Design (NON-NEGOTIABLE) | Invariant (Confirmed requires both references) enforced **inside** the `SalesOrder` aggregate; domain layer pure; strongly-typed IDs; state changes via domain events; ubiquitous language ("Sales Order", "Payment Authorization", "Stock Reservation") | PASS |
| II. Modular Architecture, Bounded Contexts & Module Structure | Payment introduced as a **separate authority** → created as a full module `src/Payment/` with the standard 6 projects (AR-001, AR-002); Warehouse/Sales/Sagas extended in place; cross-context only via contracts; explicit `IModule` registration (AR-013) | PASS |
| III. Test-First Discipline (NON-NEGOTIABLE) | Each behavioral change starts with a failing `CommandSpecification<T>`; tasks order tests before implementation | PASS |
| IV. Architecture Fitness Functions | Each module's `Tests/Architecture` enforces dependency direction (AR-015): no `Domain → Infrastructure/ReadModel/Facade`, no `ModuleA → ModuleB.Domain/Infrastructure` | PASS |
| V. Property-Based & Mutation Testing | Confirmation invariant expressed as a property (Confirmed ⇒ both references present); mutation testing run over Sales/Payment domain | PASS |
| VI. Spec-Driven Development & Agent Governance | BC-000…BC-011 and AR-000…AR-018 preserved; open questions OQ-4/OQ-7 remain unresolved (not turned into implementation); no invented compensation/retry/notification policy | PASS |

**Result**: No violations. Complexity Tracking section intentionally empty.

### Bounded-context ownership decisions (authoritative for tasks)

- **Payment is in scope and implemented as a full module** (`src/Payment/`). Rationale: the feature requires payment-authorization *behavior and outcomes* (a command to authorize and `PaymentAuthorized`/`PaymentDeclined` outcomes) for confirmation to be demonstrable end-to-end. Per the architecture memory "When Payment must be created" criteria (separate authority ✓, behavior required ✓, not declared external ✓), a full Payment module is mandatory (AR-001, AR-002, AR-016).
- **Warehouse owns Stock Reservation** and is extended with a `ReserveStock` command and `StockReserved` / `StockReservationRejected` outcomes (BC-005, BC-006, BC-007). Sales never reserves/releases stock.
- **Sales owns the Sales Order lifecycle** and the `Confirmed` transition; it stores `PaymentAuthorizationId` and `StockReservationId` as external decision references only (BC-001, BC-002, BC-009).
- **The existing `SalesOrderSaga` is the coordinator** (extended, not duplicated). It requests both decisions in parallel, collects evidence, and raises a gate event when both are present; it does **not** own Payment or Warehouse decisions (BC-010, AR-018). Choosing the saga as the coordination mechanism is a technical choice that does not move decision authority.

## Project Structure

### Documentation (this feature)

```text
specs/001-order-confirmation/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (command & event contracts)
│   ├── payment.md
│   ├── warehouse-reservation.md
│   ├── sales-confirmation.md
│   └── saga-coordination.md
├── checklists/
│   └── requirements.md  # Spec quality checklist (already present)
└── tasks.md             # Created by /speckit.tasks (NOT by /speckit.plan)
```

### Source Code (repository root)

New module — **Payment** (full standard structure, AR-002):

```text
src/Payment/
├── BrewUp.Payment.SharedKernel/
│   ├── DomainIds/
│   │   └── PaymentAuthorizationId.cs           # : DomainId
│   ├── Enums/
│   │   └── PaymentAuthorizationStatus.cs        # : Enumeration (Authorized, Declined, Pending)
│   └── Messages/
│       ├── Commands/
│       │   └── AuthorizePayment.cs              # : Command
│       └── Events/
│           ├── PaymentAuthorized.cs             # : DomainEvent
│           └── PaymentDeclined.cs               # : DomainEvent
├── BrewUp.Payment.Domain/
│   ├── Entities/
│   │   └── PaymentAuthorization.cs              # AggregateRoot — authorize() decision
│   ├── CommandHandlers/
│   │   └── AuthorizePaymentCommandHandler.cs
│   └── PaymentDomainHelper.cs
├── BrewUp.Payment.ReadModel/
│   ├── EventHandlers/
│   │   ├── PaymentAuthorizedEventHandler.cs     # publishes PaymentAuthorizedIntegrationEvent
│   │   └── PaymentDeclinedEventHandler.cs
│   ├── Queries/
│   ├── Dtos/
│   └── PaymentReadModelHelper.cs
├── BrewUp.Payment.Infrastructure/
│   └── InfrastructureHelper.cs
├── BrewUp.Payment.Facade/
│   ├── Acl/                                      # ACL handler dispatches AuthorizePayment on saga request
│   ├── Endpoints/
│   │   └── PaymentEndpoints.cs
│   ├── IPaymentFacade.cs
│   ├── PaymentFacade.cs
│   └── PaymentFacadeHelper.cs                    # AddPaymentFacade(configuration)
└── BrewUp.Payment.Tests/
    ├── Architecture/
    └── Domain/
```

Extended existing modules (new files only listed):

```text
src/Sales/
├── BrewUp.Sales.SharedKernel/
│   ├── CustomTypes/
│   │   ├── PaymentAuthorizationReference.cs     # external decision reference stored by Sales
│   │   └── StockReservationReference.cs         # external decision reference stored by Sales
│   ├── Enums/SalesOrderStatus.cs                # ADD `Confirmed`
│   └── Messages/
│       ├── Commands/ConfirmSalesOrder.cs        # carries paymentAuthorizationId + stockReservationId
│       └── Events/SalesOrderConfirmed.cs        # DomainEvent (Sales-owned)
├── BrewUp.Sales.Domain/
│   ├── Entities/SalesOrder.cs                   # ADD ConfirmOrder(...) + invariant + Apply
│   ├── CommandHandlers/ConfirmSalesOrderCommandHandler.cs
│   └── DomainHelper.cs                           # register handler
├── BrewUp.Sales.ReadModel/EventHandlers/SalesOrderConfirmedEventHandler.cs
└── BrewUp.Sales.Facade/Acl/SagaSalesOrderReadyToConfirmIntegrationEventHandler.cs

src/Warehouse/
├── BrewUp.Warehouse.SharedKernel/
│   ├── CustomTypes/StockReservationId.cs        # : DomainId (Warehouse owns reservation identity)
│   └── Messages/
│       ├── Commands/ReserveStock.cs
│       └── Events/StockReserved.cs, StockReservationRejected.cs
├── BrewUp.Warehouse.Domain/
│   ├── Entities/Availability.cs                 # ADD ReserveStock(...) producing (partial) reservation
│   └── CommandHandlers/ReserveStockCommandHandler.cs
└── BrewUp.Warehouse.ReadModel/EventHandlers/StockReservedEventHandler.cs (+ rejected)

src/Sagas/
├── BrewUp.Sagas.Domain/
│   ├── Entities/SalesOrderSaga.cs               # ADD MarkPaymentAuthorized/MarkStockReserved + ReadyToConfirm gate
│   └── Orchestrators/SalesOrderSagaOrchestrator.cs  # ADD handlers for the 4 new integration events + request dispatch
├── BrewUp.Sagas.SharedKernel/Messages/Events/SagaSalesOrderReadyToConfirm.cs   # gate domain event
└── BrewUp.Sagas.ReadModel/EventHandlers/SagaSalesOrderReadyToConfirmEventHandler.cs  # publishes integration event

src/BrewUp.Shared/Messages/Events/Sagas/         # saga-facing integration events (…IntegrationEvent suffix convention)
├── PaymentAuthorizedIntegrationEvent.cs
├── PaymentDeclinedIntegrationEvent.cs
├── StockReservedIntegrationEvent.cs
├── StockReservationRejectedIntegrationEvent.cs
└── SagaSalesOrderReadyToConfirmIntegrationEvent.cs

src/BrewUp.Rest/Module/PaymentModule.cs           # IModule registration (AR-013)
```

**Structure Decision**: Modular monolith. One new module (`Payment`) created with the full standard 6-project structure under `src/Payment/`; `Warehouse`, `Sales`, and `Sagas` extended in place; saga-facing integration events placed in `BrewUp.Shared` following the existing `…IntegrationEvent` convention. All new projects are added to `src/BrewUp.slnx` and Payment is registered via `PaymentModule.cs`.

### Dependency direction (AR-015 — enforced by fitness tests)

```text
Payment.Facade   → Payment.Domain, Payment.ReadModel, Payment.Infrastructure, Payment.SharedKernel, BrewUp.Shared
Payment.Domain   → Payment.SharedKernel, BrewUp.Shared
Sales (stores refs) uses its OWN PaymentAuthorizationReference / StockReservationReference value objects;
  cross-context evidence (raw id strings) arrives via Shared integration events — Sales does NOT reference Payment.Domain / Warehouse.Domain.
Sagas.Domain     → Sagas.SharedKernel, BrewUp.Shared (consumes integration events; dispatches commands via published SharedKernel contracts)
```

Forbidden (verified by architecture tests): `Sales → Payment.Domain`, `Sales → Warehouse.Domain`, any `Domain → Infrastructure/ReadModel/Facade`, any module embedding another's aggregate.

## Complexity Tracking

> No Constitution Check violations. No entries required.
