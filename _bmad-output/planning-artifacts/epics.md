---
stepsCompleted: [1, 2, 3, 4]
inputDocuments:
  - _bmad-output/planning-artifacts/prd.md
  - _bmad-output/planning-artifacts/architecture.md
  - docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md
  - _bmad-output/project-context.md
  - .bmad-harness/governance/brewup-sales-order-confirmation.md
  - .bmad-harness/governance/brewup-module-structure.md
status: final
created: 2026-07-27
updated: 2026-07-27
---

# BrewUp - Epic Breakdown

## Overview

This document decomposes the approved BrewUp Order Confirmation requirements
into one feature epic and independently verifiable, ownership-aligned stories.
The approved design supplies the resolved domain decisions; BC-000 through
BC-011 and AR-000 through AR-018 remain binding.

## Requirements Inventory

### Functional Requirements

- **FR-001:** Payment owns its authorization aggregate, contracts, callback,
  projection, and wiring.
- **FR-002:** A stable request creates one pending Payment authorization.
- **FR-003:** A simulator callback supplies the outcome; only Payment emits
  `PaymentAuthorized`.
- **FR-004:** Duplicate authorization outcomes are no-ops.
- **FR-005:** Warehouse assesses every row before one reservation decision.
- **FR-006:** Warehouse reserves all rows or emits one failure with none.
- **FR-007:** Availability subtracts only active successful reservations.
- **FR-008:** Duplicate reservation requests do not reserve twice.
- **FR-009:** Sales stores nullable external authorization/reservation IDs.
- **FR-010:** Confirmation requires both IDs and emits one confirmation.
- **FR-011:** Missing evidence leaves the Sales Order unconfirmed.
- **FR-012:** Duplicate confirmation requests do not confirm twice.
- **FR-013:** Saga order is Payment -> Warehouse -> Sales after placement.
- **FR-014:** Reservation failure is terminal, retains authorization, and emits
  no confirmation or compensation.
- **FR-015:** Duplicate outcomes do not repeat downstream saga actions.
- **FR-016:** Payment uses the exact six standard projects.
- **FR-017:** Payment is registered in the REST host and all six projects are
  members of the solution.
- **FR-018:** Layer references and cross-module contracts preserve boundaries.
- **FR-019:** Behavior is test-first and finishes with targeted, architecture,
  solution, and harness verification.

### NonFunctional Requirements

- **NFR-001:** Domain and cross-module dependency boundaries remain enforceable.
- **NFR-002:** Stable IDs and state guards make duplicate handling deterministic.
- **NFR-003:** Projections remain eventually consistent and rebuildable.
- **NFR-004:** Existing budget, placement, and shipment timing are preserved.
- **NFR-005:** New GUIDs use `Guid.CreateVersion7()` and awaited tasks use
  `ConfigureAwait(false)`.

### Additional Requirements

- **ADR-001:** Use the existing .NET 10, DDD modular-monolith, CQRS, and
  event-sourcing paradigm (architecture AD-1, AD-11).
- **ADR-002:** Create the exact six Payment project paths, solution folder, and
  `PaymentModule` registration (AD-3).
- **ADR-003:** Enforce the allowed project-reference graph and SharedKernel
  contract placement (AD-2, AD-4).
- **ADR-004:** Keep Payment pending-to-authorized behavior inside Payment
  (AD-5).
- **ADR-005:** Keep all-or-nothing reservation and its projection inside
  Warehouse (AD-6, AD-9).
- **ADR-006:** Keep the evidence gate and both IDs inside Sales (AD-7, AD-9).
- **ADR-007:** Keep coordination order and terminal failure inside saga state,
  without producer authority (AD-8).
- **ADR-008:** Add domain, saga, boundary, and architecture fitness tests at the
  exact locations in AD-10.
- **ADR-009:** Add no operational, provider, compensation, shipment, or
  invoicing policy (AD-11 and Deferred).

### UX Design Requirements

None. This feature changes backend domain behavior and integration boundaries;
no user-interface contract is in scope.

### FR Coverage Map

- FR-001 through FR-004: Epic 1 - Payment-owned authorization.
- FR-005 through FR-008: Epic 1 - Warehouse-owned complete reservation.
- FR-009 through FR-012: Epic 1 - Sales-owned evidence-gated confirmation.
- FR-013 through FR-015: Epic 1 - ordered saga coordination and failure.
- FR-016 through FR-019: Epic 1 - module structure, wiring, boundaries, tests.

## Epic List

### Epic 1: Evidence-Backed Sales Order Confirmation

After this epic, BrewUp operations can trust `Confirmed` to mean that Payment
authorized payment and Warehouse reserved every requested beer, while each
bounded context retains its authority and duplicate outcomes remain safe.

**FRs covered:** FR-001 through FR-019.

