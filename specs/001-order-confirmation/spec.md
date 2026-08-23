# Feature Specification: Sales Order Confirmation

**Feature Branch**: `001-order-confirmation`

**Created**: 2026-06-27

**Status**: Draft

**Input**: User description: "Implement order confirmation for BrewUp. An order can be confirmed when: the customer payment has been authorized; all requested beers are available in the warehouse. When the order is confirmed, reserve the stock."

## Domain Authorities & Ownership *(mandatory — preserves BC-000 … BC-011)*

This feature spans **three separate domain authorities**. Each owns its own decisions. None may own or reproduce another's decision.

| Authority | Owns | Must NOT own (in this feature) |
|---|---|---|
| **Sales** | Sales Order lifecycle, commercial status, customer demand, the transition to `Confirmed` once required external evidence is present | Payment authorization, stock availability-as-truth, stock reservation/release, refunds, voids, shipment, invoicing |
| **Payment** | Payment authorization request and outcome, provider-timeout interpretation, void, refund | The Sales Order lifecycle; stock |
| **Warehouse** | Physical stock, availability-as-truth, stock reservation, stock release, reservation expiration | The Sales Order lifecycle; payment |

**Authoritative carried rules** (from the domain carrier; generated artifacts MUST preserve these IDs):

- **BC-001 / BC-002** — Sales owns the Sales Order aggregate and its lifecycle; it must not embed Payment or Warehouse domain models.
- **BC-003 / BC-004** — Payment Authorization is an **external decision** produced by Payment. Sales may *request* it and *react* to its outcome, storing `PaymentAuthorizationId` as evidence; Sales must not produce the authorization.
- **BC-005 / BC-006 / BC-007** — Stock Reservation is an **external decision** produced by Warehouse. Sales may *request* it and *react* to its outcome, storing `StockReservationId` as evidence; Sales must not reserve, release, or decrement stock. Availability is **not** a durable fact — only a Stock Reservation is durable enough to support confirmation.
- **BC-008 / BC-009** — Reacting to an outcome is not owning it. Sales stores external decision references only as evidence, never as a back door into another context's aggregate.
- **BC-010** — A Sales Order must not become `Confirmed` unless the required external decision references (`PaymentAuthorizationId` **and** `StockReservationId`) are both present.
- **BC-011** — Unresolved business policy stays explicit (see *Open Questions*); the spec must not silently decide it.

> Terminology: this feature uses **Sales Order**, never generic *Order*, for the Sales-owned aggregate.

## Clarifications

### Session 2026-06-27

- Q: How should a Sales Order become Confirmed once both evidence references exist? → A: A coordinator dispatches a Confirm command to Sales when the confirmation conditions hold; Sales still owns and performs the transition (resolves OQ-6).
- Q: Can a Sales Order be confirmed when only some requested beers can be reserved? → A: Yes — partial confirmation is allowed; the order confirms for the reservable subset Warehouse reserves (resolves OQ-2).
- Q: What happens when one evidence succeeds but the other fails? → A: Remain unconfirmed; no Sales-owned compensation; release/void/refund stay with Warehouse/Payment and are out of scope (resolves OQ-1).
- Q: Should payment authorization and stock reservation be requested in parallel or in sequence? → A: In parallel (resolves OQ-5).
- Q: How should Sales behave toward a payment-provider timeout? → A: Sales does not interpret timeouts; it reacts only to definitive Payment outcomes that Payment emits (resolves OQ-3).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Confirm a Sales Order backed by authorized payment and reserved stock (Priority: P1)

A customer has placed a Sales Order for one or more beers. Before the order is committed as a binding sale, the business needs assurance that the customer can pay and that the beers can actually be delivered. The Sales Order becomes `Confirmed` only when Payment has authorized the customer's payment **and** Warehouse has produced a durable Stock Reservation for all requested beers. Confirmation is the commercial commitment that downstream fulfilment relies on.

**Why this priority**: This is the core of the feature and the minimum viable outcome. Without it there is no Sales Order confirmation. It exercises the full evidence-gated confirmation invariant across all three authorities.

**Independent Test**: Given a placed Sales Order, simulate a Payment authorization outcome and a Warehouse stock reservation outcome for the requested beers, then verify the Sales Order transitions to `Confirmed` and records both `PaymentAuthorizationId` and `StockReservationId`.

