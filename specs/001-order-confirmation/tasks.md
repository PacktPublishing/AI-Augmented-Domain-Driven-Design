# Tasks: Sales Order Confirmation

**Input**: Design documents from `specs/001-order-confirmation/`

**Prerequisites**: plan.md ✓ · spec.md ✓ · research.md ✓ · data-model.md ✓ · contracts/ ✓

**Tests**: Included — Test-First is NON-NEGOTIABLE (Constitution III). All failing tests MUST be written and observed to fail before implementation that makes them pass.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing. US1 is the full MVP: once Phase 3 is complete, Sales Order confirmation is independently verifiable.

## Format: `[ID] [P?] [Story?] Description — file path`

- **[P]**: Can run in parallel (independent files, no incomplete dependencies)
- **[US1/2/3]**: User story ownership (Phase 3+)
- Paths relative to repository root (`src/`, `specs/`)

---

## Phase 1: Setup

**Purpose**: Create the Payment module project scaffolding and register it in the solution and REST host. No business logic here.

- [X] T001 Create 6 Payment module .csproj files under `src/Payment/` — `src/Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj`, `src/Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj`, `src/Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj`, `src/Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj`, `src/Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj`, `src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj`
- [X] T002 Add all 6 Payment module projects to the solution — `src/BrewUp.slnx`
- [X] T003 [P] Set intra-Payment project references in each .csproj per AR-015: Domain→SharedKernel; ReadModel→SharedKernel; Infrastructure→Domain+SharedKernel; Facade→Domain+ReadModel+Infrastructure+SharedKernel; Tests→all — edit each `src/Payment/BrewUp.Payment.*.csproj`
- [X] T004 [P] Create PaymentModule IModule registration — `src/BrewUp.Rest/Module/PaymentModule.cs`

**Checkpoint**: Solution builds (`dotnet build src/BrewUp.slnx`) with empty Payment projects. Payment module is wired into the host.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Cross-context shared contracts, domain IDs, and status values that ALL user stories depend on. No story work can begin until this phase is complete.

**⚠️ CRITICAL**: No user story phase can start until this phase is complete.

- [X] T005 [P] Add `PaymentAuthorizationReference : DomainId` to Sales SharedKernel — `src/Sales/BrewUp.Sales.SharedKernel/CustomTypes/PaymentAuthorizationReference.cs`
- [X] T006 [P] Add `StockReservationReference : DomainId` to Sales SharedKernel — `src/Sales/BrewUp.Sales.SharedKernel/CustomTypes/StockReservationReference.cs`
- [X] T007 [P] Add `StockReservationId : DomainId` to Warehouse SharedKernel — `src/Warehouse/BrewUp.Warehouse.SharedKernel/CustomTypes/StockReservationId.cs`
- [X] T008 [P] Add `SalesOrderStatus.Confirmed` (id `6`) to the Enumeration and `List()` — `src/Sales/BrewUp.Sales.SharedKernel/Enums/SalesOrderStatus.cs`
- [X] T009 [P] Add `PaymentAuthorizedIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/PaymentAuthorizedIntegrationEvent.cs`
- [X] T010 [P] Add `PaymentDeclinedIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/PaymentDeclinedIntegrationEvent.cs`
- [X] T011 [P] Add `StockReservedIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/StockReservedIntegrationEvent.cs`
- [X] T012 [P] Add `StockReservationRejectedIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/StockReservationRejectedIntegrationEvent.cs`
- [X] T013 [P] Add `SagaSalesOrderReadyToConfirmIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/SagaSalesOrderReadyToConfirmIntegrationEvent.cs`
- [X] T014 [P] Add `SagaRequestsPaymentAuthorizationIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/SagaRequestsPaymentAuthorizationIntegrationEvent.cs`
- [X] T015 [P] Add `SagaRequestsStockReservationIntegrationEvent` to Shared saga integration events — `src/BrewUp.Shared/Messages/Events/Sagas/SagaRequestsStockReservationIntegrationEvent.cs`

**Checkpoint**: Solution builds with all new types. No business logic yet.

---

## Phase 3: User Story 1 — Confirm a Sales Order (Priority: P1) 🎯 MVP

