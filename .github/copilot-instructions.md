# BrewUp — Copilot Instructions

<!-- SPECKIT START -->
## Active Spec Kit Feature

- **001-order-confirmation** — Sales Order Confirmation. Plan: [specs/001-order-confirmation/plan.md](../specs/001-order-confirmation/plan.md) · Spec: [specs/001-order-confirmation/spec.md](../specs/001-order-confirmation/spec.md)
<!-- SPECKIT END -->

## Solution Overview

BrewUp is a **DDD Modular Monolith** built on **.NET 10 / C#** using CQRS and Event Sourcing.
The framework is **Muflone** (`AggregateRoot`, `CommandHandlerAsync<T>`, `IntegrationEvent`, `DomainEvent`).
The message bus is **RabbitMQ** (via `Muflone.Transport.RabbitMQ`).
The write store is **EventStore** (gRPC); the read store is **MongoDB**.

## Spec-Driven Development Context

When working inside a Spec Kit flow, treat the specification, constitution, domain carrier, and architecture memory as durable context.

The model must not silently invent missing domain or architecture decisions.

If the feature introduces a new bounded context or domain authority, the implementation plan must decide explicitly whether that authority is:

1. an existing module;
2. a new BrewUp module to create;
3. an external system outside the repository;
4. out of scope for the current feature.

For the active feature, **Payment is a separate authority**.

If Payment behavior is implemented in this repository, Payment must be created as a full BrewUp module following the standard module structure.

### Key Rules

- **Never** put business logic in facades.
- **Never** put domain decisions in command handlers. Command handlers load aggregates, call aggregate methods, and save.
- **Never** reference another module's `Domain` or `Infrastructure` project.
- **Never** implement a bounded context as loose folders inside another module.
- **Never** introduce ad-hoc project names or ad-hoc folder structures.
- **Never** place Payment behavior inside Sales.
- **Never** place Warehouse stock mutation inside Sales.
- **Never** place module-specific commands or domain events in the wrong module.
- **Strongly-typed IDs** always derive from `Muflone.Core.DomainId(string value)`.
- **Enumeration pattern** (Smart Enum) for status/type values — derive from `BrewUp.Shared.Helpers.Enumeration`.
- **Test-First**: write a failing `CommandSpecification<T>` (Given/When/Expect) before implementing aggregate logic.
- Use `Guid.CreateVersion7()` everywhere, never `Guid.NewGuid()`.
- Use `ConfigureAwait(false)` on every `await` in non-test code.
---

## Architecture Rules

These architecture rules are authoritative for generated plans, tasks, and code.

They complement the bounded-context rules in the feature specification.

Use these rule IDs when reporting architectural drift.

### AR-000 — Architecture rules are authoritative

The rules in this section must constrain specifications, plans, tasks, source code, tests, project structure, and module registration.

A generated artifact that violates these rules is architecturally misaligned.

Do not override these rules to make an implementation look simpler or more complete.

### AR-001 — Bounded contexts must be explicit modules

Every bounded context or business authority implemented in BrewUp must be represented as an explicit module.

A module requires:

- a solution folder;
- a physical directory under `src/<ModuleName>/`;
- a standard set of projects.

Do not implement a new bounded context as:

- a folder inside another module;
- a namespace inside another module;
- a set of classes inside `BrewUp.Shared`;
- a set of classes inside `BrewUp.Rest`;
- a set of classes inside the Sagas module.

If a feature introduces `Payment` as a separate authority and implementation is in scope, the plan must create a `Payment` module.

### AR-002 — Every business module uses the standard project structure

Every BrewUp business module must follow this project structure:

```text
src/<ModuleName>/
├── BrewUp.<ModuleName>.SharedKernel/
├── BrewUp.<ModuleName>.Domain/
├── BrewUp.<ModuleName>.ReadModel/
├── BrewUp.<ModuleName>.Infrastructure/
├── BrewUp.<ModuleName>.Facade/
└── BrewUp.<ModuleName>.Tests/
```

The projects must be added to the solution.

The projects must use the exact naming format:

```text
BrewUp.<ModuleName>.SharedKernel
BrewUp.<ModuleName>.Domain
BrewUp.<ModuleName>.ReadModel
BrewUp.<ModuleName>.Infrastructure
BrewUp.<ModuleName>.Facade
BrewUp.<ModuleName>.Tests
```

