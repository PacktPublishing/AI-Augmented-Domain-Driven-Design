---
title: "Story 1.2: Complete Payment Projection and Provider Callback Behavior"
storyId: "1.2"
storyKey: "1-2-complete-payment-projection-and-provider-callback-behavior"
status: review
created: 2026-07-27
updated: 2026-07-27
epic: "Evidence-Backed Sales Order Confirmation"
previous_story: "_bmad-output/implementation-artifacts/1-1-order-confirmation.md"
previous_story_head: 7d3cd354b2ab7289b64aa254e94dcb126217980b
baseline_commit: 2dad5dd54851dff8a74a1bc179d2b832ee553533
---

# Story 1.2: Complete Payment Projection and Provider Callback Behavior

Status: review

## Story

As a BrewUp operator,
I want Payment authorization exposed through a complete registered module,
so that provider-supplied authorized outcomes are projected and published
through Payment-owned integration boundaries.

## Acceptance Criteria

1. Given Story 1.1's six Payment projects, contracts, aggregate, handlers,
   solution membership, and REST composition, when Story 1.2 completes, then
   ReadModel, Infrastructure, and Facade contain the Payment projection,
   existing EventStore/Mongo composition, provider-simulator callback, and
   Payment-owned integration publication without duplicating the module or
   infrastructure stack.
2. Given `PaymentAuthorizationRequested`, when the pending projection handler
   runs, then one Payment projection is stored with the authorization ID,
   Sales Order reference, `pending` status, and no provider reference.
3. Given the simulator supplies a provider reference, when it calls
   `POST /v1/payment/authorizations/{authorizationId}/authorized`, then the
   Payment endpoint delegates to `IPaymentFacade.RecordAuthorizedAsync`, the
   Facade sends `RecordPaymentAuthorized`, and neither layer calculates or
   interprets an authorization decision.
4. Given the Payment aggregate emits `PaymentAuthorized`, when registered
   domain-event handlers run, then the projection becomes `authorized` with
   the provider reference and one existing
   `PaymentAuthorizedIntegrationEvent` is published with the Payment-owned ID
   and preserved correlation metadata.
5. Given the same authorized callback is delivered again, when the Facade
   sends the command against the already-authorized aggregate, then Story
   1.1's state guard emits no second `PaymentAuthorized`; consequently no
   second integration outcome is published.
6. Given BrewUp's shared infrastructure has registered `IMongoClient`,
   RabbitMQ transport, and the Muflone EventStore repository, when Payment is
   composed, then the Payment Mongo persister, query/service, projection
   handlers, command handlers, Facade, publisher, and endpoint are registered
   exactly once. Story 1.2 creates no module-specific EventStore client,
   RabbitMQ transport, or Mongo client.
7. Given the REST host composes modules, when it starts, then the existing
   `PaymentModule` registers `AddPaymentFacade(...)` and configures
   `MapPaymentEndpoints()`; `Program.cs` still contains one
   `new PaymentModule()`, the REST project still references Payment Facade
   only, and the host contains no Payment business logic.
8. Given architecture fitness tests inspect Payment, then the exact six
   projects and `/50 Modules/Payment/` solution membership remain valid,
   Payment-owned contracts remain in SharedKernel, all project references
   follow AD-4/AR-015, and Payment Domain has no Facade, ReadModel,
   Infrastructure, ASP.NET, MongoDB, RabbitMQ, Sales Domain/Infrastructure, or
   Warehouse Domain/Infrastructure dependency.
9. Given callback, projection, publication, and persistence wiring are absent,
   when the four tests `RecordAuthorizedCallbackBoundary`,
   `ProjectPendingAuthorization`, `ProjectAndPublishAuthorizedOutcome`, and
   `PaymentPersistenceWiring` are written and executed before any Story 1.2
   production change, then the run records meaningful RED caused by missing
   Story 1.2 behavior. The same four tests pass after the minimum
   implementation, followed by the full Payment suite, REST architecture
   suite, and solution build.