**Goal**: A Sales Order transitions to `Confirmed` when a coordinator dispatches `ConfirmSalesOrder` carrying both a `PaymentAuthorizationReference` and a `StockReservationReference`. Sales owns and enforces the invariant; it never performs payment or stock decisions.

**Independent Test**: Write and run the CommandSpecs below with Given/When/Expect. No Payment module or Warehouse integration needed — evidence is injected directly into the test.

### Tests for User Story 1 ⚠️ Write FIRST — must FAIL before implementation

- [X] T016 [P] [US1] CommandSpec: `ConfirmSalesOrderSuccessfully` — Given SalesOrderCreated, When ConfirmSalesOrder with both refs, Expect SalesOrderConfirmed — `src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderSuccessfully.cs`
- [X] T017 [P] [US1] CommandSpec: `ConfirmSalesOrderInvariantMissingPayment` — Given SalesOrderCreated, When ConfirmSalesOrder with empty PaymentAuthorizationReference, Expect no SalesOrderConfirmed — `src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderInvariantMissingPayment.cs`
- [X] T018 [P] [US1] CommandSpec: `ConfirmSalesOrderInvariantMissingStock` — Given SalesOrderCreated, When ConfirmSalesOrder with empty StockReservationReference, Expect no SalesOrderConfirmed — `src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderInvariantMissingStock.cs`
- [X] T019 [P] [US1] CommandSpec: `ConfirmSalesOrderIdempotent` — Given SalesOrderCreated + SalesOrderConfirmed, When ConfirmSalesOrder again, Expect no second SalesOrderConfirmed — `src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderIdempotent.cs`
- [X] T020 [P] [US1] Property-based test: `ConfirmationInvariantProperty` — for any SalesOrder in Confirmed state, PaymentAuthorizationReference and StockReservationReference MUST both be present — `src/Sales/BrewUp.Sales.Tests/Domain/ConfirmationInvariantPropertyTests.cs`
- [X] T021 [P] [US1] Architecture test: assert Sales has no reference to `Payment.Domain` or `Warehouse.Domain` — `src/Sales/BrewUp.Sales.Tests/Architecture/SalesArchitectureTests.cs`

### Implementation for User Story 1

- [X] T022 [P] [US1] Add `ConfirmSalesOrder` command carrying `PaymentAuthorizationReference` and `StockReservationReference` — `src/Sales/BrewUp.Sales.SharedKernel/Messages/Commands/ConfirmSalesOrder.cs`
- [X] T023 [P] [US1] Add `SalesOrderConfirmed` domain event carrying both references — `src/Sales/BrewUp.Sales.SharedKernel/Messages/Events/SalesOrderConfirmed.cs`
- [X] T024 [US1] Add `SalesOrder.ConfirmOrder(PaymentAuthorizationReference, StockReservationReference, Guid correlationId)` and `Apply(SalesOrderConfirmed)` with invariant guard (BC-010) and idempotency (FR-009) — `src/Sales/BrewUp.Sales.Domain/Entities/SalesOrder.cs`
- [X] T025 [US1] Add `ConfirmSalesOrderCommandHandler` — load aggregate, call `ConfirmOrder`, save — `src/Sales/BrewUp.Sales.Domain/CommandHandlers/ConfirmSalesOrderCommandHandler.cs`
- [X] T026 [US1] Register `AddCommandHandler<ConfirmSalesOrderCommandHandler>()` in `AddSalesDomain()` — `src/Sales/BrewUp.Sales.Domain/DomainHelper.cs`
- [X] T027 [P] [US1] Add `SalesOrderConfirmedEventHandler` — MongoDB upsert on Sales Order read model — `src/Sales/BrewUp.Sales.ReadModel/EventHandlers/SalesOrderConfirmedEventHandler.cs`
- [X] T028 [US1] Add `SagaSalesOrderReadyToConfirmIntegrationEventHandler` (ACL) — dispatches `ConfirmSalesOrder` wrapping the id strings in `PaymentAuthorizationReference`/`StockReservationReference` — `src/Sales/BrewUp.Sales.Facade/Acl/SagaSalesOrderReadyToConfirmIntegrationEventHandler.cs`
- [X] T029 [US1] Register `AddIntegrationEventHandler<SagaSalesOrderReadyToConfirmIntegrationEventHandler>()` in `AddSalesFacade()` — `src/Sales/BrewUp.Sales.Facade/SalesFacadeHelper.cs`