Do not use alternative names such as:

```text
BrewUp.<ModuleName>.Api
BrewUp.<ModuleName>.Application
BrewUp.<ModuleName>.Core
BrewUp.<ModuleName>.Contracts
BrewUp.<ModuleName>.Queries
```

unless the existing repository has already established that exact convention for the target module.

### AR-003 — SharedKernel owns module contracts

Each module owns its own `SharedKernel` project.

Module-specific IDs, commands, domain events, enums, and public message contracts belong in that module's `SharedKernel`.

For a new module, prefer this structure:

```text
BrewUp.<ModuleName>.SharedKernel/
├── DomainIds/
├── CustomTypes/
├── Enums/
└── Messages/
    ├── Commands/
    └── Events/
```

For the Payment module:

```text
BrewUp.Payment.SharedKernel/
├── DomainIds/
│   └── PaymentAuthorizationId.cs
├── Messages/
│   ├── Commands/
│   │   └── AuthorizePayment.cs
│   └── Events/
│       ├── PaymentAuthorized.cs
│       ├── PaymentDeclined.cs
│       └── PaymentOutcomeUnknown.cs
└── Enums/
```

Do not put Payment-specific IDs, commands, or domain events in Sales, Warehouse, Sagas, or `BrewUp.Shared`.

Use `BrewUp.Shared` only for truly cross-context shared primitives, helpers, external DTOs, and integration contracts that are intentionally global.

### AR-004 — Domain owns command handlers and business behavior

The `Domain` project contains aggregates, aggregate entities, command handlers, domain services when needed, domain behavior, and domain helper registration.

Command handlers must be thin:

1. load the aggregate;
2. call an aggregate method;
3. save the aggregate.

Business decisions belong in aggregate methods, not in command handlers.

### AR-005 — Facade is the module entry point

The `Facade` project is the public entry point of a module.

It contains REST endpoints, ACL handlers, module facade services, and facade helper registration.

Other modules must communicate with a module through one of these boundaries:

- the module Facade;
- explicit commands;
- integration events;
- shared message contracts;
- the Saga module if orchestration is required.

Other modules must not access another module's `Domain` project directly.

### AR-006 — ReadModel owns queries

The `ReadModel` project contains MongoDB projections, read-side documents, query services, query handlers, and domain event handlers that update read models.

Queries must not be implemented in the `Domain` project.

Facades may expose query endpoints, but query behavior belongs to `ReadModel`.

### AR-007 — Infrastructure owns persistence wiring

The `Infrastructure` project contains module-specific persistence wiring.

It owns EventStore persister registration, MongoDB collection wiring, and infrastructure helper registration.

Business logic must not be placed in Infrastructure.

### AR-008 — REST host only registers modules

`BrewUp.Rest` is the ASP.NET Core host.

It must only register modules through the `IModule` pattern, map module endpoints, and host the application.

It must not contain business logic.

Every module must have its own `<ModuleName>Module.cs` under:

```text
BrewUp.Rest/Module/
```

For Payment:

```text
BrewUp.Rest/Module/PaymentModule.cs
```

The module must call:

```csharp
builder.Services.AddPaymentFacade(builder.Configuration);
```

and map:

```csharp
endpoints.MapPaymentEndpoints();
```

if Payment exposes endpoints.

### AR-009 — Solution structure must mirror module ownership

The physical solution structure must reflect the domain model.

If a feature says `Payment` owns payment authorization, then Payment implementation must live under:

```text
src/Payment/
```

not under:

```text
src/Sales/
src/Sagas/
src/BrewUp.Shared/
src/BrewUp.Rest/
```

The solution structure must make ownership visible.

### AR-010 — Sagas coordinate; they do not own domain decisions

The Sagas module may coordinate cross-context processes.

It may collect evidence from multiple contexts, react to integration events, and dispatch commands.

It must not own Payment authorization decisions.

It must not own Warehouse stock decisions.

It must not contain Sales, Payment, or Warehouse aggregate behavior.

A saga may coordinate.

It must not steal authority.

### AR-011 — Architecture drift must be reported using rule IDs