10. Given Story 1.2 production files have reached GREEN, when
    `Story12ImplementationConventionReview` runs over only those changed
    production files, then every new GUID uses `Guid.CreateVersion7()`, every
    awaited task uses `ConfigureAwait(false)`, and no exception is accepted
    without explicit architecture approval.
11. No real-provider execution, amount/currency, decline, unknown outcome,
    timeout interpretation, authentication change, void, refund,
    compensation, retry, notification, expiry, reservation, Sales
    confirmation, saga, shipment, invoicing, or dependency-upgrade behavior is
    introduced.

## Tasks / Subtasks

- [x] Task 1 — Add all four boundary/integration tests before production
  changes (AC: 2–6, 9)
  - [x] Record the implementation baseline with `git rev-parse HEAD`; do not
    modify a Story 1.2 production file before the RED run.
  - [x] Create
    `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectPaymentAuthorizationRequested.cs`
    with test class/specification name `ProjectPendingAuthorization`.
  - [x] Create
    `src/Payment/BrewUp.Payment.Tests/Facade/RecordAuthorizedProviderOutcome.cs`
    with test class/specification name `RecordAuthorizedCallbackBoundary`.
  - [x] Create
    `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectAndPublishAuthorizedOutcome.cs`
    with test class/specification name
    `ProjectAndPublishAuthorizedOutcome`; it must prove projection update,
    one integration publication, and no second publication after duplicate
    callback delivery.
  - [x] Create
    `src/Payment/BrewUp.Payment.Tests/Infrastructure/PaymentPersistenceWiring.cs`
    with test class/specification name `PaymentPersistenceWiring`; verify
    Payment-specific DI and keyed Mongo persister wiring without requiring a
    live MongoDB, RabbitMQ, or EventStore process.
  - [x] Update the Tests project references only as needed for these tests to
    see Payment's owned layers. Test scaffolding is allowed before RED;
    production behavior is not.

- [x] Task 2 — Execute and record mandatory meaningful RED (AC: 9)
  - [x] Run all four named tests in one focused command:

    ```powershell
    dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter "FullyQualifiedName~RecordAuthorizedCallbackBoundary|FullyQualifiedName~ProjectPendingAuthorization|FullyQualifiedName~ProjectAndPublishAuthorizedOutcome|FullyQualifiedName~PaymentPersistenceWiring"
    ```

  - [x] Record the executed failure. An expected compile failure must name
    absent Story 1.2 types/members; an assertion failure must identify absent
    projection/callback/publication/persistence wiring. A project-not-found,
    restore, syntax, or unrelated infrastructure failure is not acceptable
    RED.
  - [x] Stop if any Story 1.2 production behavior was already added before
    this run or if all four tests do not participate in the focused run.

- [x] Task 3 — Add only the minimum registered Payment behavior (AC: 1–7)
  - [x] Complete `DomainHelper` by registering Story 1.1's two existing
    command handlers. Do not move or duplicate domain behavior in Facade,
    ReadModel, Infrastructure, or REST.
  - [x] Add the Payment projection DTO, query, service, and two projection
    handlers. The DTO derives from the repository's `DtoBase`, uses inherited
    `Id`, and adds `SalesOrderId`, `Status`, and nullable
    `ProviderReference`; pending and authorized values are lower-case as
    specified by the Task 3 brief.
  - [x] Add the Payment Mongo `IPersister` using the existing module-keyed
    persister pattern and database name `Payment`. Reuse the host-registered
    `IMongoClient`; do not construct a client or connection string.
  - [x] Complete `PaymentReadModelHelper` and `InfrastructureHelper` with
    query/service, domain-event-handler, and keyed-persister registrations.
  - [x] Add `IPaymentFacade`, `PaymentFacade`, callback JSON, and endpoint.
    `PaymentFacade.RecordAuthorizedAsync(...)` sends the existing
    `RecordPaymentAuthorized` command through `IServiceBus` with a new
    `Guid.CreateVersion7()` message/correlation ID and uses
    `ConfigureAwait(false)`.
  - [x] Keep `PaymentAuthorizedEventHandler` responsible for projection state
    and `PaymentAuthorizedIntegrationEventPublisher` responsible for
    publishing the existing SharedKernel integration event. Preserve the
    domain event's correlation metadata; do not create a second outcome
    contract or publish from the endpoint.
  - [x] Complete `PaymentFacadeHelper` by composing Domain, ReadModel,
    Infrastructure, Facade, and publisher registrations once. Do not register
    global Mongo/EventStore/RabbitMQ infrastructure again.
  - [x] Update the existing `PaymentModule.Configure(...)` to map the endpoint.
    Do not add another `PaymentModule` or modify `Program.cs` merely to
    re-register it.

