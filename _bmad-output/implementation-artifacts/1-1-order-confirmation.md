---
title: "Story 1.1: Establish the Payment Module and Authorization Domain"
storyId: "1.1"
storyKey: "1-1-order-confirmation"
status: review
created: 2026-07-27
updated: 2026-07-27
epic: "Evidence-Backed Sales Order Confirmation"
baseline_commit: c51190e583a1ca4bd414349b21c52c611d0304f3
---

# Story 1.1: Establish the Payment Module and Authorization Domain

Status: review

## Story

As a BrewUp operator,
I want an independently valid Payment module with its authorization domain,
so that Payment authority exists as a complete architectural increment before
later projection and provider-boundary behavior is added.

## Acceptance Criteria

1. Given no authorization exists for a stable `PaymentAuthorizationId`, when
   `RequestPaymentAuthorization` is handled, then Payment emits one
   `PaymentAuthorizationRequested` and restores its Payment ID,
   `SalesOrderReference`, and pending state. Correlation ID remains
   command/event metadata and is not retained as aggregate business state.
2. Given the same stable authorization request was already applied, when it is
   replayed, then Payment emits no second `PaymentAuthorizationRequested`.
3. Given a pending authorization and an externally supplied provider reference,
   when `RecordPaymentAuthorized` is handled, then Payment emits one
   `PaymentAuthorized`; replaying the outcome emits no second event.
4. Given Payment becomes an implemented authority, when this story completes,
   then all six exact Payment projects exist and build:
   SharedKernel, Domain, ReadModel, Infrastructure, Facade, and Tests.
5. Given the six projects exist, when solution/host composition is inspected,
   then all six are under `/50 Modules/Payment/` in `src/BrewUp.slnx`,
   `PaymentModule` is present in the REST composition root, and the REST host
   references only Payment Facade.
6. Given architecture fitness tests run, then they prove module/contract
   placement, solution and host membership, allowed references, and absence of
   Payment Domain references to Facade, ReadModel, Infrastructure, ASP.NET,
   MongoDB, RabbitMQ, Sales Domain, or Warehouse Domain.
7. Given minimal six-project/test-harness shells compile but Payment behavior
   and composition are absent, the four domain specifications and architecture
   fitness test are written and run to meaningful RED before production
   behavior; all pass after the minimum implementation.
8. Given Story 1.1 changed production code creates GUIDs or awaits tasks,
   `Story11ImplementationConventionReview` confirms new GUIDs use
   `Guid.CreateVersion7()` and awaited tasks use `ConfigureAwait(false)` unless
   an explicit architecture exception exists.
9. No amount/currency, real provider execution, decline, timeout
   interpretation, void, refund, retry, notification, expiry, reservation,
   confirmation, shipment, or invoicing behavior is introduced.

## Tasks / Subtasks

- [x] Task 1 — Create a compilable six-project/test harness shell (AC: 4, 7)
  - [x] Create the exact SharedKernel, Domain, ReadModel, Infrastructure,
    Facade, Tests `.csproj` files at the six AD-3 paths.
  - [x] Wire only AD-4 project references and repository-pinned test packages.
    The Tests project must be runnable and reference the empty Domain and
    SharedKernel shells.
  - [x] Do not add domain contracts, aggregate behavior, helpers, solution
    membership, or host registration yet. Building the shells is setup, not
    GREEN.

- [x] Task 2 — Write and run meaningful RED architecture coverage (AC: 4–7)
  - [x] Create `Architecture/PaymentModuleStructureAndComposition.cs`.
  - [x] Assert solution membership, `PaymentModule`/Program registration,
    contract placement, and AD-4 allowed/forbidden references.
  - [x] Run it immediately after the shells compile, while solution/host
    composition and contracts are absent; record an executed behavior/structure
    assertion failure before any compile-failing domain specifications exist.

- [x] Task 3 — Write and run meaningful RED domain specifications (AC: 1–3, 7)
  - [x] Create `RequestPaymentAuthorizationSuccessfully`.
  - [x] Create `DoNotRequestPaymentAuthorizationTwice`.
  - [x] Create `RecordPaymentAuthorizedSuccessfully`.
  - [x] Create `DoNotAuthorizePaymentTwice`.
  - [x] Seed request/outcome history for both replay tests.
  - [x] Run the four tests from the existing Tests project and record RED due to
    missing Payment contracts/domain behavior, not project-not-found.