When a generated artifact violates these rules, report the violation using the architecture rule ID.

Examples:

```text
The plan violates AR-001 and AR-002 because Payment is a separate authority but no Payment module is created.
```

```text
The task list violates AR-003 because Payment commands are placed in Sales.SharedKernel.
```

```text
The implementation violates AR-005 because Sales references Payment.Domain directly.
```


---

## Solution Structure

```
src/
├── BrewUp.Shared/               # Cross-context contracts, shared types, helpers
├── BrewUp.Shared.Tests/         # Architecture and shared-kernel tests
├── BrewUp.Infrastructure/       # Global EventStore + RabbitMQ + MongoDB wiring
├── BrewUp.Rest/                 # ASP.NET Core host — module registration only
├── BrewUp.Rest.Tests/           # Integration / architecture tests for the host
│
├── Sales/                       # Sales bounded context
├── Warehouse/                   # Warehouse bounded context
├── Payment/                     # Payment bounded context, if implemented by the current feature
├── Purchases/                   # Purchases bounded context
├── Sagas/                       # Cross-context orchestration
├── MasterData/                  # Master data: customers, beers
└── Dashboards/                  # Read-only reporting / dashboards
```

---

## Module Layer Pattern

Every bounded context follows this layer structure.

Existing modules include `Sales`, `Warehouse`, `Purchases`, `Sagas`, `MasterData`, and `Dashboards`.

If `Payment` is implemented by the current feature, it must follow the same structure.

| Layer | Project | Responsibilities |
|---|---|---|
| **SharedKernel** | `BrewUp.<Module>.SharedKernel` | Module contracts: Domain IDs, commands, domain events, enums, custom types |
| **Domain** | `BrewUp.<Module>.Domain` | Aggregate roots, command handlers, domain services, business behavior, `<Module>DomainHelper.cs` |
| **ReadModel** | `BrewUp.<Module>.ReadModel` | MongoDB projections, read-side documents, queries, `<Module>ReadModelHelper.cs` |
| **Infrastructure** | `BrewUp.<Module>.Infrastructure` | EventStore persister, MongoDB collections, `<Module>InfrastructureHelper.cs` |
| **Facade** | `BrewUp.<Module>.Facade` | REST endpoints, ACL integration-event handlers, module entry point, `<Module>FacadeHelper.cs` |
| **Tests** | `BrewUp.<Module>.Tests` | `CommandSpecification<T>` aggregate tests, architecture tests |

> **Entities** project (`BrewUp.<Module>.Entities`) exists only in `Warehouse`, `MasterData`, `Dashboards` for value objects shared inside that BC.

---

## New Module Creation Checklist

When a feature introduces a new BrewUp module, create the full module structure before adding business behavior.

For a new module `<ModuleName>`:

```text
src/<ModuleName>/
├── BrewUp.<ModuleName>.SharedKernel/
│   ├── DomainIds/
│   ├── CustomTypes/
│   ├── Enums/
│   └── Messages/
│       ├── Commands/
│       └── Events/
│
├── BrewUp.<ModuleName>.Domain/
│   ├── Entities/
│   ├── CommandHandlers/
│   └── <ModuleName>DomainHelper.cs
│
├── BrewUp.<ModuleName>.ReadModel/
│   ├── EventHandlers/
│   ├── Queries/
│   ├── Dtos/
│   └── <ModuleName>ReadModelHelper.cs
│
├── BrewUp.<ModuleName>.Infrastructure/
│   └── <ModuleName>InfrastructureHelper.cs
│
├── BrewUp.<ModuleName>.Facade/
│   ├── Acl/
│   ├── Endpoints/
│   ├── I<ModuleName>Facade.cs
│   ├── <ModuleName>Facade.cs
│   └── <ModuleName>FacadeHelper.cs
│
└── BrewUp.<ModuleName>.Tests/
    ├── Architecture/
    └── Domain/
```

Also:

1. add all projects to `src/BrewUp.slnx`;
2. add project references following the dependency rules;
3. add `<ModuleName>Module.cs` under `BrewUp.Rest/Module/`;
4. register the module through the `IModule` pattern;
5. add architecture tests for dependency rules;
6. add command specifications before implementing aggregate behavior.