**Checkpoint**: US1 is fully functional and independently testable. Run `dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj` — all US1 CommandSpecs and property tests PASS.

---

## Phase 4: User Story 2 — Request the External Decisions (Priority: P2)

**Goal**: Sales emits requests for payment authorization (to Payment) and stock reservation (to Warehouse) in parallel, via the saga coordinator. Each authority produces its outcome independently; Sales never performs either decision.

**Independent Test**: CommandSpecs for `AuthorizePayment`, `ReserveStock`, and the saga parallel-dispatch and gate logic. Each module is verifiable in isolation.

### Tests for User Story 2 ⚠️ Write FIRST — must FAIL before implementation

- [X] T030 [P] [US2] CommandSpec: `AuthorizePaymentSuccessfully` — Given empty, When AuthorizePayment (approve path), Expect PaymentAuthorized — `src/Payment/BrewUp.Payment.Tests/Domain/AuthorizePaymentSuccessfully.cs`
- [X] T031 [P] [US2] CommandSpec: `AuthorizePaymentDeclined` — Given empty, When AuthorizePayment (decline path), Expect PaymentDeclined — `src/Payment/BrewUp.Payment.Tests/Domain/AuthorizePaymentDeclined.cs`
- [X] T032 [P] [US2] Architecture test: `PaymentArchitectureTests` — Payment.Domain has no ref to Infrastructure/ReadModel/Facade; no Payment.Domain ref from Sales or Sagas — `src/Payment/BrewUp.Payment.Tests/Architecture/PaymentArchitectureTests.cs`
- [X] T033 [P] [US2] CommandSpec: `ReserveStockSuccessfully` — Given AvailabilityCreated, When ReserveStock for all rows, Expect StockReserved — `src/Warehouse/BrewUp.Warehouse.Tests/Domain/ReserveStockSuccessfully.cs`
- [X] T034 [P] [US2] CommandSpec: `ReserveStockPartially` — Given AvailabilityCreated with partial stock, When ReserveStock for all rows, Expect StockReserved for the reservable subset — `src/Warehouse/BrewUp.Warehouse.Tests/Domain/ReserveStockPartially.cs`
- [X] T035 [P] [US2] Architecture test: `WarehouseArchitectureTests` — add assertions for new Warehouse dependencies (no forbidden cross-module refs) — `src/Warehouse/BrewUp.Warehouse.Tests/Architecture/WarehouseArchitectureTests.cs`
- [X] T036 [P] [US2] CommandSpec: `SagaBothEvidencesReceived_GateFires` — Given saga started, When MarkPaymentAuthorized then MarkStockReserved, Expect exactly one SagaSalesOrderReadyToConfirm — `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SagaBothEvidencesReceived_GateFires.cs`
- [X] T037 [P] [US2] CommandSpec: `SagaRequestsBothDecisionsInParallel` — Given saga started + SalesOrderPlaced, When InitiateConfirmationRequests, Expect SagaRequestsPaymentAuthorization and SagaRequestsStockReservation both raised — `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SagaRequestsBothDecisionsInParallel.cs`

### Implementation — Payment SharedKernel

- [X] T038 [P] [US2] Add `PaymentAuthorizationId : DomainId` — `src/Payment/BrewUp.Payment.SharedKernel/DomainIds/PaymentAuthorizationId.cs`
- [X] T039 [P] [US2] Add `PaymentAuthorizationStatus` Enumeration (Authorized=1, Declined=2, Pending=3) — `src/Payment/BrewUp.Payment.SharedKernel/Enums/PaymentAuthorizationStatus.cs`
- [X] T040 [P] [US2] Add `AuthorizePayment` command (carries salesOrderId, amount) — `src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/AuthorizePayment.cs`
- [X] T041 [P] [US2] Add `PaymentAuthorized` domain event (carries salesOrderId) — `src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentAuthorized.cs`
- [X] T042 [P] [US2] Add `PaymentDeclined` domain event (carries salesOrderId, reason) — `src/Payment/BrewUp.Payment.SharedKernel/Messages/Events/PaymentDeclined.cs`