- [x] Task 4 — Prove focused GREEN, then preserve architecture (AC: 2–9)
  - [x] Re-run the exact four-test filter and require all four named tests to
    pass.
  - [x] Update the existing
    `PaymentModuleStructureAndComposition` expectations from Story 1.1's
    intentionally empty callback shell to Story 1.2's completed mapping and
    owned-layer test references.
  - [x] Keep its existing checks for six-project placement, contract
    placement, solution membership, host registration, governed dependency
    item types, Facade-only REST reference, and forbidden Domain/cross-context
    dependencies.
  - [x] Run:

    ```powershell
    dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
    dotnet test src/BrewUp.Rest.Tests/BrewUp.Rest.Tests.csproj --filter FullyQualifiedName~Architecture
    dotnet build src/BrewUp.slnx
    ```

  - [x] Do not start infrastructure-dependent REST integration fixtures.
    If broader solution tests are run, classify known brownfield
    infrastructure/Purchases failures separately and do not conceal a new
    Payment regression.

- [x] Task 5 — Run the post-GREEN convention and scope gates (AC: 10, 11)
  - [x] Run `Story12ImplementationConventionReview` only after focused and
    full Payment GREEN. Derive the production-file set from the recorded
    baseline; exclude tests, story/gate documents, and unrelated pre-existing
    worktree files.
  - [x] Inspect every GUID creation and every `await` in that set. Correct
    `Guid.NewGuid()` and missing `ConfigureAwait(false)` before rerunning GREEN.
  - [x] Verify `Program.cs`, `BrewUp.Rest.csproj`, and `BrewUp.slnx` retain
    Story 1.1's single valid composition and that no Sales, Warehouse, Sagas,
    real-provider, compensation, or other excluded policy changed.
  - [x] Run the repository harness validators/artifact lint required by FR-019
    if they are part of the implementation handoff; any story-document lint
    must target this exact story path or artifact root without selecting Story
    1.1 by guess.

## Dev Notes

### Current Story 1.1 state and prior-story intelligence

- Story 1.1 is at `status: review`. Commit
  `6932de7482ae799b820ca88e3fa05201b3f5d52e` created the six projects,
  Payment-owned contracts, pending-to-authorized aggregate, handlers, minimal
  helpers, solution membership, and REST composition. Commit
  `7d3cd354b2ab7289b64aa254e94dcb126217980b` strengthened dependency-item
  architecture checks.
- Story 1.1 recorded targeted domain 4/4, architecture 5/5, full Payment 9/9,
  REST architecture 1/1, and a zero-error solution build. Its convention
  review passed 2/2 GUID sites and 4/4 awaits.
- The existing
  `PaymentModuleStructureAndComposition.RestHostComposesPaymentThroughFacadeOnly`
  currently asserts that `MapPaymentEndpoints` is absent. That was correct for
  Story 1.1 and must be deliberately changed to require exactly the Story 1.2
  mapping; do not bypass or delete the architecture test.
- The current Payment ReadModel, Infrastructure, and Facade helpers are empty
  compiling shells. `PaymentModule.Register(...)` already calls
  `AddPaymentFacade(...)`; `PaymentModule.Configure(...)` is currently a no-op.
- Existing SharedKernel types are authoritative and must be reused:
  `PaymentAuthorizationId`, `SalesOrderReference`,
  `RecordPaymentAuthorized`, `PaymentAuthorizationRequested`,
  `PaymentAuthorized`, and `PaymentAuthorizedIntegrationEvent`. Correlation
  remains message/event metadata, not projection authority or aggregate
  business state.
