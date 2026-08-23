---
description: Validate BrewUp module structure against architecture memory rules.
handoffs:
  - label: Analyze Consistency
    agent: speckit.analyze
    prompt: Analyze specification, plan, tasks, and architecture rules for consistency.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Module Structure Guard

You are reviewing BrewUp Spec Kit artifacts and repository structure against the BrewUp modular architecture rules.

This command may run after `/speckit.plan`, after `/speckit.tasks`, or after implementation work.

Its purpose is not to design the architecture creatively.

Its purpose is to verify that business authorities and bounded contexts are represented by the correct BrewUp module structure.

## Goal

Detect whether a generated specification, implementation plan, task list, or source structure violates the BrewUp module architecture.

The most important risk is that the model correctly identifies a business authority, such as `Payment`, but fails to create the corresponding BrewUp module.

Example of the failure this guard must detect:

```text
The specification says:
- Payment owns payment authorization.

But the plan creates:
- PaymentAuthorizationId inside Sales.
- Payment commands inside Sales.SharedKernel.
- Payment behavior inside Sales.Domain.

And it does not create:
- src/Payment/
- BrewUp.Payment.Domain
- BrewUp.Payment.Facade
- BrewUp.Payment.Infrastructure
- BrewUp.Payment.ReadModel
- BrewUp.Payment.SharedKernel
- BrewUp.Payment.Tests
```

This is architectural drift.

Correct domain ownership without correct solution structure is still architectural drift.

## Files and folders to read

Read the following files if they exist:

- `.specify/memory/constitution.md`
- `.specify/memory/architecture/brewup-module-structure.md`
- `.specify/memory/domain-carriers/brewup-sales-order-confirmation.md`
- `.specify/feature.json`
- `specs/001-order-confirmation/spec.md`
- `specs/001-order-confirmation/plan.md`
- `specs/001-order-confirmation/tasks.md`
- `specs/001-order-confirmation/analyze-report.md`
- `src/BrewUp.slnx`
- `src/BrewUp.Rest/Module/*.cs`
- `src/*/BrewUp.*.SharedKernel/*.csproj`
- `src/*/BrewUp.*.Domain/*.csproj`
- `src/*/BrewUp.*.ReadModel/*.csproj`
- `src/*/BrewUp.*.Infrastructure/*.csproj`
- `src/*/BrewUp.*.Facade/*.csproj`
- `src/*/BrewUp.*.Tests/*.csproj`

If the active feature directory is different, resolve it from `.specify/feature.json` if available.

If `.specify/feature.json` does not exist, scan `specs/*/spec.md`, `specs/*/plan.md`, and `specs/*/tasks.md` and use the files that most clearly refer to Sales Order confirmation.

Do not fail because optional files are missing.

Report missing files and continue with the available information.

If neither architecture memory nor constitution exists, stop and report:

```text
BrewUp Module Structure Guard: BLOCKED
Reason: no architecture rules found.
```

## Conceptual distinction

Domain rules answer:

```text
Who owns the decision?
```

Architecture rules answer:

```text
Where must that ownership live in the solution?
```

Both must be respected.

A plan may satisfy domain ownership and still fail architecture structure.

Example:

```text
Payment owns payment authorization.
```

is not enough if the plan does not create:

```text
src/Payment/BrewUp.Payment.Domain
```

## Architecture rules to enforce

### AR-000 — Architecture memory is authoritative

The rules in `.specify/memory/architecture/brewup-module-structure.md` are authoritative for the physical structure of BrewUp modules.

Generated artifacts must not invent ad-hoc project structures when the architecture memory defines the expected one.

### AR-001 — Bounded contexts must be explicit modules

Every bounded context or business authority that is in scope for implementation must be represented as an explicit module under:

```text
src/<ModuleName>/
```

A business authority must not be implemented as loose folders or classes inside another module.

If the specification introduces `Payment` as a separate authority and Payment behavior is in scope, the plan and tasks must create:

```text
src/Payment/
```

and a corresponding solution folder.

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

Do not create alternative project names such as:

```text
BrewUp.<ModuleName>.Application
BrewUp.<ModuleName>.Api
BrewUp.<ModuleName>.Contracts
BrewUp.<ModuleName>.Core
BrewUp.<ModuleName>.Persistence
```