- [x] Task 4 — Implement Payment-owned contracts (AC: 1–3, 9)
  - [x] Create
    `SharedKernel/DomainIds/PaymentAuthorizationId.cs` deriving from
    `Muflone.Core.DomainId`.
  - [x] Create `SharedKernel/CustomTypes/SalesOrderReference.cs`.
  - [x] Create
    `SharedKernel/Messages/Commands/RequestPaymentAuthorization.cs` and
    `RecordPaymentAuthorized.cs`.
  - [x] Create
    `SharedKernel/Messages/Events/PaymentAuthorizationRequested.cs`,
    `PaymentAuthorized.cs`, and the contract
    `PaymentAuthorizedIntegrationEvent.cs`; Story 1.2 owns publishing it.
  - [x] Contracts contain only approved IDs, Sales Order reference, provider
    reference, and correlation metadata. Correlation ID stays on command/events,
    not aggregate business state.

- [x] Task 5 — Implement the event-sourced aggregate and handlers (AC: 1–3)
  - [x] Create `Domain/Entities/PaymentAuthorization.cs`.
  - [x] `Request(...)` raises `PaymentAuthorizationRequested`; apply restores
    ID, Sales Order reference, and pending state. Replaying the stable request
    is a no-op.
  - [x] `RecordAuthorized(...)` raises `PaymentAuthorized` once; an
    already-authorized aggregate returns without raising an event.
  - [x] Create
    `Domain/CommandHandlers/RequestPaymentAuthorizationCommandHandler.cs` and
    `RecordPaymentAuthorizedCommandHandler.cs`.
  - [x] Handlers perform only load/create -> aggregate method -> repository
    save and use `ConfigureAwait(false)`.
  - [x] Use stable IDs from commands; use `Guid.CreateVersion7()` only where a
    new technical GUID is actually required.

- [x] Task 6 — Add minimal helpers, solution, and REST composition (AC: 4–6)
  - [x] Add only minimal compiling `DomainHelper`,
    `PaymentReadModelHelper`, `InfrastructureHelper`, and
    `PaymentFacadeHelper`; Story 1.2 completes their behavior.
  - [x] Add all six project paths beneath `/50 Modules/Payment/` in
    `src/BrewUp.slnx`.
  - [x] Create `src/BrewUp.Rest/Module/PaymentModule.cs` implementing `IModule`.
  - [x] Add `new PaymentModule()` to `src/BrewUp.Rest/Program.cs`.
  - [x] Add only the Payment Facade project reference to
    `src/BrewUp.Rest/BrewUp.Rest.csproj`.
  - [x] Minimal `PaymentModule` registration may compose the empty
    Story-1.1-safe Facade shell; it must not expose a callback endpoint or
    publish integration events. Story 1.2 owns those behaviors.

- [x] Task 7 — Verify GREEN and conventions (AC: 1–9)
  - [x] Run the four targeted Payment domain specifications.
  - [x] Run `PaymentModuleStructureAndComposition`.
  - [x] Run the full Payment Tests project.
  - [x] Run affected REST architecture tests without starting
    infrastructure-dependent fixtures.
  - [x] Build `src/BrewUp.slnx`.
  - [x] Run `Story11ImplementationConventionReview` over only Story 1.1 changed
    production files for `Guid.CreateVersion7()` and `ConfigureAwait(false)`.
  - [x] Confirm the changed-file list contains no Sales, Warehouse, Sagas, real
    provider, compensation, retry, timeout, shipment, or invoice behavior.

## Dev Notes

### Developer context

This is the architecture-foundation and Payment-domain story, not the complete
Payment integration story. It must leave Payment independently valid under
AR-001/AR-002 while keeping Story 1.2's projection, EventStore/Mongo
persistence, facade endpoint, provider callback, and integration publisher
unimplemented.

Payment alone owns authorization state and events. The caller supplies the
authorized outcome; neither the REST shell nor saga exists in this story to
decide it. Sales Order references are correlation data, not embedded Sales
models.

### Current brownfield state

- `src/BrewUp.Rest/Program.cs` registers modules explicitly through
  `builder.RegisterModules([...])`; insert `new PaymentModule()` with other
  business modules.
- `src/BrewUp.Rest/BrewUp.Rest.csproj` references module Facades only; add
  Payment Facade and no other Payment layer.
- `src/BrewUp.slnx` groups module projects beneath `/50 Modules/<Module>/`;
  add one `/50 Modules/Payment/` folder containing exactly six projects.
- Existing projects target `net10.0`. Existing module contracts use Muflone
  10.2.1; tests use Muflone.SpecificationTests 10.0.0, xUnit 2.9.3, and
  NetArchTest.Rules 1.3.2. Use repository-pinned versions rather than
  introducing alternatives.

### Architecture compliance

