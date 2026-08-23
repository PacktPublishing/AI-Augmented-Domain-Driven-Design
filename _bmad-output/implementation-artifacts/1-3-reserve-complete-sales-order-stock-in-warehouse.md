---
title: "Story 1.3: Reserve Complete Sales Order Stock in Warehouse"
storyId: "1.3"
storyKey: "1-3-reserve-complete-sales-order-stock-in-warehouse"
status: review
created: 2026-07-27
updated: 2026-07-27
epic: "Evidence-Backed Sales Order Confirmation"
baseline_commit: 8ca284c00a08bcb1a8375369f341d830941280f6
---

# Story 1.3: Reserve Complete Sales Order Stock in Warehouse

Status: review

## Story

As a Sales Order process,
I want Warehouse to reserve every requested row or none,
so that confirmation evidence never represents a partial physical-stock
decision.

## Acceptance Criteria

1. Given every requested beer and quantity is available, when the Warehouse ACL
   assesses every input row in order and sends one complete `ReserveStock`,
   then the Warehouse-owned `StockReservation` aggregate emits exactly one
   `StockReserved` containing the stable `StockReservationId`, Warehouse ID,
   Sales Order ID, and every assessed row in original order.
2. Given multiple rows use the same `(WarehouseId, BeerId,
   UnitOfMeasure)`, when the aggregate decides the request, then it compares the
   cumulative ordered quantity with one Warehouse-owned availability fact.
   Individually sufficient rows whose cumulative total exceeds availability
   produce one `StockReservationFailed`, never `StockReserved`.
3. Given any row has no availability, has less availability than the cumulative
   requested quantity, or has an available unit different from its ordered
   unit, when the complete request is decided, then the aggregate emits exactly
   one `StockReservationFailed` and zero reserved rows. No success event is
   emitted for any row.
4. Given the same stable `StockReservationId` has already reached either the
   reserved or failed terminal outcome, when `ReserveStock` is replayed, then
   the aggregate emits no second domain event, no second integration outcome,
   and availability is not subtracted again.
5. Given a `StockReservationRequestedIntegrationEvent` with multiple rows,
   including a missing row before or between available rows, when the new
   Warehouse ACL handles it, then it calls the Warehouse-owned availability
   service for every row without early return, maps a missing result to
   available quantity zero in that row's ordered unit, preserves input order,
   and dispatches exactly one `ReserveStock` only after all assessments finish.
   The ACL makes no availability or reservation decision and publishes no
   success/failure outcome itself.
6. Given reservation domain outcomes are handled, when projections and
   integration publishers run, then `StockReserved` creates one successful
   reservation projection and publishes one
   `StockReservedIntegrationEvent`; `StockReservationFailed` creates one failed
   projection with no reserved rows and publishes one
   `StockReservationFailedIntegrationEvent`. IDs, original row order, failure
   reason, and correlation metadata are preserved by the matching contracts.
7. Given on-hand availability exists and successful/failed reservation
   projections exist, when availability is queried, then Warehouse returns
   `on-hand - sum(matching successful reservation quantities)` exactly once per
   stable reservation. Matching uses Warehouse, beer, and unit of measure.
   Failed reservations subtract zero. Reservation handlers never mutate the
   on-hand `Availability` aggregate or document.
8. Given the Story 1.3 contracts are inspected, then
   `StockReservationId`, `ReserveStock`, both domain outcomes, the request
   integration event, and both outcome integration events live only in
   `BrewUp.Warehouse.SharedKernel`. Reservation decision behavior lives only in
   Warehouse Domain; ACL and integration publication live in Warehouse Facade;
   projections and availability subtraction live in Warehouse ReadModel.
9. Given Story 1.3 begins, then the twelve named tests below are written before
   any Story 1.3 production change and run together to meaningful RED caused by
   absent reservation behavior/contracts. After minimum implementation, the
   same set, the full Warehouse suite, and Warehouse architecture tests are
   GREEN. Existing baseline Warehouse tests remain 6/6.
10. Given Story 1.3 production files are GREEN, when
    `Story13ImplementationConventionReview` examines only the changed
    production-file set relative to the recorded baseline, then every new GUID
    uses `Guid.CreateVersion7()` and every awaited task uses
    `ConfigureAwait(false)`, unless an explicit architecture approval records
    an exception.
11. Existing Warehouse stock-addition, availability creation, shipment
    preparation, REST endpoints, legacy availability-check handler, module
    registration, keyed `warehouse` persister, and shipment-trigger timing
    remain behaviorally unchanged. Story 1.3 adds no Sales decision or mutation.
