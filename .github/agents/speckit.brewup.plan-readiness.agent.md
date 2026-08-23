---
description: Check whether the BrewUp Sales Order specification is ready for planning.
handoffs:
  - label: Build Technical Plan
    agent: speckit.plan
    prompt: Create a technical plan from the Sales Order specification, preserving bounded-context ownership, external decision references, and unresolved domain decisions.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Plan Readiness Check

You are reviewing whether the current BrewUp Sales Order specification is ready for `/speckit.plan`.

This command runs before `/speckit.plan`.

Its purpose is not to generate the technical plan.

Its purpose is to decide whether the specification contains enough durable domain context to constrain planning safely.

A technical plan may choose coordination mechanisms.

A technical plan must not invent domain behavior.

A technical plan must not move decision authority across bounded-context boundaries.

## Goal

Ensure that the specification contains enough explicit domain language, ownership rules, external decision references, confirmation invariants, and open questions to guide planning without architectural drift.

Planning is allowed only if the plan can be generated without inventing domain decisions.

## Files to read

Read the following files if they exist:

* `.specify/memory/constitution.md`
* `specs/001-sales-order-confirmation/domain-carrier.md`
* `specs/001-sales-order-confirmation/spec.md`
* `specs/001-sales-order-confirmation/clarify-log.md`
* `specs/001-sales-order-confirmation/checklists/requirements.md`

If the active feature directory is different, resolve it from `.specify/feature.json` if available.

If `.specify/feature.json` does not exist, scan `specs/*/spec.md` and use the specification that most clearly refers to Sales Order confirmation.

If no specification file exists, stop and report:

```text
BrewUp Plan Readiness: BLOCKED
Reason: no specification file found.
```

Do not fail because optional context files are missing.

Report missing files and continue with the available information.

## Planning principle

The specification answers:

```text
What must happen?
Who owns the decision?
Which outcomes are valid?
What happens on failure?
```

The plan may answer:

```text
How is it coordinated?
Which component coordinates?
Which events, commands, retries, or handlers are needed?
Which technical mechanism is used?
```

The plan must not change the answers already recorded in the specification.

## Required specification content

The specification is ready for planning only if it explicitly contains the following sections or equivalent content.

### Ubiquitous Language

The specification must define:

* `Sales Order`
* `Payment Authorization`
* `Stock Reservation`
* `External Decision Reference`

The Sales-owned concept should be called `Sales Order`.

If the specification uses generic `Order`, it must clearly define that `Order` means `Sales Order` within this feature.

### Domain Ownership

The specification must state:

* Sales owns Sales Order lifecycle, commercial status, and customer demand.
* Payment owns payment authorization, authorization outcome, payment-provider timeout interpretation, void behavior, and refund behavior.
* Warehouse owns physical stock, stock availability, stock reservation, stock release, and reservation expiration.

### Bounded Context Rules

The specification must include rules equivalent to:

* BC-001 — Sales owns Sales Order lifecycle.
* BC-002 — Sales Order is the aggregate owned by Sales.
* BC-003 — Payment owns authorization outcomes.
* BC-004 — Payment Authorization is an external decision.
* BC-005 — Warehouse owns physical stock.
* BC-006 — Stock Reservation is an external decision.
* BC-007 — Warehouse owns stock mutation.
* BC-008 — Reacting is not owning.
* BC-009 — External decision references, not embedded models.
* BC-010 — Confirmed requires evidence.
* BC-011 — Clarification preserves authority.

### External Decision References

The specification must state that Sales may store external decision references such as:

* `PaymentAuthorizationId`
* `StockReservationId`

as evidence of external decisions.

The specification must not instruct Sales to load, manipulate, embed, or own Payment or Warehouse aggregates.

### Confirmation Invariant

The specification must state that a Sales Order cannot become `Confirmed` unless required external evidence is present.

For the base scenario, this means:

* `PaymentAuthorizationId` must be present.
* `StockReservationId` must be present.

During the lifecycle, these references may be empty.

At the moment a Sales Order becomes `Confirmed`, missing required evidence is an invalid state.

### Open Questions

The specification or clarify log must preserve unresolved business decisions, including:

* What happens if payment is authorized but stock cannot be reserved?
* How long can a stock reservation remain active while payment is pending?
* What does a payment-provider timeout mean: declined, pending, or unknown?
* May a Sales Order be partially reserved?
* Which exact outcome must Sales receive before the Sales Order can become `Confirmed`?
* Who owns customer notification after partial failure?
* Who coordinates the process?
* Who approves pre-approved payment terms for wholesale customers?
* How long is a payment terms approval valid?
* Can a payment terms approval be revoked?
* Does payment terms approval apply to the customer or to a specific Sales Order?
* What evidence does Sales need when confirmation is based on pre-approved payment terms?

Open questions do not necessarily block planning.

They block planning only if the plan would need to resolve them in order to proceed.

If an unresolved decision is out of scope for the current feature, it must be explicitly marked as out of scope or deferred.

## Readiness rules

The specification is ready for planning only if all these rules hold:

1. Sales, Payment, and Warehouse are modeled as separate authorities.
2. The Sales-owned aggregate is named `Sales Order` or clearly defined as such.
3. External decision references are explicit.
4. Payment and Warehouse domain models are not embedded in Sales.
5. The confirmation invariant is explicit.
6. Open questions remain visible.
7. Unresolved payment timeout behavior is not silently resolved.
8. Unresolved stock failure behavior is not silently resolved.
9. Unresolved cancellation behavior is not silently resolved.
10. Unresolved refund or void behavior is not silently resolved.
11. Unresolved stock release or expiration behavior is not silently resolved.
12. The specification distinguishes required behavior from technical coordination mechanism.
13. The plan can choose a coordination mechanism without changing decision ownership.
14. The specification gives the plan a stable basis for rejecting ownership drift.