### Implementation — Payment Domain

- [X] T043 [US2] Add `PaymentAuthorization` aggregate with `Authorize()` and `Apply(PaymentAuthorized)` / `Apply(PaymentDeclined)` — Payment owns the authorization decision — `src/Payment/BrewUp.Payment.Domain/Entities/PaymentAuthorization.cs`
- [X] T044 [US2] Add `AuthorizePaymentCommandHandler` — load/create PaymentAuthorization, call `Authorize()`, save — `src/Payment/BrewUp.Payment.Domain/CommandHandlers/AuthorizePaymentCommandHandler.cs`
- [X] T045 [US2] Add `PaymentDomainHelper` with `AddPaymentDomain()` registering `AddCommandHandler<AuthorizePaymentCommandHandler>()` — `src/Payment/BrewUp.Payment.Domain/PaymentDomainHelper.cs`

### Implementation — Payment ReadModel

- [X] T046 [P] [US2] Add `PaymentAuthorizedEventHandler` — publishes `PaymentAuthorizedIntegrationEvent` (correlationId used for saga routing) — `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentAuthorizedEventHandler.cs`
- [X] T047 [P] [US2] Add `PaymentDeclinedEventHandler` — publishes `PaymentDeclinedIntegrationEvent` — `src/Payment/BrewUp.Payment.ReadModel/EventHandlers/PaymentDeclinedEventHandler.cs`
- [X] T048 [US2] Add `PaymentReadModelHelper` with `AddPaymentReadModel()` registering both event handlers — `src/Payment/BrewUp.Payment.ReadModel/PaymentReadModelHelper.cs`

### Implementation — Payment Infrastructure

- [X] T049 [US2] Add `InfrastructureHelper` with `AddPaymentInfrastructure()` — EventStore persister for `PaymentAuthorization`, MongoDB collection wiring — `src/Payment/BrewUp.Payment.Infrastructure/InfrastructureHelper.cs`

### Implementation — Payment Facade

- [X] T050 [P] [US2] Add `IPaymentFacade` interface and `PaymentFacade` implementation — `src/Payment/BrewUp.Payment.Facade/IPaymentFacade.cs` and `src/Payment/BrewUp.Payment.Facade/PaymentFacade.cs`
- [X] T051 [US2] Add `PaymentFacadeHelper` with `AddPaymentFacade(configuration)` — calls `AddPaymentDomain()`, `AddPaymentReadModel()`, `AddPaymentInfrastructure()` — `src/Payment/BrewUp.Payment.Facade/PaymentFacadeHelper.cs`
- [X] T052 [P] [US2] Add `PaymentEndpoints` with `MapPaymentEndpoints()` minimal API map — `src/Payment/BrewUp.Payment.Facade/Endpoints/PaymentEndpoints.cs`
- [X] T053 [US2] Add `SagaRequestsPaymentAuthorizationIntegrationEventHandler` ACL — dispatches `AuthorizePayment` — `src/Payment/BrewUp.Payment.Facade/Acl/SagaRequestsPaymentAuthorizationIntegrationEventHandler.cs`
- [X] T054 [US2] Register `AddIntegrationEventHandler<SagaRequestsPaymentAuthorizationIntegrationEventHandler>()` in `AddPaymentFacade()` — `src/Payment/BrewUp.Payment.Facade/PaymentFacadeHelper.cs`

### Implementation — Warehouse SharedKernel extension

- [X] T055 [P] [US2] Add `ReserveStock` command (carries warehouseId, correlationId, salesOrderId, rows) — `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs`
- [X] T056 [P] [US2] Add `StockReserved` domain event (carries StockReservationId, salesOrderId, reservedRows subset) — `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReserved.cs`
- [X] T057 [P] [US2] Add `StockReservationRejected` domain event (carries salesOrderId, reason) — `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Events/StockReservationRejected.cs`