12. No release, reservation expiration, retry, partial reservation,
    compensation, void/refund, notification, stronger cross-order concurrency,
    shipment, invoicing, unit conversion, or unrelated refactoring is
    introduced.

## Tasks / Subtasks

- [x] Task 1 — Record baseline and write all RED-first tests (AC: 1–10)
  - [x] Record `git rev-parse HEAD`, the unchanged Story 1.3 production-file
    set, and the current full Warehouse result (baseline: 6/6 at
    `8ca284c00a08bcb1a8375369f341d830941280f6`).
  - [x] Before changing production code, create these exact tests:
    `ReserveAllStockSuccessfully`,
    `RejectReservationWhenBeerMissing`,
    `RejectReservationWhenQuantityInsufficient`,
    `RejectReservationWhenUnitOfMeasureMismatches`,
    `DoNotReserveStockTwice`,
    `RejectCumulativeDuplicateRowsBeyondAvailability`,
    `AssessEveryReservationRowBeforeDispatch`,
    `ProjectSuccessfulReservationOnce`,
    `DoNotProjectFailedReservationAsAvailability`,
    `PublishReservationIntegrationOutcomes`,
    `WarehouseReservationContractsStayInSharedKernel`, and
    `Story13ImplementationConventionReview`.
  - [x] `ReserveAllStockSuccessfully` asserts one event with all rows in their
    exact input order, not merely equivalent grouped rows.
  - [x] `RejectCumulativeDuplicateRowsBeyondAvailability` uses at least two
    individually sufficient duplicate rows whose sum exceeds the single
    availability fact.
  - [x] `DoNotReserveStockTwice` seeds successful history and replays the same
    stable ID; add the failed-history replay assertion in the same test class
    or a second fact with the same exact test-class name.
  - [x] Run one exact filter containing all twelve names. Accept compile RED
    only for the deliberately absent Story 1.3 contracts/types; reject
    restore, syntax, project, or infrastructure failure as non-meaningful RED.

- [x] Task 2 — Add Warehouse-owned reservation contracts (AC: 1–8)
  - [x] Create `StockReservationId(string value) : DomainId` under Warehouse
    SharedKernel `DomainIds/`.
  - [x] Create `ReserveStock` with aggregate/reservation ID, `WarehouseId`,
    Warehouse-owned `SalesOrderId`, a materialized ordered
    `IReadOnlyList<ItemRequested>`, and correlation ID.
  - [x] Create `StockReservationRequestedIntegrationEvent` with the same
    stable reservation ID, Warehouse ID, Sales Order ID, ordered rows, and
    correlation ID. It is an integration request to Warehouse, not a stock
    decision.
  - [x] Create `StockReserved` and
    `StockReservedIntegrationEvent` carrying the reservation ID, Warehouse ID,
    Sales Order ID, and all original ordered assessed rows.
  - [x] Create `StockReservationFailed` and
    `StockReservationFailedIntegrationEvent` carrying the reservation ID,
    Warehouse ID, Sales Order ID, the exact stable failure reason
    `"All requested stock is not available."`, and correlation ID. They expose
    no reserved-row collection.
  - [x] Do not create reservation contracts under `BrewUp.Shared`, Sales,
    Sagas, the REST host, or Warehouse Domain.

- [x] Task 3 — Implement the all-or-nothing aggregate and handler (AC: 1–4, 8)
  - [x] Create an event-sourced `StockReservation` aggregate. Its empty state
    accepts one decision; applying either terminal event restores its ID and
    terminal state so replay is a no-op.
  - [x] Materialize input exactly once. Reject an empty request only if the
    existing upstream contract/test requires it; otherwise stop because empty
    reservation policy is not defined.
  - [x] First validate that every assessed row has matching ordered/available
    units. Then group by canonical key
    `(WarehouseId from command, BeerId, QuantityOrdered.UnitOfMeasure)`, sum
    ordered quantity per group, and compare with the single availability fact
    supplied for that key. The Task 4 row-by-row pseudocode is not sufficient
    for duplicate rows and must not be copied as the final decision.
  - [x] Emit one failure if any group fails; otherwise emit one success with the
    untouched ordered array. Never emit per-row events or reorder/group the
    outcome rows.
  - [x] `ReserveStockCommandHandler` performs only
    `load-or-create -> aggregate.Reserve(...) -> save`; it contains no
    availability query or policy and uses the stable command ID.
  - [x] Register the handler exactly once in `DomainHelper`.

