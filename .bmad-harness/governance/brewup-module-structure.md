# BrewUp Module Structure Governance

## Numbered rules

### AR-000 — Architecture memory is authoritative

The rules in this file are authoritative for the physical structure of BrewUp modules.

They must constrain:

- specifications;
- clarification outcomes;
- implementation plans;
- generated tasks;
- source code;
- tests;
- architecture fitness functions.

Generated artifacts must not invent ad-hoc project structures when this file defines the expected one.

If an artifact conflicts with this file, the artifact is architecturally misaligned.

---

### AR-001 — Bounded contexts must be explicit modules

Every bounded context or business authority that is in scope for implementation must be represented as an explicit module under:

```text
src/<ModuleName>/
```

A business authority must not be implemented as loose folders or classes inside another module.

If a specification introduces `Payment` as a separate authority and Payment behavior is in scope, the implementation plan must create:

```text
src/Payment/
```

and a corresponding solution folder.

---

### AR-002 — Standard module project structure

Every BrewUp business module must follow the standard module structure.

For a module named `<ModuleName>`, create:

```text
src/<ModuleName>/
├── BrewUp.<ModuleName>.SharedKernel/
├── BrewUp.<ModuleName>.Domain/
├── BrewUp.<ModuleName>.ReadModel/
├── BrewUp.<ModuleName>.Infrastructure/
├── BrewUp.<ModuleName>.Facade/
└── BrewUp.<ModuleName>.Tests/
```

The project names must follow this exact format:

```text
BrewUp.<ModuleName>.SharedKernel
BrewUp.<ModuleName>.Domain
BrewUp.<ModuleName>.ReadModel
BrewUp.<ModuleName>.Infrastructure
BrewUp.<ModuleName>.Facade
BrewUp.<ModuleName>.Tests
```

Do not create alternative names such as:

```text
BrewUp.<ModuleName>.Application
BrewUp.<ModuleName>.Api
BrewUp.<ModuleName>.Contracts
BrewUp.<ModuleName>.Core
BrewUp.<ModuleName>.Persistence
```

unless an explicit architecture decision changes the standard structure.

---

### AR-003 — Optional Entities project

Some existing modules have an additional `Entities` project.

Examples:

```text
BrewUp.Warehouse.Entities
BrewUp.MasterData.Entities
BrewUp.Dashboards.Entities
```

An `Entities` project is optional.

It may be introduced only when a module needs value objects or internal shared structures that are used inside that bounded context but should not belong directly to the Domain project.

Do not create an `Entities` project by default for every new module.

If the generated plan creates `BrewUp.<ModuleName>.Entities`, it must explain why the module needs it.

---

### AR-004 — SharedKernel owns module contracts

Each module has its own `SharedKernel`.

The module `SharedKernel` contains the contracts and shared types owned by that module.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.SharedKernel/
├── DomainIds/
├── CustomTypes/
├── Enums/
└── Messages/
    ├── Commands/
    └── Events/
```

The exact folders should be created when needed.

Do not create empty folders unless the repository convention requires them.

---

### AR-005 — Domain IDs live in module SharedKernel

Module-specific strongly typed IDs must live under:

```text
BrewUp.<ModuleName>.SharedKernel/DomainIds/
```

For Payment, IDs such as these belong in:

```text
src/Payment/BrewUp.Payment.SharedKernel/DomainIds/
```

Examples:

```text
PaymentAuthorizationId
PaymentId
PaymentTermsApprovalId
```

Strongly typed IDs must derive from:

```csharp
Muflone.Core.DomainId
```

Example:

```csharp
public sealed class PaymentAuthorizationId(string value) : DomainId(value);
```

Do not use raw `Guid`, `string`, or primitive identifiers across module boundaries.

---

### AR-006 — Commands live in module SharedKernel messages

Commands owned by a module must live under:

```text
BrewUp.<ModuleName>.SharedKernel/Messages/Commands/
```

For Payment, commands such as these belong in:

```text
src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/
```

Examples:

```text
RequestPaymentAuthorization
VoidPaymentAuthorization
RequestPaymentRefund
ApprovePaymentTerms
```

Only create commands that are explicitly required by the specification, plan, or clarified domain decisions.

Do not invent refund, void, retry, timeout, or payment-terms commands unless the feature explicitly needs them.

---

### AR-007 — Domain events live in module SharedKernel messages

Domain events owned by a module must live under:

```text
BrewUp.<ModuleName>.SharedKernel/Messages/Events/
```

For Payment, events such as these belong in:

```text
src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/
```

Examples:

```text
PaymentAuthorizationRequested
PaymentAuthorized
PaymentDeclined
PaymentOutcomeUnknown
PaymentAuthorizationVoided
PaymentRefundIssued
PaymentTermsApproved
```

Only create events that correspond to explicit domain outcomes.

Do not create events that silently decide unresolved business policies.

---

### AR-008 — Domain project owns business behavior

The `Domain` project contains:

- aggregates;
- entities owned by the module;
- value objects when they belong directly to the domain model;
- command handlers;
- domain services;
- domain behavior;
- domain helper registration.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.Domain/
├── Entities/
├── CommandHandlers/
├── DomainHelper.cs
└── ...
```

Business rules must live in aggregate methods or domain services.

Command handlers orchestrate command execution:

```text
load aggregate → call domain method → save aggregate
```

Command handlers must not contain business logic.

Facade and endpoint code must not contain business logic.

---

### AR-009 — ReadModel project owns queries and projections

The `ReadModel` project contains:

- read model documents;
- query services;
- domain event handlers that update projections;
- read model helper registration.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.ReadModel/
├── EventHandlers/
├── Queries/
├── Dtos/
└── <ModuleName>ReadModelHelper.cs
```

Queries belong in the `ReadModel` project.

Queries must not be implemented in the `Domain` project.

Read models may be denormalized and rebuilt from events.

---

### AR-010 — Infrastructure project owns persistence and technical wiring

The `Infrastructure` project contains module-specific infrastructure concerns.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.Infrastructure/
└── InfrastructureHelper.cs
```

Infrastructure responsibilities include:

- EventStore persister wiring;
- MongoDB collection setup;
- technical adapters;
- infrastructure configuration;
- infrastructure helper registration.

The Domain project must not reference EventStore, MongoDB, RabbitMQ, ASP.NET Core, or any infrastructure framework.

---

### AR-011 — Facade project is the module entry point

The `Facade` project is the public entry point of a module.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.Facade/
├── Acl/
├── Endpoints/
├── I<ModuleName>Facade.cs
├── <ModuleName>Facade.cs
└── <ModuleName>FacadeHelper.cs
```

The Facade project owns:

- Minimal API endpoint maps;
- ACL integration-event handlers;
- public module facade;
- module service registration through `<ModuleName>FacadeHelper.cs`.

Other modules must not call another module's Domain project directly.

Cross-module interaction must happen through:

- shared messages;
- ACL handlers;
- integration events;
- the module facade;
- explicit integration contracts.

---

### AR-012 — Tests project is mandatory

Every implemented module must have a Tests project.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.Tests/
├── Architecture/
└── Domain/
```

The Tests project must include:

- architecture tests;
- command specifications for domain behavior;
- tests for aggregate invariants;
- tests for command handlers when behavior is introduced;
- integration or boundary tests when relevant.

New behavioral changes must start with failing tests.

---

### AR-013 — REST host must register modules explicitly

Every implemented module must be registered in the REST host using the `IModule` pattern.

For a module named `<ModuleName>`, create or update:

```text
src/BrewUp.Rest/Module/<ModuleName>Module.cs
```

Example structure:

```csharp
public class PaymentModule : IModule
{
    public bool IsEnabled => true;
    public int Order => 0;

    public IServiceCollection Register(WebApplicationBuilder builder)
    {
        builder.Services.AddPaymentFacade(builder.Configuration);
        return builder.Services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPaymentEndpoints();
        return endpoints;
    }
}
```

The REST host must only compose modules.

It must not contain business logic.

---

### AR-014 — Solution file must include all module projects

When a new module is created, all module projects must be added to the solution.

For Payment, the solution must include:

```text
src/Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj
src/Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj
src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj
src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj
src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj
src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
```

The module must not exist only as files outside the solution.

---

### AR-015 — Project references must preserve layer boundaries

Project references must preserve the modular architecture.

General allowed direction:

```text
Facade         → Domain, ReadModel, Infrastructure, SharedKernel
Domain         → SharedKernel
ReadModel      → SharedKernel
Infrastructure → Domain, SharedKernel
Tests          → Domain, SharedKernel, Facade, ReadModel, Infrastructure as needed
```

Forbidden references:

```text
Domain         → Infrastructure
Domain         → ReadModel
Domain         → Facade
Domain         → ASP.NET Core
Domain         → MongoDB
Domain         → RabbitMQ
Domain         → EventStore
Module A       → Module B Domain
Module A       → Module B Infrastructure
```

Cross-module behavior must use explicit contracts and integration boundaries.

---

### AR-016 — Business authority must not be collapsed into another module

If the specification defines a concept as a separate authority, the implementation must not collapse it into another module for convenience.

Invalid examples:

```text
Implement PaymentAuthorization inside Sales.
Put Payment commands inside Sales.SharedKernel.
Put Payment events inside Sales.SharedKernel.
Put Payment command handlers inside Sales.Domain.
Put Payment endpoints inside Sales.Facade.
Put Payment queries inside Sales.ReadModel.
```

Valid direction:

```text
Payment authority → Payment module
Payment commands → BrewUp.Payment.SharedKernel/Messages/Commands
Payment events → BrewUp.Payment.SharedKernel/Messages/Events
Payment command handlers → BrewUp.Payment.Domain/CommandHandlers
Payment queries → BrewUp.Payment.ReadModel/Queries
Payment endpoints → BrewUp.Payment.Facade/Endpoints
```

---

### AR-017 — Cross-context contracts must be explicit

A module may depend on another module's outcome, but the dependency must be represented through explicit contracts.

Examples:

```text
PaymentAuthorized
PaymentDeclined
PaymentOutcomeUnknown
StockReserved
StockReservationFailed
```

A consuming module may store external decision references, such as:

```text
PaymentAuthorizationId
StockReservationId
```

It must not embed the producing module's domain model.

---

### AR-018 — Sagas coordinate; they do not steal authority

The Sagas module may coordinate cross-context flows.

A saga may collect evidence and trigger commands.

A saga must not own decisions that belong to another bounded context.

Valid examples:

```text
Saga requests Payment Authorization from Payment.
Saga requests Stock Reservation from Warehouse.
Saga reacts to PaymentAuthorized.
Saga reacts to StockReserved.
Saga raises a gate event when required evidence exists.
```

Invalid examples:

```text
Saga authorizes payment itself.
Saga decrements warehouse stock itself.
Saga decides refund behavior without a domain decision.
Saga interprets payment-provider timeout without a Payment-domain decision.
```