**Acceptance Scenarios**:

1. **Given** a placed Sales Order with requested beers and no external evidence yet, **When** Payment reports the customer payment as authorized and Warehouse reports the requested beers as reserved, and the coordinator dispatches the Confirm command, **Then** the Sales Order becomes `Confirmed` and stores both `PaymentAuthorizationId` and `StockReservationId`.
2. **Given** a placed Sales Order, **When** only the payment authorization evidence is present and the stock reservation evidence is absent, **Then** the Sales Order does **not** become `Confirmed`.
3. **Given** a placed Sales Order, **When** only the stock reservation evidence is present and the payment authorization evidence is absent, **Then** the Sales Order does **not** become `Confirmed`.
4. **Given** a Sales Order that is already `Confirmed`, **When** the same confirmation evidence arrives again or the Confirm command is dispatched again, **Then** the Sales Order remains `Confirmed` exactly once and no duplicate commitment is recorded (idempotent confirmation).
5. **Given** a placed Sales Order, **When** Payment authorizes the payment and Warehouse reserves a subset of the requested beers, and the coordinator dispatches the Confirm command, **Then** the Sales Order becomes `Confirmed` for the reserved subset and stores the `StockReservationId` for that reservation (partial confirmation).

---

### User Story 2 - Request the external decisions that confirmation depends on (Priority: P2)

For a Sales Order to gather the evidence it needs, Sales must ask Payment to authorize the customer's payment and ask Warehouse to reserve stock for the requested beers. Sales issues these requests across an integration boundary and then waits to react to the outcomes; it never performs authorization or reservation itself.

**Why this priority**: Confirmation (P1) reacts to outcomes; this story produces the requests that cause those outcomes to exist. It is required for an end-to-end flow but is separable from the pure confirmation invariant, which can be tested by injecting outcomes directly.

**Independent Test**: Given a placed Sales Order, verify that Sales emits a request for payment authorization to Payment and a request for stock reservation to Warehouse for the requested beers, without Sales performing either decision.

**Acceptance Scenarios**:

1. **Given** a placed Sales Order, **When** the order enters the confirmation flow, **Then** Sales requests payment authorization from Payment for the order's amount.
2. **Given** a placed Sales Order, **When** the order enters the confirmation flow, **Then** Sales requests a stock reservation from Warehouse for all requested beers.
3. **Given** Sales has requested an external decision, **When** the corresponding outcome (authorized / reserved, or the negative outcomes) is reported, **Then** Sales records the outcome as evidence and reacts accordingly — it does not re-decide the outcome.

---

### User Story 3 - Withhold confirmation when evidence is negative or missing (Priority: P2)

When Payment declines authorization, or Warehouse cannot reserve all requested beers, the required evidence for confirmation does not exist. The Sales Order must remain unconfirmed and must surface the unmet condition rather than silently confirming or inventing a remediation policy.

**Why this priority**: Protects the confirmation invariant against false positives. The negative path is as important as the happy path for a binding commercial commitment.

**Independent Test**: Given a placed Sales Order, report a declined payment authorization and/or a rejected stock reservation, and verify the Sales Order stays unconfirmed.

**Acceptance Scenarios**:

1. **Given** a placed Sales Order, **When** Payment reports the authorization as declined, **Then** the Sales Order does **not** become `Confirmed`.
2. **Given** a placed Sales Order, **When** Warehouse reports that one or more requested beers cannot be reserved, **Then** the Sales Order does **not** become `Confirmed`.
3. **Given** a placed Sales Order with no evidence, **When** no outcomes have arrived, **Then** the Sales Order remains in its pre-confirmation status and exposes which evidence is still outstanding.

---

### Edge Cases