**Implementation notes:** Five ordered stories preserve the controller-approved
ownership sequence. Each story is independently verifiable and introduces no
future-story dependency into its own acceptance criteria; end-to-end user value
is complete when all five stories land.

## Epic 1: Evidence-Backed Sales Order Confirmation

BrewUp operations can trust `Confirmed` to mean that Payment authorized
payment and Warehouse reserved every requested beer, with bounded-context
authority and duplicate safety preserved.

### Story 1.1: Establish the Payment Module and Authorization Domain

As a BrewUp operator,
I want an independently valid Payment module with its authorization domain,
So that Payment authority exists as a complete architectural increment.

**Acceptance Criteria:**

**Given** no authorization exists for a stable `PaymentAuthorizationId`
**When** `RequestPaymentAuthorization` is handled
**Then** Payment emits one `PaymentAuthorizationRequested` and restores the
aggregate as pending
**And** the aggregate retains only its Payment ID, Sales Order reference, and
pending state; correlation ID remains command/event metadata.

**Given** the same stable authorization request was already applied
**When** `RequestPaymentAuthorization` is replayed
**Then** Payment emits no second `PaymentAuthorizationRequested`
**And** `DoNotRequestPaymentAuthorizationTwice` proves the no-op.

**Given** a pending authorization and a provider-supplied reference
**When** `RecordPaymentAuthorized` is handled
**Then** Payment emits one `PaymentAuthorized`
**And** replaying the authorized outcome emits no second event.

**Given** the six minimal project shells and compilable test harness exist but
Payment contracts/domain behavior do not
**When** the four Payment `CommandSpecification<T>` tests are added and run
**Then** they fail for missing contracts/domain behavior before production code
is written
**And** pass after the aggregate and load-call-save handlers are implemented.

**Given** Payment becomes an implemented business authority in this story
**When** the increment is complete
**Then** all six exact Payment project shells exist, are members of
`src/BrewUp.slnx`, and minimal `PaymentModule`/REST host composition is present
**And** architecture fitness tests prove project existence, solution
membership, host registration, contract placement, and allowed references.

**Given** Story 1.1 creates GUIDs or awaits asynchronous work
**When** static/code-review verification inspects Story 1.1 changed production code
**Then** new GUIDs use `Guid.CreateVersion7()` and awaited tasks use
`ConfigureAwait(false)`
**And** `Story11ImplementationConventionReview` records no exception without explicit
architecture approval.

**Traceability and ownership:** FR-001 through FR-004; FR-016 through FR-019;
NFR-005; AD-1, AD-2, AD-3, AD-4, AD-5, AD-10, AD-11; BC-003, BC-004, BC-008, BC-011; AR-000, AR-001,
AR-002, AR-004 through AR-008, AR-012 through AR-017. Domain files belong in
`src/Payment/BrewUp.Payment.SharedKernel/{DomainIds,CustomTypes,Messages}/`,
`src/Payment/BrewUp.Payment.Domain/{Entities,CommandHandlers}/`, and
`src/Payment/BrewUp.Payment.Tests/Domain/`; minimal project shells belong at
all six AD-3 paths; composition belongs in `src/BrewUp.slnx`,
`src/BrewUp.Rest/Module/PaymentModule.cs`, `src/BrewUp.Rest/Program.cs`, and
`src/BrewUp.Rest/BrewUp.Rest.csproj`. Tests:
`RequestPaymentAuthorizationSuccessfully`,
`DoNotRequestPaymentAuthorizationTwice`,
`RecordPaymentAuthorizedSuccessfully`, `DoNotAuthorizePaymentTwice`, and
`PaymentModuleStructureAndComposition`. The minimal Facade registration may be
empty of endpoints until Story 1.2, but the module must build and be
architecture-valid.
No amount, currency, provider execution, decline, timeout, void, refund, or
retry policy is permitted.

### Story 1.2: Complete Payment Projection and Provider Callback Behavior

As a BrewUp operator,
I want Payment authorization exposed through a complete registered module,
So that authorized outcomes are projected and published through supported
integration boundaries.

**Acceptance Criteria:**

**Given** Story 1.1 provides all six projects, composition, contracts, and
domain behavior
**When** the module is completed
**Then** ReadModel, Infrastructure, and Facade contain the Payment projection,
EventStore/Mongo wiring, callback endpoint, and integration-event publisher
**And** existing solution/host composition remains valid.

**Given** the provider simulator supplies an authorized outcome
**When** it calls
`POST /v1/payment/authorizations/{authorizationId}/authorized`
**Then** the Payment Facade sends `RecordPaymentAuthorized`
**And** the read projection becomes authorized and publishes the
Payment-owned integration outcome.

**Given** the REST host composes modules
**When** BrewUp starts
**Then** `src/BrewUp.Rest/Module/PaymentModule.cs` registers the Payment Facade
**And** the host contains no Payment business logic.