- Story 1.1's aggregate already makes duplicate authorized commands a no-op.
  Story 1.2 must test and preserve that behavior through the callback boundary,
  not add a second idempotency store or provider policy.

### Exact implementation file manifest

`NEW` and `UPDATE` are implementation scope. `VERIFY` files must remain valid
and should not change unless a failing check proves Story 1.1 composition is
incorrect.

| Mode | Path | Required change or preserved state |
|---|---|---|
| UPDATE | `src/Payment/BrewUp.Payment.Domain/DomainHelper.cs` | Register the two existing command handlers; no business logic. |
| UPDATE | `src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj` | Add only repository-pinned dependencies/references required for Shared read-model and Mongo/Muflone patterns. |
| NEW | `src/Payment/BrewUp.Payment.ReadModel/Dtos/PaymentAuthorization.cs` | Rebuildable pending/authorized projection derived from `DtoBase`. |
| NEW | `src/Payment/BrewUp.Payment.ReadModel/Queries/PaymentAuthorizationQueries.cs` | Payment-database query implementation. |
| NEW | `src/Payment/BrewUp.Payment.ReadModel/Services/IPaymentAuthorizationService.cs` | Projection create/update service contract. |
| NEW | `src/Payment/BrewUp.Payment.ReadModel/Services/PaymentAuthorizationService.cs` | Keyed-persister projection service; no authorization decision. |
| NEW | `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizationRequestedEventHandler.cs` | Insert pending projection. |
| NEW | `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizedEventHandler.cs` | Update projection to authorized only. |
| UPDATE | `src/Payment/BrewUp.Payment.ReadModel/PaymentReadModelHelper.cs` | Register query, service, and both projection handlers. |
| UPDATE | `src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj` | Add the shared read-model/Mongo dependencies while preserving AD-4 references. |
| NEW | `src/Payment/BrewUp.Payment.Infrastructure/PaymentPersister.cs` | Payment-keyed Mongo `IPersister`, database `Payment`. |
| UPDATE | `src/Payment/BrewUp.Payment.Infrastructure/InfrastructureHelper.cs` | Register one keyed `IPersister` under `payment`. |
| UPDATE | `src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj` | Add only pinned Muflone/Lena/OpenAPI dependencies required by the boundary. |
| NEW | `src/Payment/BrewUp.Payment.Facade/IPaymentFacade.cs` | Declare `RecordAuthorizedAsync(PaymentAuthorizationId, string, CancellationToken)`. |
| NEW | `src/Payment/BrewUp.Payment.Facade/PaymentFacade.cs` | Send the existing command; return a boundary result; no decision logic. |
| UPDATE | `src/Payment/BrewUp.Payment.Facade/PaymentFacadeHelper.cs` | Compose all owned layers and publisher exactly once. |
| NEW | `src/Payment/BrewUp.Payment.Facade/Endpoints/PaymentEndpoints.cs` | Map the exact `/v1/payment/authorizations/{authorizationId}/authorized` POST route. |
| NEW | `src/Payment/BrewUp.Payment.Facade/ExternalContracts/ProviderAuthorizationJson.cs` | Callback body containing only `ProviderReference`. |
| NEW | `src/Payment/BrewUp.Payment.Facade/EventHandlers/PaymentAuthorizedIntegrationEventPublisher.cs` | Publish the existing Payment integration event from the owned domain outcome. |
| UPDATE | `src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj` | Reference Payment owned layers needed by the four tests; retain test packages. |
| NEW | `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectPaymentAuthorizationRequested.cs` | Contains named test `ProjectPendingAuthorization`. |
| NEW | `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectAndPublishAuthorizedOutcome.cs` | Contains the same-named test and duplicate-publication assertion. |
| NEW | `src/Payment/BrewUp.Payment.Tests/Facade/RecordAuthorizedProviderOutcome.cs` | Contains named test `RecordAuthorizedCallbackBoundary`. |
| NEW | `src/Payment/BrewUp.Payment.Tests/Infrastructure/PaymentPersistenceWiring.cs` | DI/persister/query wiring test with no live infrastructure. |
| UPDATE | `src/Payment/BrewUp.Payment.Tests/Architecture/PaymentModuleStructureAndComposition.cs` | Require endpoint mapping and updated owned-layer test references; preserve all boundary assertions. |
| UPDATE | `src/BrewUp.Rest/Module/PaymentModule.cs` | Map Payment endpoints in `Configure`; retain Facade-only composition. |
| VERIFY | `src/BrewUp.Rest/Program.cs` | One existing `new PaymentModule()`; no Payment logic. |
| VERIFY | `src/BrewUp.Rest/BrewUp.Rest.csproj` | One Payment Facade reference and no other Payment layer. |
| VERIFY | `src/BrewUp.slnx` | Exact six projects remain under `/50 Modules/Payment/`. |