- [x] Task 4 — Assess every row in the Warehouse ACL (AC: 1, 2, 5, 8, 11)
  - [x] Implement `StockReservationRequestedIntegrationEventHandler` as a new
    handler; do not repurpose or modify
    `RequestBeerAvailablityRaisedEventHandler`.
  - [x] Iterate the request rows in input order. Await
    `GetAvailabilityByWarehouseIdAndBeerIdAsync` for every row with
    `ConfigureAwait(false)`, recording zero in the ordered unit when lookup
    fails and recording the factual on-hand/remaining quantity when it
    succeeds.
  - [x] Do not return, break, dispatch, publish, or decide inside the loop.
    After the loop, send exactly one `ReserveStock` with the same stable
    reservation ID, Warehouse ID, Sales Order ID, ordered assessed rows, and
    correlation ID.
  - [x] Register the ACL once in `WarehouseFacadeHelper`; preserve all four
    existing ACL registrations and the existing Facade composition.

- [x] Task 5 — Project reservation outcomes and subtract exactly once (AC: 4,
      6, 7, 11)
  - [x] Add a rebuildable `StockReservation` DTO keyed by reservation ID with
    Warehouse ID, Sales Order ID, terminal status, and ordered rows. Reserved
    projections contain the original rows; failed projections contain an empty
    reserved-row list and the stable failure reason.
  - [x] `StockReservedEventHandler` and
    `StockReservationFailedEventHandler` persist only the corresponding
    projection. Propagate unsuccessful/false persistence results as failures;
    do not acknowledge failed persistence as success and do not add retry.
  - [x] Add `StockReservationQueries`,
    `IStockReservationService`, and `StockReservationService` to retrieve
    successful reservations relevant to Warehouse/beer/unit queries.
  - [x] Extend `AvailabilityService`, without changing the on-hand
    `Availability` DTO, so both availability lookup methods return on-hand
    minus matching rows from successful reservation documents. Do the
    subtraction in exactly one layer; do not also decrement on-hand or subtract
    in both query and service.
  - [x] Register reservation queries/services in both `AddReadModel` and
    `AddReadModelForMcp` so existing consumers receive the same remaining
    availability semantics. Register both projection handlers only in the
    normal domain-event composition.

- [x] Task 6 — Publish owned integration outcomes (AC: 4, 6, 8)
  - [x] Add separate Facade domain-event publishers for `StockReserved` and
    `StockReservationFailed`, following the completed Payment publisher
    pattern. They translate the owned domain outcome to the one matching
    existing Warehouse integration contract and preserve correlation with
    `MessageHelpers.GetCorrelationId`.
  - [x] Register each publisher exactly once in `WarehouseFacadeHelper`.
    Projection handlers do not publish, and the ACL never manufactures an
    outcome.
  - [x] Prove success emits only `StockReservedIntegrationEvent`, failure emits
    only `StockReservationFailedIntegrationEvent`, and a duplicate command
    produces neither outcome again.

- [x] Task 7 — Verify placement, GREEN, regressions, and conventions (AC: 8–12)
  - [x] Run the exact twelve-test Story 1.3 filter from Task 1.
  - [x] Run
    `dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj`.
  - [x] Run Warehouse architecture tests and build `src/BrewUp.slnx`.
    Architecture assertions must not be weakened to make new dependencies pass.
  - [x] Run `Story13ImplementationConventionReview` only after Warehouse GREEN,
    against the recorded changed production-file set.
  - [x] Confirm the existing six baseline Warehouse tests remain present and
    green; confirm no existing Warehouse endpoint, shipment, stock-addition,
    legacy ACL, or REST composition file changed unless a failing Story 1.3
    check proves the change necessary.
  - [x] Confirm one reservation projection per stable ID; remaining
    availability subtracts a successful reservation once and a failed
    reservation zero times.

### Review Findings

- [ ] [Review][Decision] Define behavior for inconsistent duplicate
  availability facts — Duplicate canonical rows can carry different
  `QuantityAvailable` values because the ACL assesses each row separately.
  Selecting the first value makes the decision input-order-dependent, while
  the story explicitly requires stopping instead of inventing reconciliation
  behavior. Decide whether inconsistent facts must fail, abort without a
  terminal outcome, or use another explicitly owned rule.
- [ ] [Review][Decision] Define behavior for an empty reservation request —
  the aggregate currently emits `StockReserved` for an empty row collection,
  but the story explicitly says to stop when no upstream non-empty invariant
  is proven rather than inventing an empty-request policy. Decide whether
  empty input must fail, abort without a terminal outcome, or be accepted.
- [ ] [Review][Patch] Filter successful reservation projections by matching
  beer and unit before materialization
  [`src/Warehouse/BrewUp.Warehouse.ReadModel/Services/StockReservationService.cs`:52]