**Given** architecture fitness tests run
**When** they inspect Payment
**Then** module placement, host/solution membership, contract placement, and
allowed references pass
**And** Payment Domain has no Facade, ReadModel, Infrastructure, ASP.NET,
MongoDB, RabbitMQ, Sales Domain, or Warehouse Domain dependency.

**Given** callback, projection, and integration behavior is implemented
**When** Payment boundary tests run
**Then** `RecordAuthorizedCallbackBoundary`, `ProjectPendingAuthorization`,
`ProjectAndPublishAuthorizedOutcome`, and `PaymentPersistenceWiring` pass
**And** duplicate callback delivery publishes no second integration outcome.

**Given** callback, projection, persistence, and publication behavior is absent
**When** the four named boundary/integration tests are written and run first
**Then** they fail for the missing Story 1.2 behavior before production changes
**And** they pass only after the minimum implementation is complete.

**Given** Story 1.2 changes production code
**When** `Story12ImplementationConventionReview` inspects those changed files
**Then** new GUIDs use `Guid.CreateVersion7()` and awaited tasks use
`ConfigureAwait(false)`
**And** the review runs after GREEN verification.

**Implementation order (mandatory):** (1) write the four named tests, (2) run
and record meaningful RED, (3) implement projection/persistence/Facade/callback/
publication behavior, (4) run GREEN plus architecture and convention checks.

**Traceability and ownership:** FR-001, FR-003, FR-004, FR-016 through FR-019;
NFR-005; AD-3,
AD-4, AD-5, AD-9, AD-10, AD-11; BC-003, BC-004; AR-000 through AR-017. ReadModel,
Infrastructure, Facade, Tests/Architecture, REST `PaymentModule`,
`BrewUp.Rest.csproj`, `Program.cs`, and `BrewUp.slnx` are the verified
locations. Boundary/integration tests live in
`src/Payment/BrewUp.Payment.Tests/{ReadModel,Infrastructure,Facade}/`; tests
also run Payment and REST architecture suites. No future story is required for
the Payment callback/projection boundary to work.

### Story 1.3: Reserve Complete Sales Order Stock in Warehouse

As a Sales Order process,
I want Warehouse to reserve every requested row or none,
So that confirmation evidence never represents a partial physical-stock
decision.

**Acceptance Criteria:**

**Given** every requested beer and quantity is available
**When** Warehouse assesses all rows and handles one `ReserveStock`
**Then** the `StockReservation` aggregate emits one `StockReserved` containing
all rows and its `StockReservationId`
**And** replaying the stable reservation ID does not reserve again.

**Given** multiple rows request the same Warehouse, beer, and unit of measure
**When** the aggregate evaluates the complete request
**Then** it compares the cumulative requested quantity against one availability
fact while retaining the original row order
**And** cannot reserve more than the available total.

**Given** any beer is missing, insufficient, or uses a mismatched unit
**When** Warehouse assesses the complete request
**Then** it emits one `StockReservationFailed`
**And** no requested row is reserved.

**Given** reservation outcomes are projected
**When** availability is queried
**Then** on-hand remains unchanged by reservation handlers and the query
subtracts active successful reservations exactly once
**And** failed reservations subtract zero.

**Given** new reservation tests are added
**When** implementation begins and ends
**Then** missing, insufficient, multi-row success, and duplicate tests are red
before behavior and green afterward
**And** the full Warehouse suite passes.

**Given** Story 1.3 changes production code
**When** `Story13ImplementationConventionReview` inspects those changed files
**Then** new GUIDs use `Guid.CreateVersion7()` and awaited tasks use
`ConfigureAwait(false)`
**And** the check runs after Warehouse GREEN verification.

**Traceability and ownership:** FR-005 through FR-008; FR-019; NFR-005; AD-1, AD-2,
AD-6, AD-9, AD-10, AD-11; BC-005 through BC-008, BC-011; AR-000, AR-004 through AR-012,
AR-017. Contracts belong in Warehouse SharedKernel; decision behavior in
Warehouse Domain; full-row assessment in Warehouse Facade ACL; outcome and
availability projections in Warehouse ReadModel; tests in
`src/Warehouse/BrewUp.Warehouse.Tests/Domain/`. Boundary tests
`AssessEveryReservationRowBeforeDispatch`, `ProjectSuccessfulReservationOnce`,
`DoNotProjectFailedReservationAsAvailability`, and
`WarehouseReservationContractsStayInSharedKernel` live in the Warehouse Tests
project and cover ACL, projection/query, integration outcome, and contract
placement. Sales performs no availability
or stock mutation. No release, expiry, partial-reservation, or shipment policy
is introduced.

### Story 1.4: Confirm Sales Orders Only with Both Evidence IDs

