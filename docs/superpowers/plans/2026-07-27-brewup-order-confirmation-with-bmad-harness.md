# BrewUp Order Confirmation with BMAD Harness Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm a BrewUp Sales Order only after Payment has authorized payment and Warehouse has reserved every requested beer, while preserving the ownership and module rules enforced by the BMAD harness.

**Architecture:** The existing SalesOrder saga coordinates Payment → Warehouse → Sales using explicit commands and events. A new six-project Payment module owns `PaymentAuthorization`; Warehouse owns an all-or-nothing `StockReservation`; Sales stores only the two external evidence IDs and owns the final `Confirmed` transition.

**Tech Stack:** C# 14, .NET 10, ASP.NET Core minimal APIs, Muflone CQRS/Event Sourcing, RabbitMQ, EventStoreDB, MongoDB, xUnit, Muflone.SpecificationTests, NetArchTest, BMAD Method 6.10.0.

## Global Constraints

- `.bmad-harness/governance/brewup-sales-order-confirmation.md` (`BC-000` through `BC-011`) is authoritative.
- `.bmad-harness/governance/brewup-module-structure.md` (`AR-000` through `AR-018`) is authoritative.
- Every harness gate returns exactly `PASS`, `CONCERNS`, or `FAIL`; never advance on `FAIL`.
- Payment behavior lives only in `src/Payment/` and uses all six standard projects.
- Sales never authorizes payment, checks physical availability as authority, reserves stock, or embeds Payment/Warehouse models.
- Warehouse reservation is all-or-nothing for the complete Sales Order.
- Payment authorization remains recorded when reservation fails; no void, refund, retry, timeout, notification, or expiry policy is added.
- The existing shipment timing is unchanged.
- All new behavior starts with a failing test.
- Use `Guid.CreateVersion7()` and `ConfigureAwait(false)` where applicable.
- Do not commit installer-owned `_bmad/core/`, `_bmad/bmm/`, generated `.agents/skills/bmad-*`, or generated standard BMAD Copilot agents.

---

### Task 1: Produce Native BMAD Planning Artifacts and Pass Harness Gates

**Files:**
- Input: `docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`
- Create: `_bmad-output/planning-artifacts/product-brief.md`
- Create: `_bmad-output/planning-artifacts/prd.md`
- Create: `_bmad-output/planning-artifacts/architecture.md`
- Create: `_bmad-output/planning-artifacts/epics.md`
- Create: `_bmad-output/planning-artifacts/implementation-readiness-report.md`
- Create: `_bmad-output/implementation-artifacts/1-1-order-confirmation.md`
- Create: `_bmad-output/planning-artifacts/gates/product-brief-gate.md`
- Create: `_bmad-output/planning-artifacts/gates/prd-gate.md`
- Create: `_bmad-output/planning-artifacts/gates/architecture-readiness-gate.md`
- Create: `_bmad-output/planning-artifacts/gates/architecture-gate.md`
- Create: `_bmad-output/planning-artifacts/gates/epics-stories-gate.md`
- Create: `_bmad-output/planning-artifacts/gates/implementation-readiness-gate.md`
- Create: `_bmad-output/implementation-artifacts/story-gate.md`

**Interfaces:**
- Consumes: approved design, `_bmad-output/project-context.md`, all three governance files, and installed BMAD 6.10.0 skills.
- Produces: one canonical artifact per BMAD phase, an approved implementation story, and explicit BC/AR traceability used by every later task.

- [ ] **Step 1: Verify native BMAD resolves the harness customization**

Run the customization resolver for `bmad-prd`, `bmad-architecture`, and `bmad-dev-story`. Each resolved configuration must contain both activation steps, all three governance facts, and the harness-specific `on_complete` contract.

```powershell
uv run _bmad/scripts/resolve_customization.py --skill .agents/skills/bmad-prd --all
uv run _bmad/scripts/resolve_customization.py --skill .agents/skills/bmad-architecture --all
uv run _bmad/scripts/resolve_customization.py --skill .agents/skills/bmad-dev-story --all
```

Expected: resolved output references `_bmad-output/project-context.md`, both BrewUp governance documents, and `gate-contract.md`.

- [ ] **Step 2: Generate the product brief and PRD with native BMAD**

Invoke `bmad-product-brief`, then `bmad-prd`, using the approved design as intent input. The PRD acceptance criteria must explicitly include:

```text
AC1: Sales Order confirmation requires PaymentAuthorizationId and StockReservationId.
AC2: Warehouse reserves all requested rows or none.
AC3: Reservation failure leaves the Sales Order unconfirmed and retains the authorization.
AC4: No void, refund, retry, timeout, notification, or expiry behavior is introduced.
AC5: Duplicate outcomes cannot reserve or confirm twice.
```

Write canonical monolith artifacts at the paths listed above; do not create competing variants.

- [ ] **Step 3: Run the Product Brief and PRD guards**

Apply the read-only instructions in:

```text
.github/agents/bmad-brewup-product-brief-guard.agent.md
.github/agents/bmad-brewup-prd-guard.agent.md
```

Write the exact five-heading gate reports required by `gate-contract.md`. Expected decision: `PASS`.

- [ ] **Step 4: Generate and gate architecture**