## Dev Notes

### Developer context

Warehouse is the sole authority for physical-stock availability and
reservation. The incoming integration event requests that authority to act.
The new ACL enriches every row with Warehouse-owned facts but makes no
success/failure decision. The event-sourced aggregate makes one atomic
all-or-none decision and is the only source of reservation outcomes.

This story produces Warehouse evidence for later saga/Sales stories. It does
not wire the saga, confirm a Sales Order, disconnect the legacy
availability-to-acceptance shortcut, or alter shipment timing. Those are Story
1.5 concerns.

### Current Warehouse state to preserve

- Full Warehouse tests are currently 6/6. Existing specifications cover
  availability creation, stock addition, shipment creation, and architecture.
- `Availability` is the on-hand aggregate/document. `ItemStockAdded` replaces
  its projected quantity with the new on-hand total. Reservation code must not
  call `AddItemStock`, raise `ItemStockAdded`, or update that DTO.
- `AvailabilityService.GetAvailabilityByWarehouseIdAndBeerIdAsync` and
  `GetAvailabilityByBeerIdAsync` currently return raw on-hand values. This
  story changes only their returned quantity calculation by subtracting
  successful reservation projections once.
- `RequestBeerAvailablityRaisedEventHandler` is a legacy handler that currently
  publishes the old availability-check outcome. Preserve it unchanged; the new
  complete reservation request has its own ACL. Story 1.5 owns disconnecting
  the legacy confirmation shortcut.
- `SalesOrderCreatedIntegrationEventHandler` currently creates a shipment
  preparation command, and `WarehouseFacadeHelper` registers it. Preserve that
  registration and timing.
- `WarehouseFacade`, `IWarehouseFacade`, `WarehouseEndpoints`,
  `WarehouseModule`, `WarehousePersister`, and the keyed `warehouse` service
  remain in place. Do not add a second Mongo client, persister key, REST module,
  or endpoint.
- Older Warehouse files contain `Guid.NewGuid()` and awaits without
  `ConfigureAwait(false)`. `Story13ImplementationConventionReview` is scoped to
  Story 1.3 changed production files; do not expand into unrelated cleanup.

### Exact file manifest

