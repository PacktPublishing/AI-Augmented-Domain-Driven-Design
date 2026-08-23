# Feature Specification: Sales Order Confirmation

**Feature Branch**: `001-order-confirmation`

**Created**: 2026-06-25

**Status**: Draft

**Input**: User description: "Implement order confirmation for BrewUp. An order can be confirmed when: the customer payment has been authorized; all requested beers are available in the warehouse. When the order is confirmed, reserve the stock."

## Domain Constraints (Authoritative)

<!--
  These constraints are carried from the BrewUp Sales Order Confirmation domain carrier
  (.specify/memory/domain-carriers/brewup-sales-order-confirmation.md) and are AUTHORITATIVE.
  They MUST be preserved in every downstream artifact (clarifications, plan, tasks, code, tests).
  A generated artifact that violates these is architecturally misaligned, not merely incomplete.
-->

- **Three separate domain authorities**: **Sales** (commercial commitment), **Payment** (payment authorization), **Warehouse** (physical stock). Sales may depend on decisions produced by Payment and Warehouse but MUST NOT own the models or processes that produce those decisions.
- **Sales owns the Sales Order lifecycle** — its commercial status, customer demand, and the transition to `Confirmed` once required external evidence is present. Use the term **Sales Order**, never a generic "Order".
- **Payment Authorization is an external decision** produced by Payment. Sales may request it through an integration boundary and may store a `PaymentAuthorizationId` as evidence, but Sales MUST NOT authorize payment, interpret payment-provider timeouts, void authorizations, or issue refunds.
- **Stock Reservation is an external decision** produced by Warehouse. Sales may request it through an integration boundary and may store a `StockReservationId` as evidence, but Sales MUST NOT reserve, release, or decrement physical stock, nor act as the authority of truth for stock availability.
- **Availability is not a durable fact.** Only a Stock Reservation is durable enough to support Sales Order confirmation.
- **External decision references, not embedded models.** `PaymentAuthorizationId` and `StockReservationId` are evidence that another bounded context made a decision. They are not a back door for Sales to load, modify, or own another context's aggregate. The Sales Order aggregate MUST NOT embed Payment or Warehouse domain models.
- **Confirmed requires evidence.** A Sales Order MUST NOT become `Confirmed` unless the required external decision references are present. For the base scenario, confirmation requires **both** `PaymentAuthorizationId` and `StockReservationId`. A `Confirmed` state with either reference missing is an invalid state.
- **Reacting is not owning.** Sales may react to `PaymentAuthorized` and `StockReserved` outcomes; it MUST NOT produce them.
- **No policy invention.** The specification MUST NOT silently invent retry, cancellation, refund, void, customer-notification, or reservation-expiration policy. Unknown business decisions remain explicit as `[NEEDS CLARIFICATION]` or **Open Questions** for the domain expert to resolve.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Confirm a Sales Order with payment authorized and stock reserved (Priority: P1)

As the Sales context, when a Sales Order has evidence that the customer's payment has been authorized and that the requested beers have been reserved in the warehouse, the Sales Order is transitioned to `Confirmed`, recording both pieces of external evidence.

**Why this priority**: This is the core commercial outcome of the feature — it is the single behavior that delivers the business value of "an order can be confirmed". Without it there is no feature. It is the MVP.

**Independent Test**: Provide a Sales Order that has a recorded `PaymentAuthorizationId` (from a Payment authorization outcome) and a recorded `StockReservationId` (from a Warehouse reservation outcome), trigger confirmation, and verify the Sales Order transitions to `Confirmed` and both references are persisted on the aggregate. Delivers the headline value independently of how the evidence was obtained.

**Acceptance Scenarios**:

1. **Given** a Sales Order awaiting confirmation that holds a valid `PaymentAuthorizationId` and a valid `StockReservationId`, **When** confirmation is attempted, **Then** the Sales Order status becomes `Confirmed` and both references remain recorded as evidence.
2. **Given** a Sales Order that becomes `Confirmed`, **When** the confirmation completes, **Then** a Sales-owned domain event announcing the confirmation is raised so other contexts can react.
3. **Given** a Sales Order already in `Confirmed` status, **When** confirmation is attempted again, **Then** the operation is idempotent and does not produce a second confirmation outcome or a duplicate stock reservation request.

---

### User Story 2 - Reserve stock as part of confirming the order (Priority: P2)