Invoke `bmad-architecture`. The artifact must name all six Payment project paths, allowed references, Payment/Warehouse/Sales ownership, saga ordering, projection behavior, and test locations.

Run architecture readiness before generation and architecture guard after generation. Expected decisions: `PASS`.

- [ ] **Step 5: Generate epics, stories, readiness report, and implementation story**

Invoke, in order:

```text
bmad-create-epics-and-stories
bmad-check-implementation-readiness
bmad-create-story
```

Use one feature epic with implementation stories matching Tasks 2–6 below. Run each matching guard and stop if any report is not `PASS`.

- [ ] **Step 6: Verify installation did not mutate committed harness sources**

Run:

```powershell
git diff --exit-code -- .bmad-harness _bmad/custom .github/agents/bmad-brewup-architecture-guard.agent.md .github/agents/bmad-brewup-architecture-readiness.agent.md .github/agents/bmad-brewup-code-review-guard.agent.md .github/agents/bmad-brewup-epics-stories-guard.agent.md .github/agents/bmad-brewup-implementation-readiness.agent.md .github/agents/bmad-brewup-load-domain-context.agent.md .github/agents/bmad-brewup-prd-guard.agent.md .github/agents/bmad-brewup-product-brief-guard.agent.md .github/agents/bmad-brewup-story-guard.agent.md
```

Expected: no diff. Full static harness validation is deliberately deferred until Task 7 removes the locally installed BMAD core; the harness correctly rejects repositories that ship installed core.

- [ ] **Step 7: Commit only BMAD artifacts**

```powershell
git add _bmad-output/planning-artifacts _bmad-output/implementation-artifacts
git commit -m "docs: plan harness-governed order confirmation"
```

Do not stage BMAD installed core or generated tool integrations.

---

### Task 2: Establish the Six-Project Payment Boundary and Authorization Domain