| Mode | Path | Required change or preserved state |
|---|---|---|
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/DomainIds/StockReservationId.cs` | Warehouse-owned strongly typed ID. |
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs` | Complete assessed request command. |
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationRequestedIntegrationEvent.cs` | Incoming Warehouse request contract. |
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReserved.cs` | Successful domain outcome with ordered rows. |
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationFailed.cs` | Failed domain outcome with no reserved rows. |
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservedIntegrationEvent.cs` | Warehouse success integration outcome. |
| NEW | `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationFailedIntegrationEvent.cs` | Warehouse failure integration outcome. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Domain/Entities/StockReservation.cs` | Event-sourced cumulative all-or-none decision. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Domain/CommandHandlers/ReserveStockCommandHandler.cs` | Load/call/save only. |
| UPDATE | `src/Warehouse/BrewUp.Warehouse.Domain/DomainHelper.cs` | Register the new handler once; preserve three existing registrations. |
| NEW | `src/Warehouse/BrewUp.Warehouse.ReadModel/Dtos/StockReservation.cs` | Rebuildable successful/failed reservation projection. |
| NEW | `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservedEventHandler.cs` | Project success only. |
| NEW | `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservationFailedEventHandler.cs` | Project failure only. |
| NEW | `src/Warehouse/BrewUp.Warehouse.ReadModel/Queries/StockReservationQueries.cs` | Query projected reservation documents. |
| NEW | `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/IStockReservationService.cs` | Projection/query service contract. |
| NEW | `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/StockReservationService.cs` | Persist/query projection; no stock decision. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/IAvailabilityService.cs` | Preserve the existing public signatures; the returned quantity semantics change in the implementation only. |
| UPDATE | `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/AvailabilityService.cs` | Subtract matching successful reservations exactly once. |
| UPDATE | `src/Warehouse/BrewUp.Warehouse.ReadModel/ReadModelHelper.cs` | Register reservation services, queries, and projection handlers. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Facade/Acl/StockReservationRequestedIntegrationEventHandler.cs` | Full ordered assessment and one command dispatch. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Facade/EventHandlers/StockReservedIntegrationEventPublisher.cs` | Publish success outcome only. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Facade/EventHandlers/StockReservationFailedIntegrationEventPublisher.cs` | Publish failure outcome only. |
| UPDATE | `src/Warehouse/BrewUp.Warehouse.Facade/WarehouseFacadeHelper.cs` | Register ACL and both publishers once. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Domain/ReserveAllStockSuccessfully.cs` | Success and exact ordered-row specification. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenBeerMissing.cs` | Missing availability fails all. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenQuantityInsufficient.cs` | Insufficient quantity fails all. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenUnitOfMeasureMismatches.cs` | Unit mismatch fails all; no conversion. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Domain/DoNotReserveStockTwice.cs` | Stable-ID success and failure replay no-op. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectCumulativeDuplicateRowsBeyondAvailability.cs` | Canonical-key cumulative guard. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Facade/AssessEveryReservationRowBeforeDispatch.cs` | No early return; one ordered command. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/ReadModel/ProjectSuccessfulReservationOnce.cs` | Success projection and one subtraction. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/ReadModel/DoNotProjectFailedReservationAsAvailability.cs` | Failure projection subtracts zero. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Facade/PublishReservationIntegrationOutcomes.cs` | Matching outcome publication and duplicate safety. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Architecture/WarehouseReservationContractsStayInSharedKernel.cs` | Contract placement, unchanged project/solution/module placement, Facade-only REST composition, and no new forbidden dependency assertions. |
| NEW | `src/Warehouse/BrewUp.Warehouse.Tests/Architecture/Story13ImplementationConventionReview.cs` | Post-GREEN changed-file convention check. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj` | Existing packages/references are sufficient; add only direct owned-layer references if compilation requires them. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.Tests/Architecture/WarehouseArchitectureTests.cs` | Existing architecture tests stay green and are not weakened. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.Domain/BrewUp.Warehouse.Domain.csproj` | Add no dependency for reservation behavior; new domain code uses owned contracts only. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.ReadModel/BrewUp.Warehouse.ReadModel.csproj` | Reuse current SharedKernel/Shared/Mongo dependencies; no upgrade. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.Facade/BrewUp.Warehouse.Facade.csproj` | Reuse current owned-layer references; no cross-module Domain/Infrastructure reference. |
| VERIFY | `src/Warehouse/BrewUp.Warehouse.Infrastructure/InfrastructureHelper.cs` | One existing keyed `warehouse` persister; unchanged. |
| VERIFY | `src/BrewUp.Rest/Module/WarehouseModule.cs` | Existing module/endpoint composition; unchanged. |

Any additional production file is out of scope unless a named failing test
proves it is required. Record the evidence before adding it.

### Contract and decision shape

Use stable IDs from the incoming request. Do not create a second reservation ID
inside Warehouse. Materialize rows in contract constructors so repeated
enumeration cannot change their content or order.

The aggregate algorithm is:

```text
if terminal -> no-op
materialize rows preserving order
if any ordered/available unit mismatch -> one failure
group ordered rows by (command.WarehouseId, BeerId, ordered unit)
for each group compare Sum(ordered) with one supplied availability fact
if any cumulative total is unavailable -> one failure
else -> one success containing the original ordered rows
```

`QuantityAvailable` is factual enrichment, not durable reserved stock. The
successful projection is the durable reservation record. No availability DTO
or aggregate quantity changes during reservation.

### Projection/query semantics

- Store one reservation document under `StockReservationId`.
- Successful status is the only status included in availability subtraction.
- Failed documents contain no reserved rows and therefore subtract zero.
- For a requested `(WarehouseId, BeerId)`, first obtain on-hand availability
  and its unit. Sum ordered values from successful reservation rows matching
  that Warehouse ID, Beer ID, and exact unit, then subtract once.
- Do not subtract `QuantityAvailable`; it is the pre-decision fact attached by
  the ACL. Subtract `QuantityOrdered`.
- Do not decrement on-hand when projecting success. Do not subtract again in
  `AvailabilityQueries`, the ACL, the aggregate, or Facade.
- No release/expiry exists, so every successful Story 1.3 reservation remains
  active for this increment. Do not add lifecycle fields or timers.
- No clamp, locking, transaction, or stronger cross-order concurrency guarantee
  is approved. If implementation requires choosing such behavior, stop.

### Test commands and evidence

The RED/GREEN filter must name every Story 1.3 gate:

```powershell
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj --filter "FullyQualifiedName~ReserveAllStockSuccessfully|FullyQualifiedName~RejectReservationWhenBeerMissing|FullyQualifiedName~RejectReservationWhenQuantityInsufficient|FullyQualifiedName~RejectReservationWhenUnitOfMeasureMismatches|FullyQualifiedName~DoNotReserveStockTwice|FullyQualifiedName~RejectCumulativeDuplicateRowsBeyondAvailability|FullyQualifiedName~AssessEveryReservationRowBeforeDispatch|FullyQualifiedName~ProjectSuccessfulReservationOnce|FullyQualifiedName~DoNotProjectFailedReservationAsAvailability|FullyQualifiedName~PublishReservationIntegrationOutcomes|FullyQualifiedName~WarehouseReservationContractsStayInSharedKernel|FullyQualifiedName~Story13ImplementationConventionReview"
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj --filter FullyQualifiedName~Architecture
dotnet build src/BrewUp.slnx
```

Use `CommandSpecification<ReserveStock>` for aggregate behavior. Boundary and
projection tests may use isolated fakes for `IServiceBus`, `IEventBus`,
`IAvailabilityService`, `IPersister`, and `IQueries<T>`; do not require live
MongoDB, EventStore, or RabbitMQ for the focused suite.

### Architecture and library requirements

- Keep `net10.0`, Muflone 10.2.1, MongoDB.Driver 3.8.1, xUnit 2.9.3,
  Muflone.SpecificationTests 10.0.0, and NetArchTest.Rules 1.3.2. No package
  upgrade is part of Story 1.3.
- Use `Guid.CreateVersion7()` for any genuinely new technical GUID; stable
  reservation and correlation IDs come from contracts.
- Every Story 1.3 await uses `ConfigureAwait(false)`.
- MongoDB's current C# driver documentation confirms that strongly typed LINQ
  supports `Where`, `SelectMany`, and `Sum`; keep the query simple and covered
  because unsupported expression translations fail at runtime. The pinned
  repository driver remains authoritative for compilation.
- Cross-context consumers may reference Warehouse SharedKernel messages only.
  No Story 1.3 file may reference Sales, Payment, or Sagas Domain or
  Infrastructure.

### Previous-story and Git intelligence

- Story 1.1 established stable-ID, event-sourced, load/call/save handlers and
  meaningful RED before production behavior.
- Story 1.2 established separate projection and integration-publisher handlers,
  keyed persister reuse, failure propagation from projection persistence, and
  isolated DI/boundary testing. Reuse these patterns rather than coupling
  publication to projection success or creating infrastructure.
- Recent Payment review fixes require projection handlers to reject
  unsuccessful/false persistence results; a handler must not silently
  acknowledge projection loss.
- The current baseline is commit
  `8ca284c00a08bcb1a8375369f341d830941280f6`; the preceding relevant commits are
  `39c52e4` (complete Payment module behavior), `2dad5dd` (Story 1.2
  preparation), `7d3cd35` (dependency-item architecture coverage), and
  `6932de7` (Payment authorization domain).

### Explicit exclusions and stop conditions

Stop and report rather than invent a workaround if implementation requires:

- partial reservation, per-row success, or changing original row order;
- release, expiry, cancellation, retry, compensation, void/refund,
  notification, timeout, or reservation-lifecycle policy;
- unit conversion or choosing how inconsistent duplicate-row facts are
  reconciled;
- an empty-request policy, stronger cross-order locking/transactions, or
  availability clamping not defined upstream;
- Sales, Payment, Sagas, or REST becoming stock/reservation authority;
- changing the legacy availability-to-acceptance shortcut, saga ordering,
  Sales confirmation, shipment trigger/timing, invoicing, or endpoints;
- a new Mongo client, EventStore repository, RabbitMQ transport, persister key,
  module, endpoint, infrastructure migration, or dependency upgrade;
- publication from the ACL or read-model state instead of the owned Warehouse
  domain outcome;
- weakening architecture tests, accepting non-meaningful RED, or modifying
  governance to make implementation pass.

### Project structure notes

Story 1.3 extends the existing six-project-plus-optional-Entities Warehouse
module; it creates no new project and does not move existing types. No UX
artifact exists or applies because this is a backend domain/integration change.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.3]
- [Source: `_bmad-output/planning-artifacts/prd.md` — FR-005–FR-008,
  FR-018–FR-019; NFR-001–NFR-005; AC2, AC4, AC5]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — AD-1, AD-2,
  AD-4, AD-6, AD-9 through AD-11]
- [Source:
  `docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`
  — Confirmed Domain Decisions, Warehouse reservation, Data and Error Flow,
  Testing Strategy, Out of Scope]
- [Source:
  `docs/superpowers/plans/2026-07-27-brewup-order-confirmation-with-bmad-harness.md`
  — Task 4]
