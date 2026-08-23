# Tasks: Sales Order Confirmation

**Input**: Design documents from `specs/001-order-confirmation/`

**Prerequisites**: [plan.md](plan.md) (required), [spec.md](spec.md) (user stories), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/)

**Tests**: Test tasks are INCLUDED and REQUIRED. The project constitution (Principle III — Test-First Discipline, NON-NEGOTIABLE) mandates a failing test before implementation. Aggregate tests use `Muflone.SpecificationTests` (`CommandSpecification` Given/When/Expect), matching existing specs in `BrewUp.Sales.Tests/Domain`.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: US1, US2, US3 (maps to spec.md user stories)
- File paths are exact and relative to the repository root.

## Conventions (from plan.md)

- DDD modular monolith; per-context layers `Domain` → `SharedKernel` → `Infrastructure`/`ReadModel` → `Facade` → `Tests`.
- Cross-context messages live in `BrewUp.Shared`. Sales stores only external decision references (no embedded Payment/Warehouse models — BC-002/BC-009).
- Strongly-typed ids derive from `Muflone.Core.DomainId`.

---

## Phase 1: Setup

**Purpose**: Establish a green baseline before changes.

- [X] T001 Confirm a clean build and green baseline: run `dotnet build src/BrewUp.slnx`, then `dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj` and `dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj`. Record that existing architecture fitness tests pass.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared building blocks used by multiple user stories. **No user story work begins until this phase is complete.**

- [X] T002 [P] Add `Confirmed` value to `SalesOrderStatus` in [src/Sales/BrewUp.Sales.SharedKernel/Enums/SalesOrderStatus.cs](../../src/Sales/BrewUp.Sales.SharedKernel/Enums/SalesOrderStatus.cs) (new id 6; include in `List()`).
- [X] T003 [P] Create strongly-typed id `PaymentAuthorizationId : DomainId` in [src/BrewUp.Shared/DomainIds/PaymentAuthorizationId.cs](../../src/BrewUp.Shared/DomainIds/PaymentAuthorizationId.cs) (mirror `WarehouseId.cs`).
- [X] T004 [P] Create strongly-typed id `StockReservationId : DomainId` in [src/BrewUp.Shared/DomainIds/StockReservationId.cs](../../src/BrewUp.Shared/DomainIds/StockReservationId.cs).
- [X] T005 Create Sales-owned domain event `SalesOrderConfirmed` in [src/Sales/BrewUp.Sales.SharedKernel/Messages/Events/SalesOrderConfirmed.cs](../../src/Sales/BrewUp.Sales.SharedKernel/Messages/Events/SalesOrderConfirmed.cs) carrying `SalesOrderId`, `PaymentAuthorizationId`, `StockReservationId`, `correlationId` (per [contracts/domain-events.md](contracts/domain-events.md)). Depends on T003, T004.
- [X] T006 Create command `ConfirmSalesOrder` in [src/Sales/BrewUp.Sales.SharedKernel/Messages/Commands/ConfirmSalesOrder.cs](../../src/Sales/BrewUp.Sales.SharedKernel/Messages/Commands/ConfirmSalesOrder.cs) carrying `SalesOrderId`, `PaymentAuthorizationId`, `StockReservationId`, `correlationId` (per [contracts/commands.md](contracts/commands.md)). Depends on T003, T004.

**Checkpoint**: Status value, id types, confirmation event, and command exist and compile.

---

## Phase 3: User Story 1 - Confirm a Sales Order with payment authorized and stock reserved (Priority: P1) 🎯 MVP

**Goal**: A Sales Order holding both external decision references transitions to `Confirmed`, recording both references and raising `SalesOrderConfirmed`.

**Independent Test**: Handle `ConfirmSalesOrder` against an `Accepted` Sales Order that has both references → assert one `SalesOrderConfirmed` and status `Confirmed`.

### Tests for User Story 1 (write first, MUST FAIL before implementation) ⚠️