Do not create an optional Payment Entities project, a second integration event,
an application/contracts/core project, a migration, or a new host
infrastructure registration.

### Projection and callback contract

The projection's logical shape is:

```csharp
public sealed class PaymentAuthorization : DtoBase
{
    // Id is inherited from DtoBase.
    public string SalesOrderId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? ProviderReference { get; set; }
}
```

Use repository-compatible mutability/constructors rather than shadowing
`DtoBase.Id`. `PaymentAuthorizationRequestedEventHandler` maps
`AggregateId.Value` and `SalesOrder.Value` and inserts `pending`.
`PaymentAuthorizedEventHandler` loads the same ID, updates only status/provider
reference, and persists it. Neither handler decides authorization.

The endpoint group is `/v1/payment`; map:

```text
POST /authorizations/{authorizationId}/authorized
```

The route ID becomes `PaymentAuthorizationId`; the JSON body supplies only
`ProviderReference`. On a successful Facade result return HTTP 202 Accepted
with a stable `/v1/payment/authorizations/{authorizationId}` location; map
errors to Bad Request using the repository's `Result<T>.Match` pattern.
Endpoint, Facade, projection, and publisher awaits must use
`ConfigureAwait(false)`.

### Persistence and composition boundaries

- `src/BrewUp.Infrastructure/InfrastructureHelper.cs` already registers one
  shared `IMongoClient`, Muflone RabbitMQ transport, and Muflone EventStore
  repository. `InfrastructureModule` runs before business modules. Reuse it.
- Follow the existing keyed module-persister pattern (`warehouse`, `sales`,
  etc.) with key `payment`, but correct Story 1.2 awaits to
  `ConfigureAwait(false)` even where older brownfield examples omit it.
- Payment's Mongo database is a rebuildable projection store. It does not
  authorize payment and is not a transactional substitute for the
  event-sourced aggregate.
- Register `RequestPaymentAuthorizationCommandHandler` and
  `RecordPaymentAuthorizedCommandHandler` through the existing Muflone
  registration extension. Register both projection handlers and the
  integration publisher as domain event handlers.
- One domain event may have a projection handler and a publisher handler.
  Publication must occur once from the Payment-owned `PaymentAuthorized`
  outcome, never directly from HTTP input.

### Architecture and library requirements

- Target `net10.0`; retain repository-pinned Muflone 10.2.1, MongoDB.Driver
  3.8.1, xUnit 2.9.3, NetArchTest.Rules 1.3.2, and other existing package
  versions. Do not opportunistically upgrade dependencies.
- Current MongoDB documentation confirms the async insert/replace APIs and
  cancellation-token support used by the repository pattern. MongoDB.Driver
  3.10 has breaking changes, which reinforces AD-11's no-upgrade boundary for
  this story.
- ASP.NET Core 10 Minimal APIs bind registered services and
  `CancellationToken` in route handlers. Keep endpoint composition in Facade
  and REST mapping in `PaymentModule`; do not move it into `Program.cs`.