### Implementation — Warehouse Domain extension

- [X] T058 [US2] Add `Availability.ReserveStock()` method producing either `StockReserved` (with the reservable subset, possibly partial per OQ-2) or `StockReservationRejected`; add `Apply(StockReserved)`, `Apply(StockReservationRejected)` — `src/Warehouse/BrewUp.Warehouse.Domain/Entities/Availability.cs`
- [X] T059 [US2] Add `ReserveStockCommandHandler` — load Availability, call `ReserveStock()`, save — `src/Warehouse/BrewUp.Warehouse.Domain/CommandHandlers/ReserveStockCommandHandler.cs`
- [X] T060 [US2] Register `AddCommandHandler<ReserveStockCommandHandler>()` in `AddDomain()` — `src/Warehouse/BrewUp.Warehouse.Domain/DomainHelper.cs`

### Implementation — Warehouse ReadModel extension

- [X] T061 [P] [US2] Add `StockReservedEventHandler` — publishes `StockReservedIntegrationEvent` — `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservedEventHandler.cs`
- [X] T062 [P] [US2] Add `StockReservationRejectedEventHandler` — publishes `StockReservationRejectedIntegrationEvent` — `src/Warehouse/BrewUp.Warehouse.ReadModel/EventHandlers/StockReservationRejectedEventHandler.cs`
- [X] T063 [US2] Register both new event handlers in the Warehouse ReadModel helper — `src/Warehouse/BrewUp.Warehouse.ReadModel/WarehouseReadModelHelper.cs`

### Implementation — Warehouse Facade extension

- [X] T064 [US2] Add `SagaRequestsStockReservationIntegrationEventHandler` ACL — dispatches `ReserveStock` — `src/Warehouse/BrewUp.Warehouse.Facade/Acl/SagaRequestsStockReservationIntegrationEventHandler.cs`
- [X] T065 [US2] Register `AddIntegrationEventHandler<SagaRequestsStockReservationIntegrationEventHandler>()` in `AddWarehouseFacade()` — `src/Warehouse/BrewUp.Warehouse.Facade/WarehouseFacadeHelper.cs`

### Implementation — Sagas SharedKernel extension

- [X] T066 [P] [US2] Add `SagaSalesOrderReadyToConfirm` domain event (carries salesOrderId, paymentAuthorizationId, stockReservationId) — gate event — `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaSalesOrderReadyToConfirm.cs`
- [X] T067 [P] [US2] Add `SagaRequestsPaymentAuthorization` domain event (carries salesOrderId, amount, correlationId) — `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaRequestsPaymentAuthorization.cs`
- [X] T068 [P] [US2] Add `SagaRequestsStockReservation` domain event (carries salesOrderId, warehouseId, rows) — `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaRequestsStockReservation.cs`

### Implementation — Sagas Domain extension

- [X] T069 [US2] Extend `SalesOrderSaga`: add `_paymentAuthorized`, `_stockReserved`, `_paymentAuthorizationId`, `_stockReservationId` fields; add `InitiateConfirmationRequests()` raising `SagaRequestsPaymentAuthorization` and `SagaRequestsStockReservation`; add `MarkPaymentAuthorized(paymentAuthorizationId, correlationId)` with gate check; add `MarkStockReserved(stockReservationId, correlationId)` with gate check; gate raises `SagaSalesOrderReadyToConfirm` exactly once when both flags true — `src/Sagas/BrewUp.Sagas.Domain/Entities/SalesOrderSaga.cs`
- [X] T070 [US2] Extend `SalesOrderSagaOrchestrator`: implement `IIntegrationEventHandlerAsync<PaymentAuthorizedIntegrationEvent>` and `IIntegrationEventHandlerAsync<StockReservedIntegrationEvent>`; update `HandleAsync(SalesOrderPlaced)` to also call `aggregate.InitiateConfirmationRequests()` after marking the order as placed — `src/Sagas/BrewUp.Sagas.Domain/Orchestrators/SalesOrderSagaOrchestrator.cs`

### Implementation — Sagas ReadModel extension