**Files:**
- Create: `src/Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/DomainIds/PaymentAuthorizationId.cs`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/CustomTypes/SalesOrderReference.cs`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/RequestPaymentAuthorization.cs`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/RecordPaymentAuthorized.cs`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorizationRequested.cs`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorized.cs`
- Create: `src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorizedIntegrationEvent.cs`
- Create: `src/Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj`
- Create: `src/Payment/BrewUp.Payment.Domain/Entities/PaymentAuthorization.cs`
- Create: `src/Payment/BrewUp.Payment.Domain/CommandHandlers/RequestPaymentAuthorizationCommandHandler.cs`
- Create: `src/Payment/BrewUp.Payment.Domain/CommandHandlers/RecordPaymentAuthorizedCommandHandler.cs`
- Create: `src/Payment/BrewUp.Payment.Domain/DomainHelper.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj`
- Create: `src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj`
- Create: `src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj`
- Create: `src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj`
- Create: `src/Payment/BrewUp.Payment.Tests/Architecture/PaymentModuleStructureAndComposition.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/Domain/RequestPaymentAuthorizationSuccessfully.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/Domain/DoNotRequestPaymentAuthorizationTwice.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/Domain/RecordPaymentAuthorizedSuccessfully.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/Domain/DoNotAuthorizePaymentTwice.cs`
- Create: `src/BrewUp.Rest/Module/PaymentModule.cs`
- Modify: `src/BrewUp.Rest/Program.cs`
- Modify: `src/BrewUp.Rest/BrewUp.Rest.csproj`
- Modify: `src/BrewUp.slnx`

**Interfaces:**
- Consumes: Muflone `DomainId`, `Command`, `DomainEvent`, and `IRepository`.
- Produces:
  - all six standard Payment projects, their allowed references, solution entries, and minimal host composition;
  - `PaymentAuthorizationId(string value) : DomainId`
  - `SalesOrderReference(string Value)`
  - `RequestPaymentAuthorization(PaymentAuthorizationId, SalesOrderReference, Guid)`
  - `RecordPaymentAuthorized(PaymentAuthorizationId, string providerReference, Guid)`
  - `PaymentAuthorizedIntegrationEvent` for the saga.

- [ ] **Step 1: Create a compilable six-project and test-harness shell**

Create the six exact `.csproj` files with only the allowed project references and repository-pinned test packages. The Tests project must run and reference the empty Domain and SharedKernel shells. Do not add contracts, aggregate behavior, helpers, solution membership, or host registration yet; this is test setup, not GREEN.

- [ ] **Step 2: Write and run a meaningful architecture RED**

Create `PaymentModuleStructureAndComposition`. Assert solution membership, `PaymentModule`/Program registration, contract placement, and allowed/forbidden references. Run it while solution/host composition and contracts are absent and record an executed assertion failure rather than a missing-project or compilation failure.

```powershell
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter FullyQualifiedName~PaymentModuleStructureAndComposition
```

Expected: the test executes and fails because composition and contracts are absent.

- [ ] **Step 3: Write and run meaningful domain RED specifications**

```csharp
public sealed class RequestPaymentAuthorizationSuccessfully
    : CommandSpecification<RequestPaymentAuthorization>
{
    private readonly PaymentAuthorizationId _id = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderReference _order = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given() => [];
    protected override RequestPaymentAuthorization When() =>
        new(_id, _order, _correlationId);
    protected override ICommandHandlerAsync<RequestPaymentAuthorization> OnHandler() =>
        new RequestPaymentAuthorizationCommandHandler(Repository, new NullLoggerFactory());
    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new PaymentAuthorizationRequested(
            _id, _order, _correlationId);
    }
}
```

Also create `DoNotRequestPaymentAuthorizationTwice`, `RecordPaymentAuthorizedSuccessfully`, and `DoNotAuthorizePaymentTwice`. Both replay specifications seed the relevant history and expect no second event.

```powershell
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter "FullyQualifiedName~RequestPaymentAuthorizationSuccessfully|FullyQualifiedName~DoNotRequestPaymentAuthorizationTwice|FullyQualifiedName~RecordPaymentAuthorizedSuccessfully|FullyQualifiedName~DoNotAuthorizePaymentTwice"
```

Expected: the existing Tests project reaches meaningful RED because Payment contracts/domain behavior are absent, never because the project is missing.

- [ ] **Step 4: Implement strongly typed contracts and pending aggregate**

Use these signatures:

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

`PaymentAuthorization.Request(...)` raises `PaymentAuthorizationRequested`; its apply method sets the aggregate ID, order reference, and `Pending` state. Correlation remains command/event metadata, not aggregate business state. Payment amount and currency are intentionally absent because no such policy was approved.

- [ ] **Step 5: Implement event-sourced request and authorization behavior**

Seed `PaymentAuthorizationRequested`, send `RecordPaymentAuthorized`, and expect:

```csharp
new PaymentAuthorized(_id, "provider-auth-123", _correlationId)
```

For already-pending request history or already-authorized history, expect no second event. Both replay paths are idempotent no-ops.

```csharp
internal void RecordAuthorized(string providerReference, Guid correlationId)
{
    if (_isAuthorized)
        return;

    RaiseEvent(new PaymentAuthorized(
        new PaymentAuthorizationId(Id.Value),
        providerReference,
        correlationId));
}
```

The command handler only performs `load → RecordAuthorized → save`. It does not decide whether the provider outcome is authorized.

- [ ] **Step 6: Complete minimal solution and REST composition**

Add only minimal compiling `DomainHelper`, `PaymentReadModelHelper`, `InfrastructureHelper`, and `PaymentFacadeHelper`; Task 3 completes their behavior. Add all six project paths beneath `/50 Modules/Payment/` in `src/BrewUp.slnx`, create `PaymentModule`, register it in `Program.cs`, and add only the Facade project reference to `BrewUp.Rest.csproj`. Do not expose the callback endpoint or publish integration events yet.

- [ ] **Step 7: Verify GREEN and Story 1.1 conventions**

```powershell
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter "FullyQualifiedName~RequestPaymentAuthorizationSuccessfully|FullyQualifiedName~DoNotRequestPaymentAuthorizationTwice|FullyQualifiedName~RecordPaymentAuthorizedSuccessfully|FullyQualifiedName~DoNotAuthorizePaymentTwice"
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter FullyQualifiedName~PaymentModuleStructureAndComposition
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
dotnet test src/BrewUp.Rest.Tests/BrewUp.Rest.Tests.csproj --filter FullyQualifiedName~Architecture
dotnet build src/BrewUp.slnx
```

Expected: all targeted and full Payment tests pass, affected REST architecture tests pass without starting infrastructure-dependent fixtures, and the solution builds. Inspect only Story 1.1 changed production files: new GUIDs use `Guid.CreateVersion7()` and awaited tasks use `ConfigureAwait(false)`.

- [ ] **Step 8: Commit the complete Payment boundary and domain**

```powershell
git add src/Payment src/BrewUp.Rest/Module/PaymentModule.cs src/BrewUp.Rest/Program.cs src/BrewUp.Rest/BrewUp.Rest.csproj src/BrewUp.slnx _bmad-output/implementation-artifacts/1-1-order-confirmation.md
git commit -m "feat(payment): model payment authorization"
```

---

### Task 3: Complete the Six-Project Payment Module Behavior

**Files:**
- Modify: `src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj`
- Create: `src/Payment/BrewUp.Payment.ReadModel/Dtos/PaymentAuthorization.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/Queries/PaymentAuthorizationQueries.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/Services/IPaymentAuthorizationService.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/Services/PaymentAuthorizationService.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizationRequestedEventHandler.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizedEventHandler.cs`
- Create: `src/Payment/BrewUp.Payment.ReadModel/PaymentReadModelHelper.cs`
- Modify: `src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj`
- Create: `src/Payment/BrewUp.Payment.Infrastructure/PaymentPersister.cs`
- Create: `src/Payment/BrewUp.Payment.Infrastructure/InfrastructureHelper.cs`
- Modify: `src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj`
- Create: `src/Payment/BrewUp.Payment.Facade/IPaymentFacade.cs`
- Create: `src/Payment/BrewUp.Payment.Facade/PaymentFacade.cs`
- Create: `src/Payment/BrewUp.Payment.Facade/PaymentFacadeHelper.cs`
- Create: `src/Payment/BrewUp.Payment.Facade/Endpoints/PaymentEndpoints.cs`
- Create: `src/Payment/BrewUp.Payment.Facade/ExternalContracts/ProviderAuthorizationJson.cs`
- Create: `src/Payment/BrewUp.Payment.Facade/EventHandlers/PaymentAuthorizedIntegrationEventPublisher.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectPaymentAuthorizationRequested.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/Facade/RecordAuthorizedProviderOutcome.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/ReadModel/ProjectAndPublishAuthorizedOutcome.cs`
- Create: `src/Payment/BrewUp.Payment.Tests/Infrastructure/PaymentPersistenceWiring.cs`

**Interfaces:**
- Consumes: the complete six-project Payment boundary and domain events from Task 2, module-specific Mongo persister conventions, `IModule`.
- Produces:
  - `POST /v1/payment/authorizations/{authorizationId}/authorized`
  - `IPaymentFacade.RecordAuthorizedAsync(...)`
  - Payment read projection
  - `PaymentAuthorizedIntegrationEvent`
  - explicitly registered Payment module.

- [ ] **Step 1: Write failing Payment projection and facade tests**

Assert that:

```text
PaymentAuthorizationRequested creates a pending projection with the Sales Order reference.
An externally supplied authorized provider outcome reaches RecordPaymentAuthorized.
PaymentAuthorized updates the projection and publishes PaymentAuthorizedIntegrationEvent once.
```

Follow the existing module test styles and keep the provider decision outside Payment.

- [ ] **Step 2: Run the focused behavior tests and verify RED**

```powershell
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --filter "FullyQualifiedName~ProjectPaymentAuthorizationRequested|FullyQualifiedName~RecordAuthorizedProviderOutcome"
```

Expected: compilation failure because Payment projection, facade behavior, and callback contracts are absent.

- [ ] **Step 3: Add minimal read model and infrastructure**

The projection contains:

```csharp
public string Id { get; init; } = string.Empty;
public string SalesOrderId { get; init; } = string.Empty;
public string Status { get; init; } = "pending";
public string? ProviderReference { get; init; }
```

`PaymentAuthorizationRequestedEventHandler` inserts `pending`; `PaymentAuthorizedEventHandler` updates it to `authorized` and publishes `PaymentAuthorizedIntegrationEvent`.

- [ ] **Step 4: Add the provider-simulator callback boundary**

```csharp
group.MapPost(
    "/authorizations/{authorizationId}/authorized",
    async (string authorizationId, ProviderAuthorizationJson body,
           IPaymentFacade facade, CancellationToken ct) =>
    {
        var result = await facade.RecordAuthorizedAsync(
            new PaymentAuthorizationId(authorizationId),
            body.ProviderReference,
            ct);
        return result.Match<IResult>(
            success => Results.Accepted(
                $"/v1/payment/authorizations/{authorizationId}", success),
            Results.BadRequest);
    });