- [X] T007 [P] [US1] Add aggregate spec `ConfirmSalesOrderSuccessfully : CommandSpecification<ConfirmSalesOrder>` in [src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderSuccessfully.cs](../../src/Sales/BrewUp.Sales.Tests/Domain/ConfirmSalesOrderSuccessfully.cs): Given `SalesOrderCreated`, When `ConfirmSalesOrder` with both refs, Expect `SalesOrderConfirmed`.
- [X] T008 [P] [US1] Add aggregate spec `ConfirmIsIdempotent` in [src/Sales/BrewUp.Sales.Tests/Domain/ConfirmIsIdempotent.cs](../../src/Sales/BrewUp.Sales.Tests/Domain/ConfirmIsIdempotent.cs): Given `SalesOrderCreated` + `SalesOrderConfirmed`, When `ConfirmSalesOrder` again, Expect no further event (empty `Expect()`).

### Implementation for User Story 1

- [X] T009 [US1] Add `Confirm(PaymentAuthorizationId, StockReservationId, Guid correlationId)` and `Apply(SalesOrderConfirmed)` to [src/Sales/BrewUp.Sales.Domain/Entities/SalesOrder.cs](../../src/Sales/BrewUp.Sales.Domain/Entities/SalesOrder.cs): raise `SalesOrderConfirmed` only when both refs present; no-op when already `Confirmed` (idempotent, INV-2); `Apply` records both refs and sets status `Confirmed`. Depends on T002, T005.
- [X] T010 [US1] Add `ConfirmSalesOrderCommandHandler : CommandHandlerAsync<ConfirmSalesOrder>` in [src/Sales/BrewUp.Sales.Domain/CommandHandlers/ConfirmSalesOrderCommandHandler.cs](../../src/Sales/BrewUp.Sales.Domain/CommandHandlers/ConfirmSalesOrderCommandHandler.cs): load `SalesOrder`, call `Confirm(...)`, save (mirror `AcceptSalesOrderCommandHandler`). No invariant logic in the handler. Depends on T006, T009.
- [X] T011 [US1] Register the handler with `services.AddCommandHandler<ConfirmSalesOrderCommandHandler>();` in [src/Sales/BrewUp.Sales.Domain/DomainHelper.cs](../../src/Sales/BrewUp.Sales.Domain/DomainHelper.cs). Depends on T010.

**Checkpoint**: US1 fully functional and independently testable (T007, T008 now pass).

---

## Phase 4: User Story 2 - Reserve stock as part of confirming the order (Priority: P2)

**Goal**: When commercial preconditions are met, the saga requests a stock reservation from Warehouse, records `StockReservationId` evidence on receiving `StockReserved`, and (with payment also authorized) sends exactly one `ConfirmSalesOrder` to Sales.

**Independent Test**: Drive the orchestrator with `PaymentAuthorized` + `StockReserved` (either order) → assert one `ConfirmSalesOrder` sent with both refs; with only one outcome present → none; on a failure outcome → none and Sales Order stays pre-confirmation.

### Cross-context contracts (in BrewUp.Shared / Warehouse SharedKernel)

- [X] T012 [P] [US2] Create integration event `PaymentAuthorized` in [src/BrewUp.Shared/Messages/Events/Sagas/PaymentAuthorized.cs](../../src/BrewUp.Shared/Messages/Events/Sagas/PaymentAuthorized.cs) carrying `IntegrationId`, `correlationId`, `PaymentAuthorizationId` (string) — per [contracts/integration-events.md](contracts/integration-events.md). Mirror `SalesOrderConfirmed` saga event style.
- [X] T013 [P] [US2] Create integration event `PaymentAuthorizationFailed` in [src/BrewUp.Shared/Messages/Events/Sagas/PaymentAuthorizationFailed.cs](../../src/BrewUp.Shared/Messages/Events/Sagas/PaymentAuthorizationFailed.cs) carrying `IntegrationId`, `correlationId`, `Reason`.
- [X] T014 [P] [US2] Create integration event `StockReserved` in [src/BrewUp.Shared/Messages/Events/Sagas/StockReserved.cs](../../src/BrewUp.Shared/Messages/Events/Sagas/StockReserved.cs) carrying `IntegrationId`, `correlationId`, `StockReservationId` (string), optional `IEnumerable<ItemRequested> Rows`.
- [X] T015 [P] [US2] Create integration event `StockReservationFailed` in [src/BrewUp.Shared/Messages/Events/Sagas/StockReservationFailed.cs](../../src/BrewUp.Shared/Messages/Events/Sagas/StockReservationFailed.cs) carrying `IntegrationId`, `correlationId`, optional `Rows`, `Reason`.
- [X] T016 [P] [US2] Create the Warehouse-owned reservation request contract `ReserveStock` in [src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs](../../src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs) carrying `WarehouseId`, the Sales Order reference, requested rows, and `correlationId` (contract only — Warehouse-side handler is out of scope per research D2).