As a Sales operator,
I want Sales Order confirmation to require Payment and Warehouse evidence,
So that `Confirmed` cannot represent an invalid cross-context state.

**Acceptance Criteria:**

**Given** a Sales Order and both `PaymentAuthorizationId` and
`StockReservationId`
**When** `ConfirmSalesOrder` is handled
**Then** Sales emits one `SalesOrderConfirmed`, stores both external references,
and transitions to `Confirmed`
**And** it never loads or embeds Payment or Warehouse aggregates.

**Given** either evidence ID is missing
**When** confirmation is requested
**Then** Sales rejects the transition with a stable error
**And** emits no confirmation event.

**Given** the Sales Order is already confirmed
**When** confirmation is replayed
**Then** no second event is emitted
**And** the original evidence remains retained.

**Given** confirmation tests are added first
**When** the targeted and full Sales suites run before and after implementation
**Then** they demonstrate red then green for success, both missing-evidence
cases, and duplicate confirmation
**And** `ProjectAndPublishSalesOrderConfirmed` proves the Sales projection and
integration boundary.

**Given** Story 1.4 changes production code
**When** `Story14ImplementationConventionReview` inspects those changed files
**Then** new GUIDs use `Guid.CreateVersion7()` and awaited tasks use
`ConfigureAwait(false)`
**And** the check runs after Sales GREEN verification.

**Traceability and ownership:** FR-009 through FR-012; FR-019; NFR-005; AD-1, AD-2,
AD-7, AD-9, AD-10, AD-11; BC-001, BC-002, BC-004, BC-006, BC-009, BC-010; AR-000, AR-006
through AR-012, AR-015, AR-017. Contracts belong in Sales SharedKernel,
behavior in `SalesOrder` and Sales Domain handler, projection in Sales
ReadModel, registration in Sales Facade, and tests in
`src/Sales/BrewUp.Sales.Tests/Domain/`.

### Story 1.5: Coordinate Payment, Reservation, and Confirmation

As a Sales Order process,
I want the existing saga to coordinate authorization, reservation, and
confirmation in order,
So that a Sales Order completes exactly once only after both owned decisions
exist.

**Acceptance Criteria:**

**Given** existing budget verification and order placement have completed
**When** the saga advances
**Then** it creates `PaymentAuthorizationId` with `Guid.CreateVersion7()`,
requests Payment authorization, waits for `PaymentAuthorized`, creates
`StockReservationId` with `Guid.CreateVersion7()`, requests complete Warehouse
reservation, waits for `StockReserved`, and sends
`ConfirmSalesOrder` with both IDs
**And** completes only after `SalesOrderConfirmed`.

**Given** Payment is authorized and Warehouse emits
`StockReservationFailed`
**When** the saga records that outcome
**Then** it enters terminal reservation failure, retains the authorization ID,
and leaves the Sales Order unconfirmed
**And** sends no confirm, void, refund, retry, release, timeout, notification,
or expiry action.

**Given** authorization, reservation, or confirmation outcomes are duplicated
**When** handlers replay them
**Then** the saga repeats no downstream command
**And** cannot complete twice.

**Given** saga ordering and failure tests are written first
**When** targeted and full Saga, Payment, Warehouse, and Sales suites run
**Then** the new tests demonstrate red then green and all impacted suites pass
**And** `PreserveExistingShipmentTriggerTimingAfterSagaRewire` proves the
pre-existing shipment-trigger event and timing remain unchanged.

**Given** the legacy availability-to-acceptance shortcut exists
**When** this saga flow is rewired
**Then** its exact registration in
`src/Sales/BrewUp.Sales.Facade/SalesFacadeHelper.cs` is disconnected from this
confirmation flow
**And** `OrderConfirmationArchitectureBoundaries` proves there is no competing
acceptance path and no forbidden cross-module implementation reference.

**Given** Story 1.5 changes production code
**When** `Story15ImplementationConventionReview` inspects those changed files
**Then** both evidence IDs use `Guid.CreateVersion7()` and awaited tasks use
`ConfigureAwait(false)`
**And** the check runs after Saga GREEN verification.

**Traceability and ownership:** FR-013 through FR-015; FR-019; NFR-005; AD-1, AD-2,
AD-8, AD-10, AD-11; BC-001, BC-003 through BC-011; AR-012, AR-015, AR-017,
AR-018, AR-000.
Saga-owned state/events/handlers live under `src/Sagas/`; Payment,
Warehouse, and Sales decisions remain in their producing modules. Tests live in
`src/Sagas/BrewUp.Sagas.Tests/Orchestrators/` and explicitly cover ordering,
failure-without-compensation, duplicate outcomes, integration boundary
publication, `PreserveExistingShipmentTriggerTimingAfterSagaRewire`, and the
architecture fitness/convention checks above.