- [Source:
  `.superpowers/sdd/2026-07-27-brewup-order-confirmation-with-bmad-harness/task-4-brief.md`
  — files, interfaces, RED/GREEN sequence, ACL, projection, convention review]
- [Source: `_bmad-output/project-context.md` — technology, module boundaries,
  implementation discipline]
- [Source:
  `.bmad-harness/governance/brewup-sales-order-confirmation.md` — BC-000,
  BC-005 through BC-008, BC-011]
- [Source: `.bmad-harness/governance/brewup-module-structure.md` — AR-000,
  AR-004 through AR-012, AR-015, AR-017]
- [Source:
  `_bmad-output/implementation-artifacts/1-2-complete-payment-projection-and-provider-callback-behavior.md`
  — projection/publisher separation, keyed persistence, failure propagation,
  boundary tests, convention review]
- [External: Microsoft Learn, `Guid.CreateVersion7`,
  `https://learn.microsoft.com/dotnet/api/system.guid.createversion7?view=net-10.0`]
- [External: MongoDB .NET/C# Driver, LINQ aggregation,
  `https://www.mongodb.com/docs/drivers/csharp/current/aggregation/linq/`]
- [External: MongoDB .NET/C# Driver, query filters and array operators,
  `https://www.mongodb.com/docs/drivers/csharp/current/crud/query/query-filter/`]

## Governance traceability

| Story concern | Requirements / decisions | Rules | Acceptance / tests |
|---|---|---|---|
| Warehouse-owned complete decision | FR-005, FR-006; AD-1, AD-6 | BC-005–BC-008; AR-007, AR-008 | AC1–AC3; six domain specifications |
| Stable-ID duplicate safety | FR-008; NFR-002; AD-6 | BC-005–BC-008; AR-008, AR-012 | AC4; `DoNotReserveStockTwice` |
| Full ordered ACL assessment | FR-005; AD-6 | BC-005–BC-008; AR-011, AR-017 | AC5; `AssessEveryReservationRowBeforeDispatch` |
| Projection and exactly-once subtraction | FR-007; NFR-003; AD-9 | BC-005, BC-007; AR-009 | AC6, AC7; `ProjectSuccessfulReservationOnce`, `DoNotProjectFailedReservationAsAvailability` |
| Owned integration outcomes | FR-006, FR-018; AD-2, AD-6 | BC-005–BC-008; AR-007, AR-011, AR-017 | AC6, AC8; `PublishReservationIntegrationOutcomes` |
| Contract placement and dependencies | FR-018; NFR-001; AD-4, AD-10 | AR-004–AR-007, AR-010–AR-012, AR-015, AR-017 | AC8; `WarehouseReservationContractsStayInSharedKernel`, Warehouse architecture suite |
| Test-first and conventions | FR-019; NFR-005; AD-10, AD-11 | AR-000, AR-012 | AC9, AC10; mandatory RED, focused/full GREEN, `Story13ImplementationConventionReview` |
| Compatibility and exclusions | NFR-004; AD-11; PRD exclusions | BC-000, BC-011; AR-000 | AC11, AC12; baseline regression and scope review |

## Completion status

Ultimate context engine analysis completed - comprehensive developer guide
created. Story is ready-for-dev only after the strict story guard returns
`PASS` for this exact path.

## Dev Agent Record

### Agent Model Used

GPT-5.6 Codex

### Implementation Plan

- Establish the complete twelve-name Story 1.3 behavior and boundary test set
  before touching Warehouse production code, then accept only absent-contract
  compiler RED.
- Add producer-owned SharedKernel contracts, implement the terminal
  all-or-nothing aggregate and load/call/save handler, then add the ordered ACL.
- Project terminal outcomes separately from integration publication and
  calculate remaining availability from successful reservation projections in
  one ReadModel layer.
- Verify focused behavior, full Warehouse regressions, architecture, solution
  build, conventions, preserved files, and known full-suite baselines before
  moving the story to review.

### Debug Log References

- Implementation workspace started at
  `c1aedfb01912df23b6fa6c6c4f509760098f8491`; the story's recorded production
  comparison baseline remains
  `8ca284c00a08bcb1a8375369f341d830941280f6`. The intervening commit changed
  documentation only; `src/Warehouse` had no delta.
- Baseline Warehouse suite: 6/6 passed. Package audit and existing nullable
  warnings were present before Story 1.3.
- Mandatory RED: after correcting two test-only namespace imports, the exact
  twelve-name filter restored and built every existing Warehouse layer, then
  failed compilation only on deliberately absent Story 1.3 types including
  `StockReservationId`, `ReserveStock`, the reservation projection, and the
  Facade publishers.