---

## Project Dependency Rules

Allowed dependency direction inside a module:

```text
Facade
  ├── Domain
  ├── ReadModel
  ├── Infrastructure
  └── SharedKernel

Domain
  └── SharedKernel

ReadModel
  └── SharedKernel

Infrastructure
  ├── Domain
  └── SharedKernel

Tests
  ├── Domain
  ├── Facade
  ├── ReadModel
  ├── Infrastructure
  └── SharedKernel
```

Forbidden dependencies:

```text
Any module -> AnotherModule.Domain
Any module -> AnotherModule.Infrastructure
Any module -> AnotherModule internal implementation types
BrewUp.Rest -> business logic
BrewUp.Shared -> module-specific behavior
```

Cross-context communication must use explicit contracts:

- commands;
- domain events;
- integration events;
- ACL handlers;
- facades;
- saga orchestration when appropriate.

---

## BrewUp.Shared (Cross-context contracts)

```
BrewUp.Shared/
├── DomainIds/              # Strongly-typed IDs shared across BCs (CustomerId, WarehouseId, …)
├── CustomTypes/            # Value objects shared across BCs (ItemRequested, …)
├── ExternalContracts/      # JSON DTOs used in integration events (CustomerJson, SalesOrderRowJson, …)
│   ├── MasterData/
│   └── Sales/
├── Messages/
│   └── Events/
│       └── Sagas/          # All integration events consumed/emitted by the Saga orchestrator
├── Helpers/                # Enumeration base class
├── Validation/             # FluentValidation base types
└── SharedHelper.cs         # DI extension: services.AddShared()
```

New cross-context IDs → `BrewUp.Shared/DomainIds/<Name>Id.cs`:
```csharp
public sealed class FooId(string value) : DomainId(value);
```

New integration events → `BrewUp.Shared/Messages/Events/Sagas/<EventName>.cs`:
```csharp
public sealed class FooHappened(IntegrationId aggregateId, Guid correlationId,
    string somePayload) : IntegrationEvent(aggregateId, correlationId)
{
    public string SomePayload { get; private set; } = somePayload;
}
```

Use `BrewUp.Shared` only when a concept is intentionally global.

Do not place module-specific IDs, commands, or domain events in `BrewUp.Shared` merely because another module needs to observe them.

If a contract belongs to a specific module, keep it in that module's `SharedKernel`.

---

## Payment Module

Payment is introduced as a separate authority for the Sales Order Confirmation feature.

If Payment is implemented in this repository, it must be a full module.

```
src/Payment/
├── BrewUp.Payment.SharedKernel/
│   ├── DomainIds/
│   │   └── PaymentAuthorizationId.cs
│   ├── Enums/
│   └── Messages/
│       ├── Commands/
│       │   └── AuthorizePayment.cs
│       └── Events/
│           ├── PaymentAuthorized.cs
│           ├── PaymentDeclined.cs
│           └── PaymentOutcomeUnknown.cs
│
├── BrewUp.Payment.Domain/
│   ├── Entities/
│   │   └── PaymentAuthorization.cs
│   ├── CommandHandlers/
│   └── PaymentDomainHelper.cs
│
├── BrewUp.Payment.ReadModel/
│   ├── EventHandlers/
│   ├── Queries/
│   ├── Dtos/
│   └── PaymentReadModelHelper.cs
│
├── BrewUp.Payment.Infrastructure/
│   └── PaymentInfrastructureHelper.cs
│
├── BrewUp.Payment.Facade/
│   ├── Acl/
│   ├── Endpoints/
│   ├── IPaymentFacade.cs
│   ├── PaymentFacade.cs
│   └── PaymentFacadeHelper.cs
│
└── BrewUp.Payment.Tests/
    ├── Architecture/
    └── Domain/
```

Payment owns:

- payment authorization request;
- payment authorization outcome;
- provider transaction reference;
- payment-provider timeout interpretation;
- declined, pending, unknown, and authorized semantics;
- void and refund behavior, unless explicitly assigned elsewhere by a domain decision.

Sales must not own these decisions.

Warehouse must not own these decisions.

Sagas may coordinate Payment commands and react to Payment events, but Sagas do not own Payment decisions.