```

The endpoint records an externally supplied outcome. It must not calculate approval, interpret timeouts, retry, void, or refund.

- [ ] **Step 5: Complete Payment composition**

Complete service, persistence, projection, handler, and endpoint registrations inside the `PaymentModule` established in Task 2. Verify that its existing REST composition and all six solution entries remain valid; do not add a second module registration.

- [ ] **Step 6: Run Payment and REST architecture tests**

```powershell
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
dotnet test src/BrewUp.Rest.Tests/BrewUp.Rest.Tests.csproj --filter FullyQualifiedName~Architecture
```

Expected: Payment tests pass. REST architecture tests pass without starting infrastructure-dependent integration fixtures.

Run `Story12ImplementationConventionReview` over only Task 3 changed production files after GREEN: new GUIDs use `Guid.CreateVersion7()` and awaited tasks use `ConfigureAwait(false)` unless an explicit architecture exception exists.

- [ ] **Step 7: Commit Payment module behavior**

```powershell
git add src/Payment src/BrewUp.Rest/Module/PaymentModule.cs _bmad-output/implementation-artifacts/1-2-complete-payment-projection-and-provider-callback-behavior.md
git commit -m "feat(payment): register authorization module"
```

---

### Task 4: Implement Warehouse-Owned All-or-Nothing Stock Reservation

**Files:**
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/DomainIds/StockReservationId.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationRequestedIntegrationEvent.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReserved.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationFailed.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservedIntegrationEvent.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationFailedIntegrationEvent.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Domain/Entities/StockReservation.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Domain/CommandHandlers/ReserveStockCommandHandler.cs`
- Modify: `src/Warehouse/BrewUp.Warehouse.Domain/DomainHelper.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.ReadModel/Dtos/StockReservation.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservedEventHandler.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservationFailedEventHandler.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.ReadModel/Queries/StockReservationQueries.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/IStockReservationService.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/StockReservationService.cs`
- Modify: `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/IAvailabilityService.cs`
- Modify: `src/Warehouse/BrewUp.Warehouse.ReadModel/Services/AvailabilityService.cs`
- Modify: `src/Warehouse/BrewUp.Warehouse.ReadModel/ReadModelHelper.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Facade/Acl/StockReservationRequestedIntegrationEventHandler.cs`
- Modify: `src/Warehouse/BrewUp.Warehouse.Facade/WarehouseFacadeHelper.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Tests/Domain/ReserveAllStockSuccessfully.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenBeerMissing.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Tests/Domain/RejectReservationWhenQuantityInsufficient.cs`
- Create: `src/Warehouse/BrewUp.Warehouse.Tests/Domain/DoNotReserveStockTwice.cs`