- Focused GREEN: the exact twelve-name filter executed 17 cases and passed
  17/17. Full Warehouse passed 23/23, retaining the six baseline tests.
- Warehouse architecture filter passed 6/6. `dotnet build src/BrewUp.slnx`
  succeeded with 0 errors and 200 existing package-audit/compiler warnings.
- Post-GREEN `Story13ImplementationConventionReview` passed 1/1 against the
  tracked and untracked Story 1.3 production-file set.
- Full no-build solution regression retained only classified baselines:
  Purchases `SubmitPurchaseOrderSuccessfully` failed with the existing
  `NotImplementedException`; three REST tests failed because RabbitMQ at
  `127.0.0.1:5672` was unavailable. Warehouse remained 23/23; Payment 27/27,
  Sales 7/7, Sagas 3/3, MasterData 3/3, and Dashboards 2/2 passed.

### Completion Notes List

- Added Warehouse-owned stable reservation contracts with constructor
  materialization so command and event row order cannot change.
- Added a terminal event-sourced `StockReservation` aggregate that makes one
  all-or-none decision, cumulatively compares duplicate canonical keys against
  one availability fact, and no-ops after either terminal history.
- Added a load-or-create/call/save command handler using the stable request ID,
  `Guid.CreateVersion7()`, and `ConfigureAwait(false)`.
- Added a dedicated ACL that assesses every original row in order, maps missing
  availability to zero in the ordered unit, and sends one complete command
  only after assessment.
- Added successful and failed reservation projections with persistence failure
  propagation. Failed projections contain no rows.
- Remaining availability now subtracts matching successful
  `QuantityOrdered` values exactly once by Warehouse, beer, and exact unit;
  reservation processing never mutates on-hand `Availability`.
- Added separate Warehouse Facade publishers for the matching success/failure
  integration outcomes and preserved correlation metadata.
- Preserved BC-005–BC-008 and BC-011 ownership/policy limits and
  AR-004–AR-012, AR-015, and AR-017 placement/dependency rules. No release,
  expiry, retry, compensation, partial reservation, shipment, invoice, unit
  conversion, stronger concurrency, or cross-module implementation dependency
  was introduced.

### File List

- `_bmad-output/implementation-artifacts/1-3-reserve-complete-sales-order-stock-in-warehouse.md`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/DomainIds/StockReservationId.cs`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationRequestedIntegrationEvent.cs`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReserved.cs`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationFailed.cs`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservedIntegrationEvent.cs`
- `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationFailedIntegrationEvent.cs`
- `src/Warehouse/BrewUp.Warehouse.Domain/Entities/StockReservation.cs`
- `src/Warehouse/BrewUp.Warehouse.Domain/CommandHandlers/ReserveStockCommandHandler.cs`
- `src/Warehouse/BrewUp.Warehouse.Domain/DomainHelper.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/Dtos/StockReservation.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservedEventHandler.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservationFailedEventHandler.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/Queries/StockReservationQueries.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/IStockReservationService.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/StockReservationService.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/AvailabilityService.cs`
- `src/Warehouse/BrewUp.Warehouse.ReadModel/ReadModelHelper.cs`
- `src/Warehouse/BrewUp.Warehouse.Facade/Acl/StockReservationRequestedIntegrationEventHandler.cs`
- `src/Warehouse/BrewUp.Warehouse.Facade/EventHandlers/StockReservedIntegrationEventPublisher.cs`
- `src/Warehouse/BrewUp.Warehouse.Facade/EventHandlers/StockReservationFailedIntegrationEventPublisher.cs`
- `src/Warehouse/BrewUp.Warehouse.Facade/WarehouseFacadeHelper.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Domain/ReserveAllStockSuccessfully.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenBeerMissing.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenQuantityInsufficient.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenUnitOfMeasureMismatches.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Domain/DoNotReserveStockTwice.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectCumulativeDuplicateRowsBeyondAvailability.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Facade/AssessEveryReservationRowBeforeDispatch.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/ReadModel/ProjectSuccessfulReservationOnce.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/ReadModel/DoNotProjectFailedReservationAsAvailability.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Facade/PublishReservationIntegrationOutcomes.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Architecture/WarehouseReservationContractsStayInSharedKernel.cs`
- `src/Warehouse/BrewUp.Warehouse.Tests/Architecture/Story13ImplementationConventionReview.cs`

## Change Log

- 2026-07-27: Implemented Warehouse-owned all-or-nothing complete-order stock
  reservation, projection-backed availability subtraction, owned integration
  outcomes, architecture/convention gates, and RED/GREEN regression evidence.