Payment commands belong in:

```text
BrewUp.Payment.SharedKernel/Messages/Commands/
```

Payment domain events belong in:

```text
BrewUp.Payment.SharedKernel/Messages/Events/
```

Payment strongly-typed IDs belong in:

```text
BrewUp.Payment.SharedKernel/DomainIds/
```

The Payment Facade is the module entry point.

Payment queries belong in `BrewUp.Payment.ReadModel`.

---

## Sales Module

```
src/Sales/
├── BrewUp.Sales.SharedKernel/
│   ├── CustomTypes/        # SalesOrderId, SalesOrderNumber, SalesOrderDate, Customer, …
│   ├── Enums/              # SalesOrderStatus (Enumeration pattern)
│   └── Messages/
│       ├── Commands/       # CreateSalesOrder, PlaceSalesOrder, AcceptSalesOrder, ConfirmSalesOrder, …
│       └── Events/         # SalesOrderCreated, SalesOrderPlaced, SalesOrderAccepted, SalesOrderConfirmed, …
│
├── BrewUp.Sales.Domain/
│   ├── Entities/
│   │   └── SalesOrder.cs   # Aggregate root — all business logic lives here
│   ├── CommandHandlers/    # One handler per command; load aggregate → call method → save
│   ├── DomainHelper.cs     # services.AddCommandHandler<…>() registrations
│   ├── ISalesDomainService.cs / SalesDomainService.cs
│   └── Mappers/
│
├── BrewUp.Sales.ReadModel/
│   ├── EventHandlers/      # DomainEventHandlerAsync<T> → MongoDB upserts / publishes integration events
│   ├── Queries/            # MongoDB query services
│   ├── Dtos/               # Read model documents
│   └── SalesReadModelHelper.cs  # services.AddDomainEventHandler<…>() registrations
│
├── BrewUp.Sales.Infrastructure/
│   └── InfrastructureHelper.cs  # EventStore persister + MongoDB collections
│
├── BrewUp.Sales.Facade/
│   ├── Acl/                # IntegrationEventHandlerAsync<T> → dispatch Sales commands
│   ├── Endpoints/          # Minimal API endpoint maps
│   ├── ISalesFacade.cs / SalesFacade.cs
│   └── SalesFacadeHelper.cs  # Wires Domain + ReadModel + Infrastructure + ACL handlers
│
└── BrewUp.Sales.Tests/
    ├── Architecture/       # NetArchTest fitness constraints
    └── Domain/             # CommandSpecification<T> aggregate specs (Given/When/Expect)
```

### Sales ownership in the Order Confirmation feature

Sales owns:

- Sales Order lifecycle;
- commercial status;
- customer demand;
- the transition to `Confirmed` when required external evidence is present.

Sales may store:

- `PaymentAuthorizationId`;
- `StockReservationId`.

Sales must not:

- authorize payment;
- interpret payment-provider timeouts;
- void or refund payment;
- reserve stock;
- release stock;
- decrement warehouse stock;
- create shipments;
- issue invoices.


**Aggregate method pattern** (`SalesOrder.cs`):
```csharp
internal void AcceptOrder(Guid correlationId)
{
    RaiseEvent(new SalesOrderAccepted(new SalesOrderId(Id.Value), correlationId));
}

private void Apply(SalesOrderAccepted @event)
{
    _salesOrderStatus = SalesOrderStatus.Accepted;
}
```

**Command handler pattern**:
```csharp
public sealed class AcceptSalesOrderCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<AcceptSalesOrder>(repository, loggerFactory)
{
    public override async Task HandleAsync(AcceptSalesOrder command, CancellationToken cancellationToken = new())
    {
        var aggregate = await Repository.GetByIdAsync<SalesOrder>(command.AggregateId, cancellationToken)
            .ConfigureAwait(false);
        aggregate!.AcceptOrder(command.MessageId);
        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
}
```

**Test pattern** (`CommandSpecification<T>`):
```csharp
public sealed class AcceptSalesOrderSuccessfully : CommandSpecification<AcceptSalesOrder>
{
    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_id, _number, _date, _customer, _delivery, _rows, _correlationId);
    }
    protected override AcceptSalesOrder When() => new(_id, _correlationId);
    protected override ICommandHandlerAsync<AcceptSalesOrder> OnHandler() =>
        new AcceptSalesOrderCommandHandler(Repository, new NullLoggerFactory());
    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new SalesOrderAccepted(_id, _correlationId);
    }
}
```