- **Partial stock**: Warehouse can reserve some, but not all, requested beers. Partial confirmation is allowed (clarified OQ-2): the Sales Order confirms for the reservable subset; handling of the unreserved remainder is not owned by Sales and is out of scope here.
- **One side succeeds, the other fails**: payment authorized but nothing can be reserved, or stock reserved but payment declined. The Sales Order remains unconfirmed with **no Sales-owned compensation** (clarified OQ-1); release/void/refund stay with Warehouse/Payment and are out of scope.
- **Payment-provider timeout**: the provider neither clearly authorizes nor declines. Sales does **not** interpret it (clarified OQ-3); Sales reacts only to definitive Payment outcomes (authorized / declined / explicit unknown) that Payment emits.
- **Reservation lifetime**: a Stock Reservation may expire while payment is still pending. Expiration policy is owned by **Warehouse** — see OQ-4.
- **Duplicate / out-of-order outcomes**: the same outcome arrives twice, or outcomes arrive in an unexpected order. Confirmation must be idempotent (FR-009).
- **Evidence for a non-existent or already-confirmed Sales Order**: outcomes reference a Sales Order that is missing or already `Confirmed`.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Sales MUST own the Sales Order lifecycle and the transition to `Confirmed`. (BC-001)
- **FR-002**: A Sales Order MUST become `Confirmed` only when **both** of the following external decision references are present: `PaymentAuthorizationId` (evidence that Payment authorized the customer payment) and `StockReservationId` (evidence that Warehouse reserved the requested beers). Under the partial-confirmation policy (FR-014), the reservation referenced by `StockReservationId` MAY cover all or a subset of the requested beers. (BC-010)
- **FR-003**: Sales MUST store `PaymentAuthorizationId` and `StockReservationId` as **external decision references** (evidence only), and MUST NOT embed Payment or Warehouse domain models inside the Sales Order. (BC-002, BC-009)
- **FR-004**: Sales MUST NOT authorize payment, interpret payment-provider timeouts, void authorizations, or issue refunds. These remain Payment's decisions. Sales MUST react only to **definitive** Payment outcomes (authorized / declined / explicit unknown) emitted by Payment. (BC-003, BC-004)
- **FR-005**: Sales MUST NOT check physical stock as the authority of truth, nor reserve, release, or decrement stock. These remain Warehouse's decisions. (BC-005, BC-006, BC-007)
- **FR-006**: Sales MUST treat beer availability as non-durable; a Sales Order MUST rely on a durable **Stock Reservation** outcome from Warehouse — not a transient availability check — as the stock evidence for confirmation. (BC-006, BC-007)
- **FR-007**: Sales MUST be able to **request** payment authorization from Payment and **request** stock reservation from Warehouse across an integration boundary, **in parallel** (the two requests have no ordering dependency), without performing either decision itself. (BC-004, BC-006, BC-008)
- **FR-008**: Sales MUST react to Payment and Warehouse outcomes (authorized/declined, reserved/rejected) by recording the corresponding evidence and updating Sales Order status, without re-deciding those outcomes. (BC-008)
- **FR-009**: Confirmation MUST be idempotent: receiving the required evidence more than once, or out of order, MUST NOT produce duplicate confirmation or duplicate commercial commitment.
- **FR-010**: While required evidence is incomplete, the Sales Order MUST remain unconfirmed and MUST expose which evidence is still outstanding.
- **FR-011**: When Payment reports a declined authorization, or Warehouse reports that none of the requested beers can be reserved, the Sales Order MUST remain unconfirmed. When one required outcome succeeds and the other is negative or absent, the Sales Order MUST remain unconfirmed with **no Sales-owned compensation**; release, void, and refund remain owned by Warehouse and Payment and are out of scope for this feature. (clarified OQ-1)
- **FR-012**: Confirmation evidence MUST be associated with the specific Sales Order it pertains to; evidence referencing an unknown Sales Order MUST NOT confirm any order.
- **FR-013**: Cross-context interaction for this feature MUST occur only through explicit contracts (commands, integration events, facades) — never direct access into another context's internal domain. (Constitution II)
- **FR-014**: Partial confirmation is permitted. Warehouse owns which requested beers are reservable; a Sales Order MAY become `Confirmed` when payment is authorized and Warehouse produces a Stock Reservation covering a subset of the requested beers. Sales MUST NOT decide the reservable subset, and the handling of any unreserved remainder is not owned by Sales and is out of scope here. (clarified OQ-2; BC-005, BC-006)
- **FR-015**: Confirmation MUST be triggered by an external coordinator that dispatches a Confirm command to Sales once the confirmation conditions hold. Sales still **owns and performs** the transition to `Confirmed` and enforces the confirmation invariant; the coordinator coordinates only and MUST NOT own the confirmation decision. The specific coordination mechanism (e.g. saga, process manager, application service) is a plan-time choice. (clarified OQ-5, OQ-6; BC-008)