- Allowed references remain:

  ```text
  Facade         -> Domain, ReadModel, Infrastructure, SharedKernel
  Domain         -> SharedKernel
  ReadModel      -> SharedKernel (+ repository Shared read-model abstractions)
  Infrastructure -> Domain, SharedKernel (+ repository Shared abstractions)
  Tests          -> owned Payment layers as needed
  REST host      -> Payment Facade only
  ```

### Testing requirements

- The four exact named tests are the mandatory RED-first gate. Two file names
  intentionally retain the Task 3 brief names while the test names retain the
  canonical Story 1.2 names; filters and evidence use the test names.
- Prefer isolated fakes/spies for `IServiceBus`, `IEventBus`, `IPersister`, and
  `IMongoClient` registration inspection. Do not turn the focused suite into a
  live-infrastructure test.
- `ProjectPendingAuthorization` proves exact ID, Sales Order reference,
  `pending`, and null provider reference.
- `RecordAuthorizedCallbackBoundary` proves route/Facade input reaches the
  existing command without manufacturing an outcome or unsupported field.
- `ProjectAndPublishAuthorizedOutcome` proves authorized projection fields,
  one existing integration contract, preserved ID/correlation, and duplicate
  callback publication count of one.
- `PaymentPersistenceWiring` proves one `payment` keyed persister plus query,
  service, handler, Facade, and publisher registrations compose against shared
  infrastructure abstractions.
- GREEN is not complete until focused four, full Payment, REST architecture,
  and solution build all pass. Architecture assertions may not be weakened to
  make new references pass.

### Explicit exclusions and stop conditions

Stop and report rather than invent a workaround if implementation requires:

- deciding whether a payment is authorized, declined, pending, unknown, or
  timed out instead of recording the simulator-supplied authorized outcome;
- adding amount, currency, provider execution, authentication/security policy,
  void, refund, compensation, retry, notification, or expiry behavior;
- changing the existing `PaymentAuthorized` or
  `PaymentAuthorizedIntegrationEvent` contract without upstream approval;
- publishing from the endpoint or ReadModel state rather than the
  Payment-owned domain event;
- introducing a second Mongo client, EventStore repository, RabbitMQ
  transport, Payment module registration, or integration event;
- adding Payment behavior under Sales, Warehouse, Sagas, REST, or another
  module; referencing another module's Domain or Infrastructure;
- modifying Warehouse reservation, Sales confirmation, saga coordination,
  shipment, invoicing, or future-story behavior;
- weakening architecture fitness coverage or accepting a non-meaningful RED;
- choosing between competing story files without the exact canonical path
  named in this document.

### Project structure notes

Story 1.2 completes existing shells; it does not create a seventh Payment
project. The Task 3 brief's two test file names and the epic's four canonical
test names are reconciled explicitly in the file manifest. No UX artifact is
applicable because this is a backend integration boundary.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.2]
- [Source: `_bmad-output/planning-artifacts/prd.md` — FR-001, FR-003,
  FR-004, FR-016–FR-019; NFR-001–NFR-005]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — AD-1 through
  AD-5, AD-9 through AD-11; Seed structure]
- [Source:
  `docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`
  — Payment provider boundary, Payment module, Data and Error Flow, Testing
  Strategy, Out of Scope]
- [Source: `_bmad-output/project-context.md` — Technology and architecture;
  Implementation discipline]
- [Source:
  `.bmad-harness/governance/brewup-sales-order-confirmation.md` — BC-000,
  BC-003, BC-004, BC-008, BC-011]
- [Source: `.bmad-harness/governance/brewup-module-structure.md` — AR-000
  through AR-017]
- [Source:
  `.superpowers/sdd/2026-07-27-brewup-order-confirmation-with-bmad-harness/task-3-brief.md`
  — Task 3 files, interfaces, RED/GREEN order, composition, convention review]
- [Source:
  `_bmad-output/implementation-artifacts/1-1-order-confirmation.md` — completed
  tasks, current review state, debug log, file list, prior learnings]
- [External: Microsoft Learn, ASP.NET Core 10 Minimal API parameter binding,
  `https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-10.0`]