---

## Warehouse Module

```
src/Warehouse/
├── BrewUp.Warehouse.SharedKernel/
│   ├── CustomTypes/        # ShipmentId, SalesOrderId (local record), DeliveryDate, …
│   ├── Enums/              # ShipmentState
│   └── Messages/
│       ├── Commands/       # CreateAvailability, AddItemStock, RequestBeersAvailability, ReserveStock, …
│       └── Events/         # AvailabilityCreated, ItemStockAdded, RequestAccepted, RequestRejected, …
│
├── BrewUp.Warehouse.Entities/
│   └── …                   # Value objects shared inside the Warehouse BC
│
├── BrewUp.Warehouse.Domain/
│   ├── Entities/
│   │   ├── Availability.cs # Aggregate — availability checks and stock management
│   │   └── Shipment.cs     # Aggregate — shipment preparation
│   ├── CommandHandlers/
│   └── DomainHelper.cs
│
├── BrewUp.Warehouse.ReadModel/
├── BrewUp.Warehouse.Infrastructure/
├── BrewUp.Warehouse.Facade/
│   ├── Endpoints/
│   └── WarehouseFacadeHelper.cs
└── BrewUp.Warehouse.Tests/
    ├── Architecture/
    └── Domain/
```

---

## Purchases Module

```
src/Purchases/
├── BrewUp.Purchases.SharedKernel/
│   ├── CustomTypes/
│   ├── Enums/
│   └── Messages/
│       ├── Commands/
│       └── Events/
│
├── BrewUp.Purchases.Domain/
│   ├── Entities/
│   ├── CommandHandlers/
│   └── DomainHelper.cs
│
├── BrewUp.Purchases.ReadModel/
├── BrewUp.Purchases.Infrastructure/
├── BrewUp.Purchases.Facade/
│   ├── Endpoints/
│   └── PurchasesFacadeHelper.cs
└── BrewUp.Purchases.Tests/
    ├── Architecture/
    └── Domain/
```

---

## Sagas Module

```
src/Sagas/
├── BrewUp.Sagas.SharedKernel/
│   ├── CustomTypes/        # SagaId
│   ├── Enums/              # SagaState
│   └── Messages/
│       ├── Commands/       # StartSalesOrderSaga, RejectSalesOrder, …
│       └── Events/         # SalesOrderSagaStarted, SagaCustomerBudgetVerified, SagaSalesOrderPlaced, …
│
├── BrewUp.Sagas.Domain/
│   ├── Entities/
│   │   └── SalesOrderSaga.cs   # Saga aggregate — collects evidence, raises ReadyToConfirm
│   ├── Orchestrators/
│   │   ├── ISalesOrderSagaOrchestrator.cs
│   │   └── SalesOrderSagaOrchestrator.cs  # implements ISagaStartedByAsync + IIntegrationEventHandlerAsync<T>
│   └── SagasDomainHelper.cs
│
├── BrewUp.Sagas.ReadModel/
│   ├── EventHandlers/      # DomainEventHandler → publish integration events to other BCs
│   └── SagaReadModelHelper.cs
│
├── BrewUp.Sagas.Infrastructure/
│   └── Hubs/               # SignalR hub for real-time saga state notifications
│
├── BrewUp.Sagas.Facade/
│   ├── Endpoints/
│   └── SagasFacadeHelper.cs
└── BrewUp.Sagas.Tests/
    ├── Architecture/
    └── Orchestrators/
```

**Saga orchestrator pattern** — implement `IIntegrationEventHandlerAsync<T>` for each incoming event; call aggregate `Mark*` methods; aggregate raises an evidence-gate event when all conditions are met:
```csharp
public async Task HandleAsync(PaymentAuthorized @event, CancellationToken cancellationToken = new())
{
    var correlationId = MessageHelpers.GetCorrelationId(@event);
    var aggregate = await repository
        .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
        .ConfigureAwait(false);
    aggregate!.MarkPaymentAuthorized(@event.PaymentAuthorizationId, correlationId);
    await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
}
```