**Interfaces:**
- Consumes: the Warehouse-owned `StockReservationRequestedIntegrationEvent`, current `IAvailabilityService`, and `ItemRequested`.
- Produces:
  - `StockReservationId`
  - `StockReservationRequestedIntegrationEvent(StockReservationId, WarehouseId, SalesOrderId, rows, correlationId)`
  - `ReserveStock(StockReservationId, WarehouseId, SalesOrderId, IEnumerable<ItemRequested>, Guid)`
  - one `StockReservedIntegrationEvent` or `StockReservationFailedIntegrationEvent`.

- [ ] **Step 1: Write failing complete-reservation specification**

```csharp
protected override ReserveStock When() => new(
    _reservationId,
    _warehouseId,
    _salesOrderId,
    [
        new ItemRequested(_beerA, new Quantity(2, "Bottle"), new Quantity(5, "Bottle")),
        new ItemRequested(_beerB, new Quantity(3, "Bottle"), new Quantity(3, "Bottle"))
    ],
    _correlationId);

protected override IEnumerable<DomainEvent> Expect()
{
    yield return new StockReserved(
        _reservationId, _warehouseId, _salesOrderId, When().Rows, _correlationId);
}
```

- [ ] **Step 2: Write failing missing/insufficient/no-duplicate specifications**

Missing beer uses available quantity `0`; insufficient beer uses available below requested. Both expect exactly one `StockReservationFailed` and no `StockReserved`. Replayed success plus duplicate command expects no second event.

- [ ] **Step 3: Run Warehouse reservation tests and verify RED**

```powershell
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj --filter FullyQualifiedName~Reservation
```

Expected: compilation failure because reservation contracts/domain are absent.

- [ ] **Step 4: Implement the aggregate decision**

```csharp
internal static StockReservation Reserve(
    StockReservationId id,
    WarehouseId warehouseId,
    SalesOrderId salesOrderId,
    IEnumerable<ItemRequested> rows,
    Guid correlationId)
{
    var assessedRows = rows.ToArray();
    var failure = assessedRows.FirstOrDefault(row =>
        row.QuantityAvailable.UnitOfMeasure != row.QuantityOrdered.UnitOfMeasure ||
        row.QuantityAvailable.Value < row.QuantityOrdered.Value);

    return failure is null
        ? CreateReserved(id, warehouseId, salesOrderId, assessedRows, correlationId)
        : CreateFailed(id, warehouseId, salesOrderId, assessedRows,
            "All requested stock is not available.", correlationId);
}
```

The command handler contains no availability policy: it passes the complete assessed request to the aggregate and saves its single outcome.

- [ ] **Step 5: Implement Warehouse ACL assessment without early return**

The ACL handler must evaluate every row, preserving order:

```csharp
var assessedRows = new List<ItemRequested>();
foreach (var row in @event.Rows)
{
    var result = await availabilityService
        .GetAvailabilityByWarehouseIdAndBeerIdAsync(
            @event.WarehouseId, row.BeerId, cancellationToken)
        .ConfigureAwait(false);

    var available = new Quantity(0, row.QuantityOrdered.UnitOfMeasure);
    if (result.IsSuccess)
    {
        result.TryGetValue(out AvailabilityJson projection);
        available = new Quantity(projection.Quantity, projection.UnitOfMeasure);
    }

    assessedRows.Add(row with
    {
        QuantityAvailable = available
    });
}
```

Send one `ReserveStock` command only after the complete assessment. Remove no stock and publish no success from this ACL.

- [ ] **Step 6: Add projections and integration outcomes**

Project `StockReserved`/`StockReservationFailed` into a reservation document. Publish the matching Warehouse-owned integration event from the event handler. Availability queries subtract successful reservation quantities from on-hand quantities; failed reservations contribute zero.

- [ ] **Step 7: Run Warehouse tests**

```powershell
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj
```

Expected: all existing and new Warehouse tests pass.

Run `Story13ImplementationConventionReview` over only Task 4 changed production files after GREEN: new GUIDs use `Guid.CreateVersion7()` and awaited tasks use `ConfigureAwait(false)` unless an explicit architecture exception exists.

- [ ] **Step 8: Commit Warehouse reservation**

```powershell
git add src/Warehouse _bmad-output/implementation-artifacts/1-3-reserve-complete-sales-order-stock-in-warehouse.md
git commit -m "feat(warehouse): reserve complete order stock"
```

---

### Task 5: Add Evidence-Gated Sales Order Confirmation