- [External: MongoDB .NET/C# Driver, replace documents and release notes,
  `https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/write-operations/replace/`,
  `https://www.mongodb.com/docs/drivers/csharp/current/reference/release-notes/`]

## Governance traceability

| Story concern | Requirements / decisions | Rules | Acceptance / tests |
|---|---|---|---|
| Payment-owned callback and outcome | FR-001, FR-003, FR-004; AD-1, AD-5 | BC-003, BC-004, BC-008; AR-006–AR-008, AR-017 | AC3–AC5; `RecordAuthorizedCallbackBoundary`, `ProjectAndPublishAuthorizedOutcome` |
| Rebuildable Payment projection | FR-001, FR-003; NFR-003; AD-9 | BC-003, BC-004; AR-009 | AC2, AC4; `ProjectPendingAuthorization`, `ProjectAndPublishAuthorizedOutcome` |
| Persistence and complete composition | FR-016–FR-018; AD-3, AD-4, AD-11 | AR-001, AR-002, AR-010, AR-011, AR-013–AR-016 | AC1, AC6–AC8; `PaymentPersistenceWiring`, `PaymentModuleStructureAndComposition` |
| Duplicate safety | FR-004; NFR-002; AD-5 | BC-003, BC-004; AR-008, AR-012 | AC5; `ProjectAndPublishAuthorizedOutcome` |
| Test-first and conventions | FR-019; NFR-005; AD-10, AD-11 | AR-000, AR-012 | AC9, AC10; mandatory RED, GREEN, architecture suites, `Story12ImplementationConventionReview` |
| Excluded/unresolved policy | PRD Out of scope; AD-11 | BC-000, BC-003, BC-011 | AC11; explicit stop conditions |

## Completion status

Ultimate context engine analysis completed - comprehensive developer guide
created. Story is ready-for-dev only after the strict story guard returns
`PASS` for this exact path.

## Dev Agent Record

### Agent Model Used

GPT-5.6 Codex

### Debug Log References

- Baseline: `2dad5dd54851dff8a74a1bc179d2b832ee553533`.
- Mandatory RED: the exact four-test filter reached all Payment projects and
  failed compilation on absent Story 1.2 DTO, service, handler, endpoint, and
  publisher types. No restore, syntax, project, or infrastructure failure
  contributed to the accepted RED.
- Focused GREEN: 4/4 canonical tests passed.
- Full Payment GREEN: 23/23 tests passed.
- REST architecture GREEN: 1/1 passed with `--no-build --no-restore`; no
  infrastructure-dependent REST fixture was started for the required gate.
- Solution build: succeeded with 0 errors and 174 existing package-audit and
  brownfield compiler warnings after approved NuGet configuration access.
- Broader no-build solution test classification: Payment remained 23/23;
  the known Purchases `SubmitPurchaseOrderSuccessfully` baseline failed with
  `NotImplementedException`; other reported module suites passed.
- `Story12ImplementationConventionReview`: one GUID site used
  `Guid.CreateVersion7()` and all 16 await sites used
  `ConfigureAwait(false)`. Host composition files retained Story 1.1 state,
  with one `PaymentModule` registration and one endpoint mapping.
- The exact Story 1.2 strict story guard was already `PASS`. Static installed
  core cleanup/full harness validation remains deliberately deferred to Task 7
  by the governing implementation plan.
- Review-fix RED: the strengthened exact Story 1.2 filter ran 8 tests with
  3 expected failures: both projection handlers acknowledged unsuccessful
  persistence and a zero-match Mongo replacement returned success. The mapped
  callback and full scoped DI-resolution tests passed against existing
  production behavior, proving those findings were test-coverage gaps.
- Review-fix GREEN: exact Story 1.2 filter 8/8, full Payment 27/27, REST
  architecture 1/1, and solution build 0 errors. `git diff --check` passed.
- Review-fix `Story12ImplementationConventionReview`: the 17 Task 3 production
  files retained one UUIDv7 site and all 16 awaited tasks use
  `ConfigureAwait(false)`.

### Completion Notes List