```text
Facade         -> Domain, ReadModel, Infrastructure, SharedKernel
Domain         -> SharedKernel
ReadModel      -> SharedKernel
Infrastructure -> Domain, SharedKernel
Tests          -> owned layers as needed
REST host      -> Facade only
```

Cross-module references may target explicit SharedKernel contracts only. No
Payment project may reference Sales/Warehouse Domain or Infrastructure.

The six required paths are:

```text
src/Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj
src/Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj
src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj
src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj
src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj
src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
```

### Required contract shape

```csharp
public sealed class PaymentAuthorizationId(string value) : DomainId(value);
public sealed record SalesOrderReference(string Value);

public sealed class RequestPaymentAuthorization(
    PaymentAuthorizationId aggregateId,
    SalesOrderReference salesOrder,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public SalesOrderReference SalesOrder { get; } = salesOrder;
}
```

`RecordPaymentAuthorized` carries the Payment ID, externally supplied provider
reference, and correlation/message ID. Correlation is message/event metadata,
not retained aggregate business state. `PaymentAuthorized` carries the approved
owned evidence and metadata. Do not add policy fields not present upstream.

### Testing requirements

- TDD order is mandatory: create compilable project/test shells; write and
  execute the architecture fitness test to assertion RED; then write/run the
  four domain specifications against missing behavior to compile/test RED;
  implement the minimum behavior, then capture GREEN.
- Command handlers contain no business policy.
- Architecture coverage must check file/project placement, project references,
  contract placement, solution membership, and REST registration.
- Static/code review covers NFR-005 only across Story 1.1 changed production
  files; every later story owns its own convention verification.
- Infrastructure-dependent integration fixtures are not required for this
  story; Story 1.2 adds persistence/boundary tests.

### Stop conditions

Stop and report a blocking policy conflict if implementation would require:

- Payment amount/currency or a real-provider decision;
- decline, unknown, or timeout interpretation;
- void, refund, compensation, retry, notification, or expiry behavior;
- partial Payment module composition;
- a cross-module Domain/Infrastructure reference;
- Payment behavior under Sales, Warehouse, Sagas, or the REST host.

Do not invent a workaround.

### Project structure notes

No starter template or database migration is required. The new module follows
the existing brownfield module and composition patterns. The optional Entities
project is explicitly not used.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.1]
- [Source: `_bmad-output/planning-artifacts/prd.md` — FR-001–FR-004,
  FR-016–FR-019, NFR-001, NFR-002, NFR-005]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — AD-1 through
  AD-5, AD-10, AD-11]
- [Source:
  `docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`
  — Payment module, Contracts, Testing Strategy, Out of Scope]
- [Source:
  `.bmad-harness/governance/brewup-sales-order-confirmation.md` — BC-000,
  BC-003, BC-004, BC-008, BC-011]
- [Source: `.bmad-harness/governance/brewup-module-structure.md` — AR-000
  through AR-017]
- [Source: `_bmad-output/project-context.md` — Technology and architecture;
  Implementation discipline]

## Governance traceability

| Story concern | Requirements/decisions | Rules |
|---|---|---|
| Complete Payment authority/module | FR-001, FR-016, FR-017; AD-3 | BC-003; BC-004; AR-001; AR-002; AR-013; AR-014; AR-016 |
| Contract and ID ownership | FR-001–FR-003; AD-2, AD-5 | BC-003; BC-004; AR-004–AR-007; AR-017 |
| Event-sourced pending/authorized behavior | FR-002–FR-004; AD-1, AD-5 | BC-003; BC-004; BC-008; AR-008; AR-012 |
| Dependencies and composition | FR-018; AD-4 | AR-011; AR-013–AR-017 |
| TDD and conventions | FR-019; NFR-005; AD-10, AD-11 | AR-000; AR-012 |
| Excluded policy | AC9; approved design Out of Scope | BC-000; BC-003; BC-011 |

## Completion status

Ultimate context engine analysis completed - comprehensive developer guide
created.

## Dev Agent Record

### Agent Model Used

OpenAI GPT-5 Codex

### Implementation Plan

- Establish all six exact Payment project shells with only approved dependency
  directions before adding behavior or composition.
- Prove architecture RED first, then prove all four domain specifications RED,
  and add the minimum Payment-owned contracts and event-sourced behavior.
- Add Story-1.1-safe helper shells and REST/solution composition without
  projection, persistence, callback, endpoint, or integration publication.
- Re-run focused, module, REST architecture, solution build, convention, and
  regression checks before review.

### Debug Log References

- Architecture RED:
  `dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter FullyQualifiedName~PaymentModuleStructureAndComposition`
  executed 5 tests: 3 failed for the missing SharedKernel contract,
  `/50 Modules/Payment/` solution folder, and REST `PaymentModule`; 2 reference
  boundary checks passed.