**Files:**
- Create: `src/Sales/BrewUp.Sales.SharedKernel/Messages/Commands/ConfirmSalesOrder.cs`
- Create: `src/Sales/BrewUp.Sales.SharedKernel/Messages/Events/SalesOrderConfirmed.cs`
- Create: `src/Sales/BrewUp.Sales.SharedKernel/Messages/Events/SalesOrderConfirmedIntegrationEvent.cs`
- Modify: `src/Sales/BrewUp.Sales.SharedKernel/Enums/SalesOrderStatus.cs`
- Modify: `src/Sales/BrewUp.Sales.SharedKernel/BrewUp.Sales.SharedKernel.csproj`
- Modify: `src/Sales/BrewUp.Sales.Domain/Entities/SalesOrder.cs`
- Create: `src/Sales/BrewUp.Sales.Domain/CommandHandlers/ConfirmSalesOrderCommandHandler.cs`
- Modify: `src/Sales/BrewUp.Sales.Domain/DomainHelper.cs`
- Create: `src/Sales/BrewUp.Sales.ReadModel/EventHandlers/SalesOrderConfirmedEventHandler.cs`
- Modify: `src/Sales/BrewUp.Sales.ReadModel/ReadModelHelper.cs`
- Modify: `src/Sales/BrewUp.Sales.Facade/SalesFacadeHelper.cs`
- Create: `src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderSuccessfully.cs`
- Create: `src/Sales/BrewUp.Sales.Tests/Domain/RejectConfirmationWithoutPaymentAuthorizationId.cs`
- Create: `src/Sales/BrewUp.Sales.Tests/Domain/RejectConfirmationWithoutStockReservation.cs`
- Create: `src/Sales/BrewUp.Sales.Tests/Domain/DoNotConfirmSalesOrderTwice.cs`

**Interfaces:**
- Consumes: `PaymentAuthorizationId` from Payment SharedKernel and `StockReservationId` from Warehouse SharedKernel.
- Produces:
  - `ConfirmSalesOrder(SalesOrderId, PaymentAuthorizationId?, StockReservationId?, Guid)`
  - `SalesOrderConfirmed`
  - `SalesOrderConfirmedIntegrationEvent`
  - `SalesOrderStatus.Confirmed`
  - projection update and saga completion integration outcome.

- [ ] **Step 1: Write failing confirmation specifications**

Success expects:

```csharp
new SalesOrderConfirmed(
    _salesOrderId,
    _paymentAuthorizationId,
    _stockReservationId,
    _correlationId)
```

Each missing-evidence test expects `InvalidOperationException` with a stable message and no confirmation event. The duplicate test replays `SalesOrderConfirmed` and expects an idempotent no-op.

- [ ] **Step 2: Run confirmation tests and verify RED**

```powershell
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj --filter FullyQualifiedName~Confirm
```

Expected: compilation failure because confirmation contracts/behavior are absent.

- [ ] **Step 3: Implement evidence-gated aggregate behavior**

```csharp
internal void Confirm(
    PaymentAuthorizationId? paymentAuthorizationId,
    StockReservationId? stockReservationId,
    Guid correlationId)
{
    if (Equals(_salesOrderStatus, SalesOrderStatus.Confirmed))
        return;
    if (paymentAuthorizationId is null)
        throw new InvalidOperationException("Payment authorization evidence is required.");
    if (stockReservationId is null)
        throw new InvalidOperationException("Stock reservation evidence is required.");

    RaiseEvent(new SalesOrderConfirmed(
        new SalesOrderId(Id.Value),
        paymentAuthorizationId,
        stockReservationId,
        correlationId));
}
```

The apply method stores both IDs and sets `SalesOrderStatus.Confirmed`.

- [ ] **Step 4: Implement handler and projection**

The handler performs only:

```csharp
var aggregate = await Repository
    .GetByIdAsync<SalesOrder>(command.AggregateId, cancellationToken)
    .ConfigureAwait(false);
aggregate!.Confirm(
    command.PaymentAuthorizationId,
    command.StockReservationId,
    command.MessageId);
await Repository
    .SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken)
    .ConfigureAwait(false);
```

Update the Sales projection to expose confirmed status and both external references. Publish the Sales-owned confirmation integration event used by the saga.

- [ ] **Step 5: Disconnect the old availability-to-acceptance shortcut**

Remove `SagaSalesOrderAvailabilityCheckedIntegrationEventHandler` from `SalesFacadeHelper` registration for this flow. Do not delete unrelated legacy contracts unless compilation proves they are now unused and the BMAD story explicitly permits cleanup.

- [ ] **Step 6: Run Sales tests**

```powershell
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj
```

Expected: all existing and new Sales tests pass.

Run `Story14ImplementationConventionReview` over only Task 5 changed production files after GREEN: new GUIDs use `Guid.CreateVersion7()` and awaited tasks use `ConfigureAwait(false)` unless an explicit architecture exception exists.

- [ ] **Step 7: Commit Sales confirmation**

```powershell
git add src/Sales
git commit -m "feat(sales): confirm orders with external evidence"
```

---

### Task 6: Rewire the Sales Order Saga to Payment → Reservation → Confirmation