unless an explicit architecture decision changes the standard structure.

### AR-003 — Optional Entities project

A module may have an optional `Entities` project only when there is a clear need for value objects or internal shared structures used inside that bounded context.

Do not create an `Entities` project by default for every new module.

If the generated plan creates `BrewUp.<ModuleName>.Entities`, it must explain why.

### AR-004 — SharedKernel owns module contracts

Each module has its own `SharedKernel`.

For a module named `<ModuleName>`, contracts and shared types owned by that module belong under:

```text
src/<ModuleName>/BrewUp.<ModuleName>.SharedKernel/
├── DomainIds/
├── CustomTypes/
├── Enums/
└── Messages/
    ├── Commands/
    └── Events/
```

Payment contracts must not be placed inside `BrewUp.Sales.SharedKernel`.

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

Only create commands required by the specification, plan, or clarified domain decisions.

Do not invent refund, void, retry, timeout, or payment-terms commands unless explicitly needed.

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

### AR-008 — Domain project owns business behavior

The `Domain` project contains:

- aggregates;
- entities owned by the module;
- value objects when they belong directly to the domain model;
- command handlers;
- domain services;
- domain behavior;
- domain helper registration.

Business rules must live in aggregate methods or domain services.

Command handlers must not contain business logic.

Facade and endpoint code must not contain business logic.

### AR-009 — ReadModel project owns queries and projections

The `ReadModel` project contains:

- read model documents;
- query services;
- domain event handlers that update projections;
- read model helper registration.

Queries belong in the `ReadModel` project.

Queries must not be implemented in the `Domain` project.

### AR-010 — Infrastructure project owns persistence and technical wiring

The `Infrastructure` project contains module-specific infrastructure concerns.

Infrastructure responsibilities include:

- EventStore persister wiring;
- MongoDB collection setup;
- technical adapters;
- infrastructure configuration;
- infrastructure helper registration.

The Domain project must not reference EventStore, MongoDB, RabbitMQ, ASP.NET Core, or any infrastructure framework.

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

Other modules must not call another module's Domain project directly.

Cross-module interaction must happen through:

- shared messages;
- ACL handlers;
- integration events;
- the module facade;
- explicit integration contracts.

### AR-012 — Tests project is mandatory

Every implemented module must have a Tests project.

For a module named `<ModuleName>`, use:

```text
src/<ModuleName>/BrewUp.<ModuleName>.Tests/
├── Architecture/
└── Domain/
```

The Tests project must include architecture tests and domain behavior tests when behavior is introduced.

### AR-013 — REST host must register modules explicitly

Every implemented module must be registered in the REST host using the `IModule` pattern.

For a module named `<ModuleName>`, create or update:

```text
src/BrewUp.Rest/Module/<ModuleName>Module.cs
```

The REST host must only compose modules.

It must not contain business logic.

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

### AR-017 — Cross-context contracts must be explicit

A module may depend on another module's outcome, but the dependency must be represented through explicit contracts.

A consuming module may store external decision references, such as:

```text
PaymentAuthorizationId
StockReservationId
```

It must not embed the producing module's domain model.

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

## Payment-specific checks

If Payment is in scope, check that the plan, tasks, or source structure create the full Payment module:

```text
src/Payment/
├── BrewUp.Payment.SharedKernel/
├── BrewUp.Payment.Domain/
├── BrewUp.Payment.ReadModel/
├── BrewUp.Payment.Infrastructure/
├── BrewUp.Payment.Facade/
└── BrewUp.Payment.Tests/
```

Check that Payment IDs belong under:

```text
src/Payment/BrewUp.Payment.SharedKernel/DomainIds/
```

Check that Payment commands belong under:

```text
src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/
```

Check that Payment events belong under:

```text
src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/
```

If Payment is referenced only as an external already-existing dependency, the specification must state that explicitly.

If the specification does not clarify whether Payment is in scope or external, report a blocking ambiguity.

## How to determine whether Payment is in scope

Treat Payment as in scope if any of the following is true:

- `spec.md` says Payment must be implemented;
- `plan.md` contains Payment behavior, commands, events, endpoints, queries, or tests;
- `tasks.md` contains Payment implementation tasks;
- source code contains Payment behavior inside Sales or another module;
- the feature requires payment authorization behavior and does not explicitly mark Payment as external or out of scope.

Treat Payment as out of scope only if the specification explicitly says that Payment is an already-existing external dependency for this feature.

If Payment ownership is described but implementation scope is ambiguous, return `BLOCKED`.

## Valid patterns

Valid plan or task examples:

```markdown
- [ ] Create `src/Payment` solution folder and directory.
- [ ] Create `BrewUp.Payment.SharedKernel`.
- [ ] Create `BrewUp.Payment.Domain`.
- [ ] Create `BrewUp.Payment.ReadModel`.
- [ ] Create `BrewUp.Payment.Infrastructure`.
- [ ] Create `BrewUp.Payment.Facade`.
- [ ] Create `BrewUp.Payment.Tests`.
- [ ] Add Payment projects to `src/BrewUp.slnx`.
- [ ] Register Payment module in `BrewUp.Rest`.
- [ ] Add Payment architecture tests.
```

Valid Payment contract tasks:

```markdown
- [ ] Add `PaymentAuthorizationId` under `BrewUp.Payment.SharedKernel/DomainIds`.
- [ ] Add `RequestPaymentAuthorization` under `BrewUp.Payment.SharedKernel/Messages/Commands`.
- [ ] Add `PaymentAuthorized` under `BrewUp.Payment.SharedKernel/Messages/Events`.
```

Valid external-reference task in Sales:

```markdown
- [ ] Add `PaymentAuthorizationId` to SalesOrder as an external decision reference.
```

## Invalid patterns

Fail if any artifact contains these patterns while Payment is in scope:

```markdown
- [ ] Add Payment commands under `BrewUp.Sales.SharedKernel`.
- [ ] Add Payment events under `BrewUp.Sales.SharedKernel`.
- [ ] Implement payment authorization inside `BrewUp.Sales.Domain`.
- [ ] Add Payment endpoints inside `BrewUp.Sales.Facade`.
- [ ] Add Payment queries inside `BrewUp.Sales.ReadModel`.
- [ ] Add `PaymentAuthorizationId` only under Sales when Payment module is in scope.
- [ ] Implement Payment as a single project.
- [ ] Implement Payment as loose folders inside Sales.
```

Fail if the plan or source structure creates a partial Payment module such as:

```text
src/Payment/BrewUp.Payment.Domain
```

without:

```text
BrewUp.Payment.SharedKernel
BrewUp.Payment.Facade
BrewUp.Payment.Infrastructure
BrewUp.Payment.ReadModel
BrewUp.Payment.Tests
```

unless the plan explicitly stages module creation and marks missing projects as planned work.

## Severity levels

Use these severity levels.

### CRITICAL

Use `CRITICAL` when a business authority is collapsed into the wrong module or the required module structure is missing.

Examples:

- Payment is in scope but no `src/Payment` module is created.
- Payment behavior is implemented inside Sales.
- Payment commands or events are placed inside Sales.SharedKernel.
- Sales references Payment Domain or Infrastructure directly.
- Domain project references infrastructure frameworks.

### HIGH

Use `HIGH` when the module exists but is structurally incomplete.

Examples:

- Missing Facade project.
- Missing SharedKernel project.
- Missing Tests project.
- Module projects are not added to the solution.
- Module is not registered in the REST host.
- Missing architecture tests.

### MEDIUM

Use `MEDIUM` when naming or layout is imprecise but the structure is still recoverable.

Examples:

- Folder exists but project naming differs slightly.
- Optional `Entities` project is created without explanation.
- Helper file naming does not match existing conventions.

### LOW

Use `LOW` for minor documentation or wording issues.

## Output format

Return the result using this exact structure:

```text
BrewUp Module Structure Guard: PASS|FAIL|BLOCKED

Checked files:
- ...

Checked folders:
- ...

Missing optional files:
- ...

Scope assessment:
- Payment in scope: YES|NO|AMBIGUOUS
- Reason: ...

Architecture memory check:
- PASS|FAIL|BLOCKED
- Details: ...

Module structure check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

Payment module check:
- PASS|FAIL|BLOCKED|NOT APPLICABLE
- Details: ...

SharedKernel contract placement check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

Layer boundary check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

REST registration check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

Solution inclusion check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

Architecture test check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

Violations:
- Severity:
  Rule:
  Location:
  Evidence:
  Why it matters:
  Suggested correction:

Missing required tasks:
- ...

Recommended next step:
- Continue to /speckit.tasks
- Continue to implementation
- Fix the plan before generating tasks
- Fix the tasks before implementation
- Ask a domain expert or architect to clarify implementation scope
```

If there are no violations, write:

```text
Violations:
- None
```

If no missing required tasks exist, write:

```text
Missing required tasks:
- None
```

## Expected failure: missing Payment module

If Payment is introduced as a separate authority and is in scope for implementation, but the plan does not create a Payment module, report:

```text
CRITICAL — Missing module structure

The specification introduces Payment as a separate authority, but the implementation plan does not create a Payment module.

Violates:
AR-001 — Bounded contexts must be explicit modules.
AR-002 — Standard module project structure.
AR-016 — Business authority must not be collapsed into another module.

Suggested correction:
Create the full Payment module under src/Payment with SharedKernel, Domain, ReadModel, Infrastructure, Facade, and Tests projects.
```

## Expected failure: Payment behavior inside Sales

If tasks implement Payment behavior inside Sales, report:

```text
CRITICAL — Business authority collapsed into Sales

The generated task list implements Payment behavior inside Sales instead of creating a Payment module.

Violates:
AR-001 — Bounded contexts must be explicit modules.
AR-004 — SharedKernel owns module contracts.
AR-008 — Domain project owns business behavior.
AR-016 — Business authority must not be collapsed into another module.

Suggested correction:
Move Payment contracts and behavior into the Payment module. Sales may store PaymentAuthorizationId only as an external decision reference.
```

## Expected blocked result: ambiguous Payment scope

If Payment is described as an authority but the specification does not say whether Payment is implemented in this feature or treated as an existing external dependency, report:

```text
BrewUp Module Structure Guard: BLOCKED

Scope assessment:
- Payment in scope: AMBIGUOUS
- Reason: Payment is described as a separate authority, but the specification does not state whether Payment must be implemented as a BrewUp module or treated as an already-existing external dependency.

Recommended next step:
- Ask a domain expert or architect to clarify implementation scope.
```

## Correction guidance

Prefer:

```markdown
- [ ] Create `src/Payment` module.
- [ ] Create `BrewUp.Payment.SharedKernel`.
- [ ] Create `BrewUp.Payment.Domain`.
- [ ] Create `BrewUp.Payment.ReadModel`.
- [ ] Create `BrewUp.Payment.Infrastructure`.
- [ ] Create `BrewUp.Payment.Facade`.
- [ ] Create `BrewUp.Payment.Tests`.
```

not:

```markdown
- [ ] Add payment authorization classes under Sales.
```

Prefer:

```markdown
- [ ] Add `PaymentAuthorizationId` under `BrewUp.Payment.SharedKernel/DomainIds`.
```

not:

```markdown
- [ ] Add `PaymentAuthorizationId` only under `BrewUp.Sales.SharedKernel`.
```

Prefer:

```markdown
- [ ] Add `RequestPaymentAuthorization` under `BrewUp.Payment.SharedKernel/Messages/Commands`.
```

not:

```markdown
- [ ] Add `RequestPaymentAuthorization` under `BrewUp.Sales.SharedKernel/Messages/Commands`.
```

Prefer:

```markdown
- [ ] Register `PaymentModule` in `BrewUp.Rest`.
```

not:

```markdown
- [ ] Add Payment endpoints to SalesFacade.
```

## Constraints

Do not create files.

Do not modify files.

Do not generate code.

Do not implement tasks.

Do not silently decide whether Payment is in scope if the specification is ambiguous.

Do not move Payment behavior into Sales.

Do not invent an alternative module structure.

Do not weaken the BrewUp modular monolith conventions.

Your job is to protect the solution structure from architectural drift.