- [X] T071 [P] [US2] Add `SagaSalesOrderReadyToConfirmEventHandler` — publishes `SagaSalesOrderReadyToConfirmIntegrationEvent` — `src/Sagas/BrewUp.Sagas.ReadModel/EventHandlers/SagaSalesOrderReadyToConfirmEventHandler.cs`
- [X] T072 [P] [US2] Add `SagaRequestsPaymentAuthorizationEventHandler` — publishes `SagaRequestsPaymentAuthorizationIntegrationEvent` — `src/Sagas/BrewUp.Sagas.ReadModel/EventHandlers/SagaRequestsPaymentAuthorizationEventHandler.cs`
- [X] T073 [P] [US2] Add `SagaRequestsStockReservationEventHandler` — publishes `SagaRequestsStockReservationIntegrationEvent` — `src/Sagas/BrewUp.Sagas.ReadModel/EventHandlers/SagaRequestsStockReservationEventHandler.cs`
- [X] T074 [US2] Register all three new ReadModel event handlers via `AddDomainEventHandler<>()` in `SagaReadModelHelper` — `src/Sagas/BrewUp.Sagas.ReadModel/SagaReadModelHelper.cs`

**Checkpoint**: Full pipeline is wired. Run `dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj`, `dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj`, `dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj` — all US2 CommandSpecs PASS.

---

## Phase 5: User Story 3 — Withhold Confirmation on Negative Evidence (Priority: P2)

**Goal**: When Payment declines or Warehouse rejects the reservation, the Sales Order stays unconfirmed. The saga records the negative outcome without dispatching any compensation, release, or cancellation (FR-011 / OQ-1 resolved). The invariant is already enforced by US1; this phase handles the saga's negative-path branches.

**Independent Test**: CommandSpecs that confirm the gate event is never raised on negative paths.

### Tests for User Story 3 ⚠️ Write FIRST — must FAIL before implementation

- [X] T075 [P] [US3] CommandSpec: `SagaPaymentDeclined_NoGate` — Given saga started + SagaRequestsPaymentAuthorization raised, When MarkPaymentDeclined, Expect no SagaSalesOrderReadyToConfirm — `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SagaPaymentDeclined_NoGate.cs`
- [X] T076 [P] [US3] CommandSpec: `SagaStockReservationRejected_NoGate` — Given saga started + SagaRequestsStockReservation raised, When MarkStockReservationRejected, Expect no SagaSalesOrderReadyToConfirm — `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SagaStockReservationRejected_NoGate.cs`
- [X] T077 [P] [US3] CommandSpec: `SagaOneSidedSuccess_NoGate` — Given saga started, When MarkPaymentAuthorized then MarkStockReservationRejected, Expect no SagaSalesOrderReadyToConfirm (no compensation dispatched) — `src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SagaOneSidedSuccess_NoGate.cs`

### Implementation for User Story 3

- [X] T078 [P] [US3] Add `SagaPaymentDeclined` domain event (records the negative outcome, no compensation) — `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaPaymentDeclined.cs`
- [X] T079 [P] [US3] Add `SagaStockReservationRejected` domain event (records the negative outcome, no compensation) — `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaStockReservationRejected.cs`
- [X] T080 [US3] Extend `SalesOrderSaga`: add `MarkPaymentDeclined(reason, correlationId)` raising `SagaPaymentDeclined`; add `MarkStockReservationRejected(reason, correlationId)` raising `SagaStockReservationRejected`; no compensation is dispatched (FR-011, OQ-1) — `src/Sagas/BrewUp.Sagas.Domain/Entities/SalesOrderSaga.cs`
- [X] T081 [US3] Extend `SalesOrderSagaOrchestrator`: implement `IIntegrationEventHandlerAsync<PaymentDeclinedIntegrationEvent>` (calls `MarkPaymentDeclined`) and `IIntegrationEventHandlerAsync<StockReservationRejectedIntegrationEvent>` (calls `MarkStockReservationRejected`) — `src/Sagas/BrewUp.Sagas.Domain/Orchestrators/SalesOrderSagaOrchestrator.cs`

**Checkpoint**: All negative-path CommandSpecs PASS. The gate is never raised when either outcome is negative.

---

## Final Phase: Polish & Cross-Cutting Concerns