- Domain RED:
  `dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter "FullyQualifiedName~RequestPaymentAuthorizationSuccessfully|FullyQualifiedName~DoNotRequestPaymentAuthorizationTwice|FullyQualifiedName~RecordPaymentAuthorizedSuccessfully|FullyQualifiedName~DoNotAuthorizePaymentTwice"`
  failed with CS0234/CS0246 for the deliberately absent Payment contracts and
  handlers after the Tests project itself restored and built.
- GREEN: targeted domain 4/4, architecture 5/5, full Payment 9/9, REST
  architecture 1/1, and `dotnet build src/BrewUp.slnx` completed with 0 errors.
- Regression classification:
  `dotnet test src/BrewUp.slnx --no-restore --no-build -v:q` retained the known
  Purchases `SubmitPurchaseOrderSuccessfully.SetUp` failure
  (`NotImplementedException`) and the known RabbitMQ-dependent REST fixture/DI
  testhost termination; all completed non-REST module suites, including
  Payment, passed.
- `Story11ImplementationConventionReview`: PASS over 15 new Story 1.1
  production `.cs` files; 2/2 GUID creation sites use
  `Guid.CreateVersion7()`, and 4/4 awaits use `ConfigureAwait(false)`.

### Completion Notes List

- Implemented an independently valid Payment authority under BC-003/BC-004 and
  AR-001/AR-002 with all six standard projects and approved references.
- Added strongly typed authorization contracts plus pending-to-authorized
  event-sourced behavior; stable request and authorized-outcome replays are
  idempotent no-ops.
- Added minimal Payment helper shells, all solution entries, explicit REST
  module registration, and a Facade-only REST project reference.
- Preserved BC-008/BC-011 by leaving provider decisions, projection,
  persistence, callback, publication, compensation, timeout, retry, refund,
  void, shipment, invoice, Sales, Warehouse, and Sagas behavior out of scope.
- Self-review found no Story 1.1 regression or unresolved policy decision.
- Review fix: architecture coverage now inspects ProjectReference,
  PackageReference, FrameworkReference, and Reference items so unused forbidden
  dependencies cannot evade the Payment/REST boundary checks.

### File List

- _bmad-output/implementation-artifacts/1-1-order-confirmation.md
- src/BrewUp.Rest/BrewUp.Rest.csproj
- src/BrewUp.Rest/Module/PaymentModule.cs
- src/BrewUp.Rest/Program.cs
- src/BrewUp.slnx
- src/Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj
- src/Payment/BrewUp.Payment.Domain/CommandHandlers/RecordPaymentAuthorizedCommandHandler.cs
- src/Payment/BrewUp.Payment.Domain/CommandHandlers/RequestPaymentAuthorizationCommandHandler.cs
- src/Payment/BrewUp.Payment.Domain/DomainHelper.cs
- src/Payment/BrewUp.Payment.Domain/Entities/PaymentAuthorization.cs
- src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj
- src/Payment/BrewUp.Payment.Facade/PaymentFacadeHelper.cs
- src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj
- src/Payment/BrewUp.Payment.Infrastructure/InfrastructureHelper.cs
- src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj
- src/Payment/BrewUp.Payment.ReadModel/PaymentReadModelHelper.cs
- src/Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj
- src/Payment/BrewUp.Payment.SharedKernel/CustomTypes/SalesOrderReference.cs
- src/Payment/BrewUp.Payment.SharedKernel/DomainIds/PaymentAuthorizationId.cs
- src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/RecordPaymentAuthorized.cs
- src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/RequestPaymentAuthorization.cs
- src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorizationRequested.cs
- src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorized.cs
- src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorizedIntegrationEvent.cs
- src/Payment/BrewUp.Payment.Tests/Architecture/PaymentModuleStructureAndComposition.cs
- src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
- src/Payment/BrewUp.Payment.Tests/Domain/DoNotAuthorizePaymentTwice.cs
- src/Payment/BrewUp.Payment.Tests/Domain/DoNotRequestPaymentAuthorizationTwice.cs
- src/Payment/BrewUp.Payment.Tests/Domain/RecordPaymentAuthorizedSuccessfully.cs
- src/Payment/BrewUp.Payment.Tests/Domain/RequestPaymentAuthorizationSuccessfully.cs

## Change Log

- 2026-07-27: Established the complete six-project Payment boundary,
  authorization contracts/domain, architecture/domain tests, and minimal
  solution/REST composition for Story 1.1.
- 2026-07-27: Strengthened dependency-item architecture coverage and aligned
  frontmatter status with the body review status.