### Tests for User Story 2 (write first, MUST FAIL before implementation) ⚠️

- [X] T017 [P] [US2] Add `SalesOrderSagaConfirmationTests` in [src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SalesOrderSagaConfirmationTests.cs](../../src/Sagas/BrewUp.Sagas.Tests/Orchestrators/SalesOrderSagaConfirmationTests.cs) covering: (a) both outcomes → exactly one `ConfirmSalesOrder`; (b) single outcome → no `ConfirmSalesOrder`; (c) `StockReservationFailed`/`PaymentAuthorizationFailed` → no `ConfirmSalesOrder` and no Sales Order transition. Use test doubles to emit outcome integration events. Depends on T012–T015.

### Implementation for User Story 2

- [X] T018 [US2] Extend [src/Sagas/BrewUp.Sagas.Domain/Entities/SalesOrderSaga.cs](../../src/Sagas/BrewUp.Sagas.Domain/Entities/SalesOrderSaga.cs): add `_paymentAuthorizationId`/`_stockReservationId` state and `MarkPaymentAuthorized`, `MarkStockReserved`, `MarkConfirmationFailed` methods with corresponding saga events + `Apply` (mirror existing `Mark*`). Depends on T012–T015.
- [X] T019 [US2] Extend [src/Sagas/BrewUp.Sagas.Domain/Orchestrators/SalesOrderSagaOrchestrator.cs](../../src/Sagas/BrewUp.Sagas.Domain/Orchestrators/SalesOrderSagaOrchestrator.cs): implement `IIntegrationEventHandlerAsync<PaymentAuthorized>`, `<StockReserved>`, `<PaymentAuthorizationFailed>`, `<StockReservationFailed>`; emit the `ReserveStock` request when preconditions warrant; when both evidences present, send exactly one `ConfirmSalesOrder` (guarded for idempotency, FR-009). Depends on T006, T016, T018.
- [X] T020 [US2] Register the new orchestrator integration-event subscriptions following the existing `services.AddIntegrationEventHandler<SalesOrderSagaOrchestrator>()` pattern in [src/Sagas/BrewUp.Sagas.Domain/SagasDomainHelper.cs](../../src/Sagas/BrewUp.Sagas.Domain/SagasDomainHelper.cs); verify the RabbitMQ consumer is subscribed to the new event topics. Depends on T019.

**Checkpoint**: US1 + US2 both work independently; saga drives confirmation end-to-end via test doubles.

---

## Phase 5: User Story 3 - Withhold confirmation when required evidence is missing (Priority: P3)

**Goal**: When either reference is absent, the Sales Order does not become `Confirmed` and no invalid state is persisted.

**Independent Test**: Handle `ConfirmSalesOrder`/call `Confirm` with a missing reference → assert no `SalesOrderConfirmed` and status unchanged.

### Tests for User Story 3 (write first, MUST FAIL before implementation) ⚠️

- [X] T021 [P] [US3] Add `ConfirmRejectedWhenEvidenceMissing` in [src/Sales/BrewUp.Sales.Tests/Domain/ConfirmRejectedWhenEvidenceMissing.cs](../../src/Sales/BrewUp.Sales.Tests/Domain/ConfirmRejectedWhenEvidenceMissing.cs) with three cases: missing `PaymentAuthorizationId`; missing `StockReservationId`; missing both — each Expect no `SalesOrderConfirmed`.