**Files:**
- Create: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaPaymentAuthorizationRequested.cs`
- Create: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaPaymentAuthorized.cs`
- Create: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaStockReservationRequested.cs`
- Create: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaStockReserved.cs`
- Create: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaStockReservationFailed.cs`
- Create: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaSalesOrderConfirmationRequested.cs`
- Modify: `src/Sagas/BrewUp.Sagas.SharedKernel/BrewUp.Sagas.SharedKernel.csproj`
- Modify: `src/Sagas/BrewUp.Sagas.Domain/BrewUp.Sagas.Domain.csproj`
- Modify: `src/Sagas/BrewUp.Sagas.Domain/Entities/SalesOrderSaga.cs`
- Modify: `src/Sagas/BrewUp.Sagas.Domain/Orchestrators/SalesOrderSagaOrchestrator.cs`
- Create: `src/Sagas/BrewUp.Sagas.ReadModel/EventHandlers/SagaPaymentAuthorizationRequestedEventHandler.cs`
- Create: `src/Sagas/BrewUp.Sagas.ReadModel/EventHandlers/SagaStockReservationRequestedEventHandler.cs`
- Create: `src/Sagas/BrewUp.Sagas.ReadModel/EventHandlers/SagaSalesOrderConfirmationRequestedEventHandler.cs`
- Modify: `src/Sagas/BrewUp.Sagas.ReadModel/SagaReadModelHelper.cs`
- Modify: `src/Sagas/BrewUp.Sagas.Facade/SagasFacadeHelper.cs`
- Modify: `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SalesOrderSagaTests.cs`
- Create: `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/RequestReservationOnlyAfterAuthorization.cs`
- Create: `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/ConfirmOnlyAfterReservation.cs`
- Create: `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/DoNotCompensateWhenReservationFails.cs`
- Create: `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/IgnoreDuplicateOutcomes.cs`
- Create: `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/PreserveExistingShipmentTriggerTimingAfterSagaRewire.cs`

**Interfaces:**
- Consumes:
  - `PaymentAuthorizedIntegrationEvent`
  - `StockReservedIntegrationEvent`
  - `StockReservationFailedIntegrationEvent`
  - Sales confirmation integration event.
- Produces:
  - `RequestPaymentAuthorization`
  - Warehouse-owned `StockReservationRequestedIntegrationEvent` containing all order rows
  - `ConfirmSalesOrder` with both evidence IDs.

- [ ] **Step 1: Write failing saga-ordering tests**

Assert the exact sequence:

```text
SagaSalesOrderPlaced
→ SagaPaymentAuthorizationRequested
PaymentAuthorizedIntegrationEvent
→ SagaPaymentAuthorized
→ SagaStockReservationRequested
StockReservedIntegrationEvent
→ SagaStockReserved
→ SagaSalesOrderConfirmationRequested
SalesOrderConfirmed integration event
→ SagaSalesOrderSuccessfullyCompleted
```

The failure test asserts `SagaStockReservationFailed` and explicitly asserts absence of confirm, void, refund, retry, release, and notification commands.

`PreserveExistingShipmentTriggerTimingAfterSagaRewire` records the existing shipment-trigger event and timing before the rewire and proves they remain unchanged afterward.

- [ ] **Step 2: Run saga tests and verify RED**

```powershell
dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj --filter FullyQualifiedName~Authorization
dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj --filter FullyQualifiedName~Reservation
```

Expected: compilation failure because new transitions and handlers are absent.

- [ ] **Step 3: Store external evidence in saga state**

Add nullable fields:

```csharp
private PaymentAuthorizationId? _paymentAuthorizationId;
private StockReservationId? _stockReservationId;
```

The saga creates `PaymentAuthorizationId` and `StockReservationId` at their respective request transitions using `Guid.CreateVersion7()`; downstream modules receive those stable IDs and never replace them.

State methods raise saga-owned events but do not decide producer outcomes:

```csharp
internal void RecordPaymentAuthorized(PaymentAuthorizationId id, Guid correlationId);
internal void RecordStockReserved(StockReservationId id, Guid correlationId);
internal void RecordStockReservationFailed(string reason, Guid correlationId);
```

- [ ] **Step 4: Publish commands from saga event handlers**

`SagaPaymentAuthorizationRequestedEventHandler` sends `RequestPaymentAuthorization`.

`SagaStockReservationRequestedEventHandler` publishes the Warehouse reservation request with every stored order row.

`SagaSalesOrderConfirmationRequestedEventHandler` sends:

```csharp
new ConfirmSalesOrder(
    new BrewUp.Sales.SharedKernel.CustomTypes.SalesOrderId(@event.SalesOrderId),
    @event.PaymentAuthorizationId,
    @event.StockReservationId,
    MessageHelpers.GetCorrelationId(@event));
```

- [ ] **Step 5: Rewire the orchestrator integrations**

Replace the availability/acceptance success path with handlers for Payment authorization, Warehouse reservation success/failure, and Sales confirmation. On reservation failure, persist terminal saga failure only.

Keep the current budget verification and order placement stages. Do not modify shipment creation.

- [ ] **Step 6: Run saga and impacted module tests**

```powershell
dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj
```

Expected: all four targeted suites pass.

Run `Story15ImplementationConventionReview` over only Task 6 changed production files after GREEN: both evidence IDs and any other new GUIDs use `Guid.CreateVersion7()`, and awaited tasks use `ConfigureAwait(false)` unless an explicit architecture exception exists.

- [ ] **Step 7: Commit saga coordination**

```powershell
git add src/Sagas
git commit -m "feat(sagas): coordinate order confirmation"
```

---

### Task 7: Verify the Complete Feature and Run BMAD Code Review Gates

**Files:**
- Modify: `_bmad-output/implementation-artifacts/1-1-order-confirmation.md`
- Create: `_bmad-output/implementation-artifacts/code-review-report.md`
- Create: `_bmad-output/implementation-artifacts/code-review-gate.md`
- Modify: `docs/superpowers/plans/2026-07-27-brewup-order-confirmation-with-bmad-harness.md` only to check completed steps.