- [X] T082 [P] Verify all 6 Payment module projects are listed correctly in solution — `src/BrewUp.slnx`
- [X] T083 [P] Run full solution build — `dotnet build src/BrewUp.slnx` — zero errors, zero warnings introduced
- [X] T084 [P] Run full test suite — `dotnet test src/BrewUp.slnx` — all tests PASS, no regressions in existing modules
- [X] T085 [P] Verify all architecture fitness functions pass (forbidden references produce zero violations) — `dotnet test src/BrewUp.Shared.Tests/` and each `*/Tests/Architecture/`
- [X] T086 Validate quickstart end-to-end scenarios S1–S8 from `specs/001-order-confirmation/quickstart.md`
- [X] T087 [P] Confirm OQ-4 (reservation lifetime) and OQ-7 (notification/downstream) have no implementation — grep for any code related to reservation expiry or customer notification in newly added files

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — start immediately.
- **Phase 2 (Foundational)**: Depends on Phase 1 completion — BLOCKS all user stories.
- **Phase 3 (US1)**: Depends on Phase 2 completion — Sales aggregate independently verifiable via CommandSpecs.
- **Phase 4 (US2)**: Depends on Phase 2 completion — can run in parallel with Phase 3 if staffed.
- **Phase 5 (US3)**: Depends on Phase 4 (T069–T070 must be complete for the saga negative-path extensions).
- **Final Phase**: Depends on all story phases complete.

### User Story Dependencies

- **US1 (P1)**: After Phase 2 — no dependency on US2 or US3. CommandSpecs inject evidence directly.
- **US2 (P2)**: After Phase 2 — no dependency on US1. Can be built in parallel.
- **US3 (P2)**: After Phase 4 T069–T070 (needs `SalesOrderSaga` negative-path extension points).

### Within Each User Story

```text
Tests (failing) → SharedKernel contracts → Domain aggregate → Command handler → Helper registration
                                         ↘ ReadModel handler
                                         ↘ Facade ACL handler → FacadeHelper registration
```

### Parallel Opportunities

- All Phase 1 and Phase 2 tasks marked [P] can run in parallel once T001–T002 are done.
- All US2 Payment SharedKernel tasks (T038–T042) are fully independent of each other.
- All Sagas/Warehouse/Payment ReadModel event handlers are independent of each other.
- US1 and US2 phases can be worked in parallel by separate developers after Phase 2.

---

## Parallel Execution Examples

### US1 Example (single developer)

```text
T005, T006, T008 [P]  → T022, T023 [P]  → T024  → T025  → T026
                       → T027 [P]
                       → T028  → T029
(Tests T016–T021 [P] written before T022–T029 make them pass)
```

### US2 Payment parallel stream

```text
T038–T042 [P] → T043  → T044  → T045
                              ↘ T046, T047 [P] → T048
                              ↘ T049
                              ↘ T050 [P]  → T051  → T052 [P]
                                          → T053  → T054
```

### US2 Warehouse parallel stream (independent of Payment stream)

```text
T055–T057 [P] → T058  → T059  → T060
                       → T061, T062 [P] → T063
               T064  → T065
```

### US2 Sagas stream (after T069 on Saga domain is done)

```text
T066–T068 [P] → T069 → T070
               → T071–T073 [P] → T074
```

---

## Implementation Strategy

**MVP (Phase 3 only)**: Once Phase 1 + Phase 2 + Phase 3 are done, the Sales aggregate correctly enforces the confirmation invariant and the ACL handler wires it to the saga gate event. This is the minimum demonstrable value — the invariant is proven by tests without needing Payment or Warehouse running.

**Full Feature (Phase 3 + 4 + 5)**: Adds the complete event-driven pipeline — Payment module, Warehouse reservation, and saga coordination. End-to-end flow is observable.

**Out of scope (do not implement)**:
- OQ-4: Stock reservation lifetime / expiry — Warehouse decision, not in this feature.
- OQ-7: Customer notification, shipment triggers, invoicing — not owned by Sales.
- Any compensation, void, refund, or retry logic — FR-011 is explicit: no Sales-owned compensation.