### Implementation for User Story 3

- [ ] T022 [US3] In [src/Sales/BrewUp.Sales.Domain/Entities/SalesOrder.cs](../../src/Sales/BrewUp.Sales.Domain/Entities/SalesOrder.cs), ensure/harden the `Confirm` guard so a missing or empty reference yields no event and no state change (INV-1, FR-010); confirm no invalid `Confirmed` state can be persisted. Depends on T009.

**Checkpoint**: All three user stories independently functional; core invariant (BC-010) protected from every path.

---

## Phase 6: Polish & Cross-Cutting Concerns

- [X] T023 [P] Run architecture fitness tests and confirm green: `SalesArchitectureTests` and `SagasArchitectureTests` (no Sales dependency on Warehouse/Payment internals; no embedded foreign models). Files: [src/Sales/BrewUp.Sales.Tests/Architecture/SalesArchitectureTests.cs](../../src/Sales/BrewUp.Sales.Tests/Architecture/SalesArchitectureTests.cs), [src/Sagas/BrewUp.Sagas.Tests/Architecture/SagasArchitectureTests.cs](../../src/Sagas/BrewUp.Sagas.Tests/Architecture/SagasArchitectureTests.cs).
- [X] T024 Run full `dotnet test src/BrewUp.slnx` and walk the [quickstart.md](quickstart.md) scenarios 1–4; verify Success Criteria SC-001…SC-005.
- [X] T025 [P] (Constitution Principle V follow-up) Add a CsCheck property-based test asserting the confirmation invariant (Confirmed ⇒ both references present) and record the Stryker mutation-gate follow-up for `BrewUp.Sales.Domain` noted in plan Complexity Tracking.

---

## Dependencies & Execution Order

### Phase dependencies

- **Setup (T001)** → no dependencies.
- **Foundational (T002–T006)** → after Setup; **blocks all user stories**. T005/T006 depend on T003+T004.
- **US1 (T007–T011)** → after Foundational. MVP.
- **US2 (T012–T020)** → after Foundational; contracts T012–T016 are independent of US1; T019 also depends on `ConfirmSalesOrder` (T006).
- **US3 (T021–T022)** → after Foundational; T022 depends on the `Confirm` method from US1 (T009).
- **Polish (T023–T025)** → after the targeted user stories are complete.

### Story independence

- **US1** is self-contained at the aggregate + command-handler level (MVP).
- **US2** is testable in isolation via the saga orchestrator with test doubles emitting outcome events.
- **US3** reuses the `SalesOrder.Confirm` method introduced in US1 (earliest story owning the shared behavior) and is independently testable through its rejection specs.

### Parallel opportunities

- Foundational: T002, T003, T004 in parallel; then T005, T006.
- US1 tests: T007, T008 in parallel (write before T009).
- US2 contracts: T012, T013, T014, T015, T016 all in parallel; T017 after them.
- US3 test T021 in parallel with other stories' test authoring.
- Polish: T023 and T025 in parallel.

---

## Implementation Strategy

- **MVP first**: complete Phase 1 → Phase 2 → **Phase 3 (US1)**. This delivers the headline value: a Sales Order confirms when both external decision references are present, with the invariant enforced inside the aggregate.
- **Incremental delivery**: add **US2** to wire the saga-driven coordination and stock-reservation evidence, then **US3** to lock down the rejection paths. Run Polish to validate fitness functions, quickstart, and success criteria.
- Keep every step test-first (Principle III): author the failing spec, observe red, implement, observe green.

## Suggested MVP scope

**User Story 1 only** (T001–T011): a Sales Order transitions to `Confirmed` when handed both a `PaymentAuthorizationId` and a `StockReservationId`, raising `SalesOrderConfirmed`, with idempotency — fully testable without the saga or external producers.