As the Sales context, when a Sales Order satisfies the commercial preconditions for confirmation (the customer's payment is authorized and the requested beers are reported available), Sales requests the Warehouse to reserve the stock and records the resulting `StockReservationId` as the durable evidence that supports confirmation.

**Why this priority**: The feature explicitly requires that "when the order is confirmed, reserve the stock". Because availability is not a durable fact, the reservation outcome is what makes confirmation safe and meaningful. This slice turns a transient availability signal into durable evidence.

**Independent Test**: Given a Sales Order with an authorized payment and reported beer availability, trigger the reservation step and verify that Sales emits a stock-reservation request across the Warehouse integration boundary and, upon receiving the `StockReserved` outcome, records the `StockReservationId` — without Sales mutating any warehouse stock itself.

**Acceptance Scenarios**:

1. **Given** a Sales Order with an authorized payment and reported availability for every requested beer, **When** the reservation step runs, **Then** Sales requests a stock reservation from Warehouse through the integration boundary and does not itself decrement, reserve, or mutate warehouse stock.
2. **Given** Warehouse produces a `StockReserved` outcome for the order, **When** Sales receives it, **Then** Sales records the `StockReservationId` as evidence on the Sales Order.
3. **Given** the requested beers span multiple order rows, **When** the reservation is requested, **Then** the reservation request reflects every requested beer and quantity on the order.

---

### User Story 3 - Withhold confirmation when required evidence is missing (Priority: P3)

As the Sales context, when either the customer's payment has not been authorized or the requested beers cannot be reserved, the Sales Order is NOT transitioned to `Confirmed` and remains in its pre-confirmation status awaiting resolution.

**Why this priority**: Protects the core invariant (BC-010): a Sales Order must never reach `Confirmed` without both pieces of evidence. It guards against an over-eager confirmation but is secondary to first delivering the happy path.

**Independent Test**: Provide a Sales Order missing one required reference (no `PaymentAuthorizationId`, or no `StockReservationId`), attempt confirmation, and verify the order does not become `Confirmed` and stays in a valid pre-confirmation status.

**Acceptance Scenarios**:

1. **Given** a Sales Order with a recorded `StockReservationId` but no `PaymentAuthorizationId`, **When** confirmation is attempted, **Then** the Sales Order does not become `Confirmed`.
2. **Given** a Sales Order with a recorded `PaymentAuthorizationId` but no `StockReservationId`, **When** confirmation is attempted, **Then** the Sales Order does not become `Confirmed`.
3. **Given** a Sales Order with neither reference recorded, **When** confirmation is attempted, **Then** the Sales Order does not become `Confirmed` and no invalid state is persisted.

---

### Edge Cases

- **Payment authorized but stock cannot be reserved**: The order has a `PaymentAuthorizationId` but Warehouse cannot reserve all requested beers. The Sales Order remains in its pre-confirmation status (it does not become `Confirmed`); the Payment authority independently handles any release/void of the authorization according to its own rules, and Sales takes no further compensating action.
- **Partial availability**: Some requested beers are available/reservable and others are not. Whether a Sales Order may be partially reserved/confirmed is a domain decision. See Open Questions.
- **Stale availability**: Availability was reported but stock is no longer reservable at reservation time (because availability is not durable). Confirmation must rely on the reservation outcome, not the earlier availability signal.
- **Reservation outcome never arrives / times out**: How long Sales waits for a `StockReserved` outcome, and what happens on timeout, is undefined here. See Open Questions.
- **Duplicate confirmation / duplicate reservation**: Re-processing the same confirmation trigger must not create a second reservation or a second confirmation (idempotency).
- **Reservation expiration while payment pending**: How long a stock reservation may remain active before payment evidence is present is undefined. See Open Questions.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Sales context MUST own the transition of a Sales Order to a `Confirmed` status.
- **FR-002**: The Sales context MUST NOT transition a Sales Order to `Confirmed` unless it holds both a `PaymentAuthorizationId` (evidence of an authorized payment) and a `StockReservationId` (evidence of a reserved stock) for that order.
- **FR-003**: The Sales context MUST treat `PaymentAuthorizationId` and `StockReservationId` solely as external decision references (evidence), and MUST NOT embed Payment or Warehouse domain models within the Sales Order aggregate.
- **FR-004**: When a Sales Order satisfies the preconditions for confirmation, the Sales context MUST request a stock reservation from the Warehouse context through an integration boundary, rather than reserving, decrementing, or otherwise mutating warehouse stock itself.
- **FR-005**: Upon receiving a stock-reservation outcome from Warehouse, the Sales context MUST record the `StockReservationId` as evidence on the corresponding Sales Order.
- **FR-006**: The Sales context MUST rely on the durable stock-reservation outcome (not a transient availability signal) as the stock-side precondition for confirmation.
- **FR-007**: The Sales context MUST NOT authorize payment, interpret payment-provider timeouts, void payment authorizations, or issue refunds; it MAY only request payment authorization through an integration boundary and react to the resulting outcome.
- **FR-008**: When a Sales Order becomes `Confirmed`, the Sales context MUST raise a Sales-owned domain event communicating the confirmation so other contexts may react.
- **FR-009**: Confirmation MUST be idempotent: re-processing the confirmation trigger for an already-`Confirmed` Sales Order MUST NOT produce a second confirmation outcome nor a duplicate stock-reservation request.
- **FR-010**: A Sales Order with a `Confirmed` status and a missing required reference MUST be treated as an invalid state and MUST NOT be persisted or produced.
- **FR-011**: The Sales context MUST NOT invent retry, cancellation, refund, void, customer-notification, or reservation-expiration policy; any such behavior MUST be modeled as an external responsibility, an explicitly out-of-scope behavior, or an open question.
- **FR-012**: The existing Sales Order saga/process manager (Sagas context) MUST coordinate the confirmation flow across Payment and Warehouse and instruct the Sales context when the confirmation preconditions are met; the Sales context reacts to that instruction and to the Payment and Warehouse outcomes, and MUST NOT own the cross-context orchestration itself.
- **FR-013**: The system MUST record sufficient evidence on the Sales Order to demonstrate, at confirmation time, that both the payment authorization and the stock reservation decisions were made by their owning authorities.
- **FR-014**: Releasing or voiding a payment authorization when a stock reservation ultimately fails is entirely outside the Sales scope; the Payment authority owns that decision based on its own rules, and Sales MUST NOT signal, request, or perform it.

### Key Entities *(include if feature involves data)*

- **Sales Order** *(Sales-owned aggregate)*: A commercial commitment made by a customer. Holds its commercial status (including the new `Confirmed` outcome), the requested beers and quantities (order rows), and optional external decision references `PaymentAuthorizationId` and `StockReservationId`. These references may be empty during the lifecycle but MUST both be present at the moment of confirmation.
- **Payment Authorization** *(external decision, owned by Payment)*: An outcome indicating that a specified amount has been authorized for the customer/order. Referenced by Sales via `PaymentAuthorizationId`; never produced or owned by Sales.
- **Stock Reservation** *(external decision, owned by Warehouse)*: An outcome indicating that the requested physical stock has been reserved for the order. Referenced by Sales via `StockReservationId`; never produced or owned by Sales. It is the durable fact that supports confirmation (availability alone is not durable).
- **Sales Order Row**: A requested beer and quantity on the Sales Order; the basis for the stock-reservation request.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of Sales Orders that reach `Confirmed` have both a recorded payment-authorization reference and a recorded stock-reservation reference (zero confirmations without complete evidence).
- **SC-002**: 0 occurrences of a Sales Order in `Confirmed` status with a missing required reference across the full test suite (invariant never violated).
- **SC-003**: For every confirmation, the requested beers are reserved via a Warehouse reservation outcome before confirmation completes, with 0 cases of Sales directly mutating warehouse stock.
- **SC-004**: Re-processing the same confirmation trigger produces at most one confirmation outcome and at most one stock-reservation request (100% idempotency in the test suite).
- **SC-005**: When a required precondition is unmet, 100% of affected Sales Orders remain in a valid pre-confirmation status and none are left in an invalid persisted state.

## Assumptions

- The existing Sales Order lifecycle (created → accepted, etc.) and the existing saga/coordination that already gathers customer and availability signals remain in place; this feature adds the `Confirmed` outcome and the stock-reservation evidence on top of it.
- A Payment authorization outcome is (or will be) made available to Sales through an integration boundary; producing it is owned by the Payment authority and is out of scope here.
- A Warehouse stock-reservation outcome is (or will be) made available to Sales through an integration boundary; producing it is owned by the Warehouse authority and is out of scope here.
- "All requested beers are available in the warehouse" is satisfied for confirmation purposes by a durable Warehouse stock-reservation outcome, because availability by itself is not durable.
- Cross-context communication uses the project's established commands/integration events; no shared database tables or direct calls into another context's internals are introduced.

## Open Questions

<!-- Domain decisions the domain expert must resolve. The agent MUST NOT silently decide these (BC-011). -->

*Resolved (2026-06-25):*

- **Payment authorized but stock cannot be reserved** → The Sales Order remains in its pre-confirmation status; Payment independently handles any release/void; Sales takes no compensating action (FR-014).
- **Who coordinates the confirmation flow** → The existing Sales Order saga/process manager in the Sagas context (FR-012).
- **Releasing/voiding a payment authorization on reservation failure** → Entirely out of Sales scope; owned by Payment (FR-014).

*Still open:*

- May a Sales Order be partially reserved/confirmed when only some requested beers are reservable?
- How long can a stock reservation remain active while payment evidence is still pending?
- What does a payment-provider timeout mean — declined, pending, or unknown? (owned by Payment; Sales must not interpret it without an explicit decision)
- How long does Sales wait for a `StockReserved` outcome, and what happens on timeout?
- Who owns customer notification after a partial or full confirmation failure?
- Does the order of preconditions matter (payment first vs. reservation first), and which outcome must Sales receive before a Sales Order can become `Confirmed`?
- Do pre-approved payment terms (e.g., for wholesale customers) provide an alternative form of payment evidence, and if so, what evidence does Sales record?

## Out of Scope

- Authorizing, voiding, or refunding payments (owned by Payment).
- Reserving, releasing, decrementing, or otherwise mutating warehouse stock (owned by Warehouse).
- Reservation-expiration, retry, cancellation, and customer-notification policies (unless later assigned by an explicit domain decision).
- Releasing or voiding a payment authorization when stock reservation fails (owned by Payment).
- Cross-context orchestration of the confirmation flow (owned by the Sales Order saga in the Sagas context; Sales only reacts).
- Shipment creation and invoice generation.