## Blocking issues

Block planning if any of these are true:

* The specification still uses generic `Order` without defining it as `Sales Order`.
* The specification says Sales owns Payment decisions.
* The specification says Sales owns Warehouse decisions.
* The specification embeds Payment or Warehouse models inside Sales.
* The specification does not define `PaymentAuthorizationId`.
* The specification does not define `StockReservationId`.
* The specification allows `Confirmed` without required external evidence.
* The specification decides payment-provider timeout behavior without a domain decision.
* The specification decides refund, void, stock release, cancellation, retry, customer notification, or reservation expiration behavior without a domain decision.
* The specification chooses saga, process manager, application service, or agent before defining behavior and ownership.
* The specification does not preserve the clarify questions required to protect domain authority.
* The specification is so vague that the plan would need to invent ownership or policy.

## Allowed planning decisions

The plan may choose among technical coordination mechanisms, provided ownership remains unchanged.

Allowed examples:

* application service coordination;
* saga;
* process manager;
* message handler;
* workflow;
* agent-assisted coordination;
* commands to Payment and Warehouse;
* events or external inputs received from Payment and Warehouse;
* retries as technical delivery concerns, only if they do not imply domain policy.

The plan may describe:

* requesting Payment Authorization;
* requesting Stock Reservation;
* reacting to `PaymentAuthorized`;
* reacting to `StockReserved`;
* reacting to `StockReservationFailed`;
* storing `PaymentAuthorizationId`;
* storing `StockReservationId`;
* preventing `Confirmed` without required evidence.

## Forbidden planning decisions

The plan must not decide that Sales:

* authorizes payment;
* interprets payment-provider timeout;
* voids payment;
* issues refunds;
* checks authoritative physical stock;
* reserves stock;
* releases stock;
* decrements warehouse stock;
* creates shipments;
* issues invoices;
* automatically cancels Sales Orders after stock failure;
* automatically retries stock reservation;
* automatically notifies customers;
* releases stock after a default timeout;
* embeds Payment or Warehouse models inside the Sales Order aggregate.

## Output format

Return the result using this exact structure:

```text
BrewUp Plan Readiness: PASS|BLOCKED

Checked files:
- ...

Missing optional files:
- ...

Planning allowed: YES|NO

Readiness summary:
- ...

Required sections:
- Ubiquitous Language: PRESENT|MISSING|INCOMPLETE
- Domain Ownership: PRESENT|MISSING|INCOMPLETE
- Bounded Context Rules: PRESENT|MISSING|INCOMPLETE
- External Decision References: PRESENT|MISSING|INCOMPLETE
- Confirmation Invariant: PRESENT|MISSING|INCOMPLETE
- Open Questions: PRESENT|MISSING|INCOMPLETE
- Out of Scope / Deferred Decisions: PRESENT|MISSING|INCOMPLETE

Blocking issues:
- ...

Open questions that must remain open:
- ...

Open questions that block planning:
- ...

Instructions for /speckit.plan:
- Do not embed Payment or Warehouse models inside Sales.
- Do not make Sales authorize payment.
- Do not make Sales reserve or mutate stock.
- Do not make Sales issue refunds, voids, shipments, or invoices.
- Use external decision references.
- Preserve the Confirmed evidence invariant.
- Keep unresolved policies as explicit open questions or blocked work.
- Choose a coordination mechanism only after preserving behavior and ownership.
```

If planning is allowed, include:

```text
Decision:
- Safe to continue to /speckit.plan: YES
```

If planning is blocked, include:

```text
Decision:
- Safe to continue to /speckit.plan: NO
- Required action: fix the specification or ask a domain expert.
```

## Expected PASS example

A PASS result should look like this:

```text
BrewUp Plan Readiness: PASS

Planning allowed: YES

Readiness summary:
- The specification defines Sales, Payment, and Warehouse as separate authorities.
- Sales owns Sales Order lifecycle only.
- PaymentAuthorizationId and StockReservationId are modeled as external decision references.
- Confirmed requires required external evidence.
- Open questions remain visible and must not be resolved by the plan.

Instructions for /speckit.plan:
- Generate a coordination plan without moving Payment or Warehouse behavior into Sales.
- Do not implement refund, void, stock release, timeout interpretation, shipment, or invoicing.
- Mark unresolved business decisions as blocked or out of scope.
```

## Expected BLOCKED example

A BLOCKED result should look like this:

```text
BrewUp Plan Readiness: BLOCKED

Planning allowed: NO

Blocking issues:
- Missing Confirmation Invariant: the specification does not state that Confirmed requires PaymentAuthorizationId and StockReservationId.
- Missing External Decision References: the specification mentions payment and stock outcomes but does not define the evidence Sales must store.
- Policy invention risk: the plan would need to decide what happens after StockReservationFailed.

Decision:
- Safe to continue to /speckit.plan: NO
- Required action: fix the specification or ask a domain expert.
```

## Constraints

Do not create files.

Do not modify files.

Do not generate the plan.

Do not generate tasks.

Do not generate code.

Do not resolve open questions.

Do not invent domain policy.

Do not choose a coordination mechanism.

Do not move Payment or Warehouse behavior into Sales.

Your job is to decide whether planning can start without architectural drift being baked into the plan.