*Requirements deferred to domain-authority resolution are captured in the Open Questions section rather than as silent decisions (BC-011).*

### Key Entities *(include if feature involves data)*

- **Sales Order** *(owned by Sales)*: The commercial commitment made by a customer. Holds requested beers (customer demand), commercial status (including `Confirmed`), and the external decision references `PaymentAuthorizationId?` and `StockReservationId?`. Does not embed Payment or Warehouse models.
- **Payment Authorization** *(owned by Payment; external to Sales)*: The outcome indicating a specified amount has been authorized for the customer. Referenced from Sales only by `PaymentAuthorizationId`.
- **Stock Reservation** *(owned by Warehouse; external to Sales)*: The durable outcome indicating that the requested beers' physical stock has been reserved. Referenced from Sales only by `StockReservationId`.
- **External Decision Reference**: A reference stored by Sales as evidence that an external authority made a decision Sales depends on. It is evidence, not a handle to another context's aggregate.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of Sales Orders that reach `Confirmed` have both a payment-authorization reference and a stock-reservation reference recorded at the moment of confirmation (zero confirmations missing required evidence).
- **SC-002**: 0% of Sales Orders become `Confirmed` when either required evidence is absent or negative, across all happy-path and negative-path acceptance scenarios.
- **SC-003**: Repeated or out-of-order delivery of confirmation evidence results in exactly one confirmation per Sales Order (no duplicate commitments) in 100% of idempotency tests.
- **SC-004**: Architecture fitness checks confirm that Sales never performs payment authorization or stock mutation and never references Payment or Warehouse internal domain types — 0 violations.
- **SC-005**: For any unconfirmed Sales Order, the set of outstanding required evidence can be determined unambiguously at any point in its lifecycle.

## Assumptions

- Payment and Warehouse are **separate authorities** for this feature. Whether either is implemented as a BrewUp module in this repository, referenced as an existing module, or treated as external is a **planning** decision deferred to `/speckit.plan` (per repository instructions, if Payment behavior is implemented here it MUST be a full Payment module).
- Partial confirmation is allowed: a Sales Order may be confirmed for the subset of requested beers Warehouse reserves (clarified OQ-2). Handling of any unreserved remainder is not owned by Sales and is out of scope here.
- An external coordinator requests the two external decisions **in parallel** and dispatches a Confirm command to Sales once the confirmation conditions hold; it coordinates without owning the confirmation decision (clarified OQ-5, OQ-6). The concrete mechanism is chosen at plan time.
- "All requested beers are available" is satisfied, for the purposes of a durable commitment, by a **Stock Reservation** outcome rather than a transient availability check (consistent with BC-006/BC-007).
- A Sales Order has already been placed (created and placed) before this confirmation flow begins; creation/placement is out of scope for this feature.
- Standard event-driven, message-coupled communication between contexts is reused; no shared database coupling is introduced.

## Open Questions *(BC-011 — must be resolved by a domain authority, not silently decided)*

These preserve unresolved business policy. They MUST remain explicit until a human domain/architecture authority resolves them (e.g., via `/speckit.clarify`). They MUST NOT be turned into implementation choices by later artifacts.

**Resolved in the 2026-06-27 clarification session** (see *Clarifications*): OQ-1 (one-sided outcome → remain unconfirmed, no Sales-owned compensation), OQ-2 (partial confirmation allowed), OQ-3 (Sales does not interpret payment timeouts), OQ-5 (parallel requests), OQ-6 (coordinator dispatches a Confirm command).

Still open (lower impact / owned by another authority — deferred):

- **OQ-4 — Stock reservation lifetime**: How long may a Stock Reservation remain active while payment is still pending, and what happens on expiry? This is a **Warehouse** decision.
- **OQ-7 — Customer notification & downstream**: Who owns notifying the customer of confirmation/failure, and what (if anything) is triggered downstream (shipment, invoicing)? Shipment and invoicing are explicitly **not** owned by Sales in this feature.