---

## MasterData Module

```
src/MasterData/
├── BrewUp.MasterData.SharedKernel/
│   ├── CustomTypes/
│   ├── Enums/
│   └── Messages/
│       ├── Commands/       # CreateCustomer, UpdateCustomer, CreateBeer, …
│       └── Events/         # CustomerCreated, CustomerUpdated, BeerCreated, …
│
├── BrewUp.MasterData.Entities/
│   └── …                   # Value objects shared inside the MasterData BC
│
├── BrewUp.MasterData.Domain/
│   ├── Entities/
│   ├── CommandHandlers/
│   └── DomainHelper.cs
│
├── BrewUp.MasterData.ReadModel/
├── BrewUp.MasterData.Infrastructure/
├── BrewUp.MasterData.Facade/
│   ├── Endpoints/
│   └── MasterDataFacadeHelper.cs
└── BrewUp.MasterData.Tests/
    ├── Architecture/
    └── Domain/
```

---

## Dashboards Module

```
src/Dashboards/
├── BrewUp.Dashboards.SharedKernel/
│   ├── CustomTypes/
│   └── Messages/
│       └── Events/
│
├── BrewUp.Dashboards.Entities/
├── BrewUp.Dashboards.Domain/
├── BrewUp.Dashboards.ReadModel/  # Primarily read aggregations from MongoDB
├── BrewUp.Dashboards.Infrastructure/
├── BrewUp.Dashboards.Facade/
│   ├── Endpoints/
│   └── DashboardsFacadeHelper.cs
└── BrewUp.Dashboards.Tests/
    └── Architecture/
```

---

## REST Host (`BrewUp.Rest`)

All modules are wired via the `IModule` pattern:

```csharp
// BrewUp.Rest/Module/SalesModule.cs
public class SalesModule : IModule
{
    public bool IsEnabled => true;
    public int Order => 0;
    public IServiceCollection Register(WebApplicationBuilder builder)
    {
        builder.Services.AddSalesFacade(builder.Configuration);
        return builder.Services;
    }
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSalesEndpoints();
        return endpoints;
    }
}
```

Each module has its own `<Module>Module.cs` in `BrewUp.Rest/Module/`.

---

## Infrastructure (`BrewUp.Infrastructure`)

Shared global infrastructure wiring:
- **EventStore** (gRPC) — write model via `Muflone.Eventstore.gRPC`
- **RabbitMQ** — message transport via `Muflone.Transport.RabbitMQ`
- **MongoDB** — read model via `MongoDB.Driver`

Configuration sections in `appsettings.json`:
```json
{
  "BrewUp": {
    "EventStoreSettings": { "ConnectionString": "esdb://..." },
    "MongoDbSettings": { "ConnectionString": "mongodb://...", "DatabaseName": "BrewUp" },
    "RabbitMQ": { "Host": "localhost", "Username": "guest", "Password": "guest" }
  }
}
```

---

## Cross-context Communication Flow

```
[External Event arrives via RabbitMQ]
        │
        ▼
[Facade ACL Handler: IntegrationEventHandlerAsync<T>]
        │ sends command via IServiceBus
        ▼
[CommandHandler: loads Aggregate, calls method, saves]
        │ RaiseEvent(new DomainEvent)
        ▼
[ReadModel EventHandler: DomainEventHandlerAsync<T>]
        │ publishes IntegrationEvent via IEventBus
        ▼
[Other BCs subscribe via RabbitMQ]
```

For **saga coordination**: the saga orchestrator (`IIntegrationEventHandlerAsync<T>`) collects evidence in the saga aggregate. When all conditions are met the aggregate raises a gate event (e.g., `SagaSalesOrderReadyToConfirm`). The ReadModel event handler publishes a cross-context integration event. The target BC's ACL handler dispatches the final command.

---

## Shell Commands

```powershell
# Build entire solution
dotnet build src/BrewUp.slnx

# Run all tests
dotnet test src/BrewUp.slnx

# Run tests for a single module
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj

# Run Payment tests, if the Payment module exists
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj

# Run the REST API locally
dotnet run --project src/BrewUp.Rest/BrewUp.Rest.csproj
```