**Interfaces:**
- Consumes: approved BMAD story and all code/tests from Tasks 2–6.
- Produces: verified solution, BMAD review report, harness `PASS`, and a clean feature diff.

- [ ] **Step 1: Build the complete solution**

```powershell
dotnet build src/BrewUp.slnx --no-restore
```

Expected: build succeeds with no new errors. Record existing package-vulnerability and nullable warnings separately.

- [ ] **Step 2: Run targeted deterministic suites**

```powershell
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --no-build
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj --no-build
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj --no-build
dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj --no-build
```

Expected: every targeted suite passes.

- [ ] **Step 3: Run architecture tests**

```powershell
dotnet test src/BrewUp.Rest.Tests/BrewUp.Rest.Tests.csproj --no-build --filter FullyQualifiedName~Architecture
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj --no-build --filter FullyQualifiedName~Architecture
```

Expected: all architecture tests pass without requiring RabbitMQ.

- [ ] **Step 4: Run the full suite and classify baseline failures**

```powershell
dotnet test src/BrewUp.slnx --no-build
```

Expected feature result: no new failures. Known baseline failures may remain:

```text
Purchases SubmitPurchaseOrderSuccessfully: existing NotImplementedException.
REST integration/DI tests: RabbitMQ unavailable at localhost:5672.
```

Any different failure is treated as a regression and fixed before proceeding.

- [ ] **Step 5: Invoke native BMAD code review**

Run `bmad-code-review` against the approved story and exact changed-file scope. The report must check:

```text
BC-003/BC-004: Payment owns authorization.
BC-005/BC-006/BC-007: Warehouse owns reservation and stock mutation.
BC-009/BC-010: Sales stores both external IDs and requires both.
BC-011: no compensation/timeout/retry policy was invented.
AR-001/AR-002/AR-012/AR-014: full Payment module, tests, and solution entries exist.
AR-015/AR-017/AR-018: references, contracts, and saga authority are valid.
```

- [ ] **Step 6: Run the code-review guard**

Apply `.github/agents/bmad-brewup-code-review-guard.agent.md` to the approved story and changed files. Expected decision: `PASS`. Fix every `FAIL`; review and resolve every `CONCERNS`.

- [ ] **Step 7: Remove only installer-owned local BMAD runtime files**

Preserve every tracked harness file and preserve `_bmad/custom/config.user.toml`. Remove only paths confirmed as untracked installer output:

```powershell
$workspace = 'C:\Sviluppo\DWX26'
$fixedInstallerPaths = @(
    '_bmad\_config',
    '_bmad\bmm',
    '_bmad\core',
    '_bmad\scripts',
    '_bmad\config.toml',
    '_bmad\config.user.toml',
    '_bmad\custom\.gitignore',
    '_bmad\custom\config.toml',
    '.github\agents\bmad-agent-analyst.agent.md',
    '.github\agents\bmad-agent-architect.agent.md',
    '.github\agents\bmad-agent-dev.agent.md',
    '.github\agents\bmad-agent-pm.agent.md',
    '.github\agents\bmad-agent-tech-writer.agent.md',
    '.github\agents\bmad-agent-ux-designer.agent.md'
)

$skillPaths = Get-ChildItem -LiteralPath "$workspace\.agents\skills" -Directory |
    Where-Object Name -Like 'bmad-*' |
    Select-Object -ExpandProperty FullName

$targets = @($fixedInstallerPaths | ForEach-Object { Join-Path $workspace $_ }) + $skillPaths
foreach ($target in $targets) {
    if (-not (Test-Path -LiteralPath $target)) { continue }
    $resolved = (Resolve-Path -LiteralPath $target).Path
    if (-not $resolved.StartsWith("$workspace\", [StringComparison]::OrdinalIgnoreCase)) {
        throw "Unsafe installer cleanup target: $resolved"
    }
    $relative = [IO.Path]::GetRelativePath($workspace, $resolved).Replace('\', '/')
    git ls-files --error-unmatch -- $relative 2>$null
    if ($LASTEXITCODE -eq 0) {
        throw "Refusing to remove tracked path: $relative"
    }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
```

Expected: `_bmad/custom/bmad-*.toml`, `_bmad/custom/config.user.toml`, `.bmad-harness/`, `_bmad-output/project-context.md`, and all `bmad-brewup-*` guard agents remain.

- [ ] **Step 8: Run both harness validators**

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1 -ArtifactRoot _bmad-output
```

Expected: test runner ends with `BMAD harness tests passed.`; repository and artifact validation exit `0`, including `PASS [no-installed-core]` and `PASS [artifact-rules]`.

- [ ] **Step 9: Inspect final diff and working tree**

```powershell
git diff --check
git status --short
git diff --stat
```

Expected: no whitespace errors; only intentional artifacts/source/tests are tracked. Installed BMAD core/tool files remain uncommitted and are excluded from feature commits.

- [ ] **Step 10: Commit final review artifacts**

```powershell
git add _bmad-output/implementation-artifacts docs/superpowers/plans/2026-07-27-brewup-order-confirmation-with-bmad-harness.md
git commit -m "docs: complete BMAD order confirmation review"
```