- Added the rebuildable pending/authorized Payment authorization projection,
  Mongo query/persister, projection services, and owned event handlers.
- Added the provider-simulator authorized callback and Facade command boundary
  without provider decision, amount/currency, timeout, retry, void/refund, or
  compensation policy.
- Published the existing `PaymentAuthorizedIntegrationEvent` only from the
  owned domain event and preserved correlation metadata.
- Preserved duplicate callback idempotency through the existing aggregate
  guard: the second callback emits no domain event, projection update, or
  integration publication.
- Completed keyed `payment` persistence and all owned DI/endpoint composition
  while reusing host-global Mongo, EventStore, and RabbitMQ registrations.
- Preserved BC-003/BC-004/BC-008/BC-011 and
  AR-001/AR-002/AR-009–AR-017 boundaries.
- Projection handlers now propagate failed or false persistence results as
  Muflone `PersistenceException`; Mongo replacement reports an error when the
  write is unacknowledged or the projection has vanished. No retry was added.
- The callback boundary test now invokes the mapped `RequestDelegate` and
  verifies route/body propagation, HTTP 202 location/result, and HTTP 400
  error mapping.
- Persistence composition now proves exact scoped registrations, builds with
  scope validation and fake host-global dependencies, and resolves the keyed
  persister, service, Facade, command handlers, projection handlers, and
  integration publisher without live infrastructure.
- Projection and integration-publication handlers remain independent event
  consumers; no transactional publication ordering or coupling was invented.

### File List

- `_bmad-output/implementation-artifacts/1-2-complete-payment-projection-and-provider-callback-behavior.md`
- `src/BrewUp.Rest/Module/PaymentModule.cs`
- `src/Payment/BrewUp.Payment.Domain/DomainHelper.cs`
- `src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj`
- `src/Payment/BrewUp.Payment.Facade/Endpoints/PaymentEndpoints.cs`
- `src/Payment/BrewUp.Payment.Facade/EventHandlers/PaymentAuthorizedIntegrationEventPublisher.cs`
- `src/Payment/BrewUp.Payment.Facade/ExternalContracts/ProviderAuthorizationJson.cs`
- `src/Payment/BrewUp.Payment.Facade/IPaymentFacade.cs`
- `src/Payment/BrewUp.Payment.Facade/PaymentFacade.cs`
- `src/Payment/BrewUp.Payment.Facade/PaymentFacadeHelper.cs`
- `src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj`
- `src/Payment/BrewUp.Payment.Infrastructure/InfrastructureHelper.cs`
- `src/Payment/BrewUp.Payment.Infrastructure/PaymentPersister.cs`
- `src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj`
- `src/Payment/BrewUp.Payment.ReadModel/Dtos/PaymentAuthorization.cs`
- `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizationRequestedEventHandler.cs`
- `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizedEventHandler.cs`
- `src/Payment/BrewUp.Payment.ReadModel/PaymentReadModelHelper.cs`
- `src/Payment/BrewUp.Payment.ReadModel/Queries/PaymentAuthorizationQueries.cs`
- `src/Payment/BrewUp.Payment.ReadModel/Services/IPaymentAuthorizationService.cs`
- `src/Payment/BrewUp.Payment.ReadModel/Services/PaymentAuthorizationService.cs`
- `src/Payment/BrewUp.Payment.Tests/Architecture/PaymentModuleStructureAndComposition.cs`
- `src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj`
- `src/Payment/BrewUp.Payment.Tests/Facade/RecordAuthorizedProviderOutcome.cs`
- `src/Payment/BrewUp.Payment.Tests/Infrastructure/PaymentPersistenceWiring.cs`
- `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectAndPublishAuthorizedOutcome.cs`
- `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectPaymentAuthorizationRequested.cs`

## Change Log

- 2026-07-27: Completed Story 1.2 Payment projection, keyed persistence,
  provider callback, integration publication, composition, architecture
  coverage, and RED/GREEN verification.
- 2026-07-27: Addressed three Important review findings with persistence
  failure propagation, mapped callback execution coverage, and validated
  scoped DI resolution.
