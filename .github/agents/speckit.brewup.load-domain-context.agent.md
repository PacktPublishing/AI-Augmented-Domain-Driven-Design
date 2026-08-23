---
description: Load BrewUp Sales Order domain context before specification.
handoffs:
  - label: Create Specification
    agent: speckit.specify
    prompt: Create or update the feature specification using this BrewUp Sales Order domain context.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Sales Order Domain Context Loader

You are preparing the domain context for a Spec Kit specification.

This command runs before `/speckit.specify`.

Its purpose is not to generate the specification.

Its purpose is to make the domain language, ownership boundaries, external decision references, forbidden responsibilities, and open questions explicit before the specification is created.

## Goal

Load the BrewUp Sales Order domain context and produce a concise, authoritative context summary that must guide the next `/speckit.specify` execution.

The output of this command should help the agent avoid:

* generic `Order` language where `Sales Order` is required;
* ownership overreach;
* embedded external domain models;
* invented business policy;
* silent defaults;
* generic e-commerce assumptions;
* confusing dependency on a decision with ownership of the decision;
* confusing coordination with decision authority;
* confusing external decision references with remote aggregate access.

## Files to read

Read the following files if they exist:

* `.specify/memory/constitution.md`
* `specs/001-sales-order-confirmation/domain-carrier.md`
* `00-discovery/running-scenario.md`
* `00-discovery/candidate-events.md`

If a file does not exist, report it as missing and continue.

Do not fail because optional files are missing.

## Domain context

The current feature is about Sales Order confirmation in BrewUp.

BrewUp is a brewery domain.

For this feature, BrewUp has three separate domain authorities:

```text
Sales        — commercial commitment
Payment      — payment authorization
Warehouse    — physical stock
```

Sales and Warehouse are existing bounded contexts.

Payment is introduced as a separate authority for this story.

The running scenario is:

```text
Implement order confirmation for BrewUp.

A Sales Order can be confirmed when:
- the customer payment has been authorized;
- all requested beers have been reserved in the warehouse.

When the Sales Order is confirmed, Sales records the evidence required for confirmation.
```

The word `confirmed` is domain-sensitive and must not be interpreted generically.

A Sales Order must not become `Confirmed` just because:

* the order was submitted;
* payment was requested;
* stock looked available at a point in time;
* a coordination workflow was started;
* an implementation plan says the happy path is complete.

## Ubiquitous Language

Use this language consistently.

### Sales Order

A commercial commitment made by a customer.

Sales owns the Sales Order lifecycle, commercial status, and customer demand.

Use `Sales Order`, not generic `Order`, when referring to the Sales-owned aggregate.

### Payment Authorization

An outcome produced by Payment indicating that a specified amount has been authorized.

Sales may depend on this outcome.

Sales does not produce this outcome.

### Stock Reservation

An outcome produced by Warehouse indicating that specified physical stock has been reserved.

Sales may depend on this outcome.

Sales does not produce this outcome.

Availability is not a durable fact.

Only a reservation is durable enough to support Sales Order confirmation.

### External Decision Reference

A reference stored by Sales as evidence that an external authority made a decision Sales depends on.

Examples:

* `PaymentAuthorizationId`
* `StockReservationId`
* `PaymentTermsApprovalId`

An external decision reference is not a back door through which Sales loads or manipulates another bounded context's aggregate.

## Authoritative ownership rules

The following rules are authoritative for this feature.

### Sales owns

Sales owns:

1. Sales Order lifecycle.
2. Commercial status.
3. Customer demand.
4. The decision that a Sales Order can transition to `Confirmed`, once required external evidence is present.

Sales does not own the decisions that produce the external evidence.

### Payment owns

Payment owns:

1. Payment authorization.
2. Authorization outcome.
3. Payment-provider timeout interpretation.
4. Void behavior.
5. Refund behavior.

### Warehouse owns

Warehouse owns:

1. Physical stock.
2. Stock availability.
3. Stock reservation.
4. Stock release.
5. Reservation expiration.

## Bounded Context Rules

The following rules must be preserved by `/speckit.specify`.

### BC-001 — Sales owns Sales Order lifecycle

Sales owns the Sales Order lifecycle, commercial status, and customer demand.

### BC-002 — Sales Order is the aggregate owned by Sales

Sales owns the Sales Order aggregate.

Sales must not embed Payment or Warehouse domain models inside the Sales Order aggregate.

### BC-003 — Payment owns authorization outcomes

Payment owns payment authorization, authorization outcome, payment-provider timeout interpretation, void behavior, and refund behavior.

### BC-004 — Payment Authorization is an external decision

Sales may store a `PaymentAuthorizationId` as evidence that Payment produced an authorization outcome.

Sales must not authorize payment.

### BC-005 — Warehouse owns physical stock

Warehouse owns physical stock, stock availability, stock reservation, stock release, and reservation expiration.

### BC-006 — Stock Reservation is an external decision

Sales may store a `StockReservationId` as evidence that Warehouse produced a reservation outcome.

Sales must not reserve stock.

### BC-007 — Warehouse owns stock mutation

Sales must not decrement stock, reserve stock, release stock, or decide stock availability as the authority of truth.

### BC-008 — Reacting is not owning

Sales may react to Payment and Warehouse outcomes.

Sales must not produce those outcomes.

### BC-009 — External decision references, not embedded models

Sales may store external decision references such as `PaymentAuthorizationId`, `StockReservationId`, and, if later approved, `PaymentTermsApprovalId`.

Sales must not embed Payment or Warehouse domain models.

### BC-010 — Confirmed requires evidence

A Sales Order must not become `Confirmed` unless the required external decision references are present.

For the base scenario:

* `PaymentAuthorizationId` is required.
* `StockReservationId` is required.

During the lifecycle, these references may be empty.

At the moment a Sales Order becomes `Confirmed`, missing required evidence is an invalid state.

### BC-011 — Clarification preserves authority

When business behavior is unclear, the specification must preserve the ambiguity as an open question or `[NEEDS CLARIFICATION]`.

The agent must not silently decide.

## External decisions

Sales depends on external decisions produced elsewhere.

Sales may store:

* `PaymentAuthorizationId`
* `StockReservationId`

Sales may later store:

* `PaymentTermsApprovalId`

only if the specification evolves to support pre-approved payment terms.

Sales must not embed:

* `Payment`
* `PaymentAuthorization`
* `WarehouseReservation`
* `StockReservation`
* `WarehouseStock`

as owned domain models inside the Sales Order aggregate.

## Forbidden responsibilities

Sales must not:

* authorize payments;
* interpret payment-provider timeouts as declined, authorized, or pending unless specified;
* void payment authorizations;
* issue refunds;
* check physical stock as the authority of truth;
* reserve stock;
* release stock;
* decrement warehouse stock;
* create shipments;
* issue invoices;
* invent retry policy;
* invent cancellation policy;
* invent customer notification policy;
* invent reservation expiration policy.

If any of these behaviors are required, they must be modeled as:

* external responsibilities;
* out-of-scope behavior;
* or explicit open questions.

## Policy invention rules

The agent must not invent business policy.

In particular, the agent must not assume that:

* stock failure automatically cancels the Sales Order;
* stock failure automatically triggers a refund;
* payment authorization alone means the Sales Order is confirmed;
* Sales Order submission means the Sales Order is confirmed;
* checking availability is equivalent to reserving stock;
* shipment starts automatically after confirmation;
* invoice generation belongs to Sales;
* a payment-provider timeout means declined;
* a payment-provider timeout means authorized;
* stock release happens after a default timeout;
* pre-approved payment terms apply without an explicit domain rule.

Unknown business decisions must remain explicit.

Use one of these forms:

```markdown
[NEEDS CLARIFICATION: ...]
```

or:

```markdown
Open question: ...
```

## Open questions

The following decisions must not be silently resolved:

* What happens if payment is authorized but stock cannot be reserved?
* How long can a stock reservation remain active while payment is pending?
* What does a payment-provider timeout mean: declined, pending, or unknown?
* May a Sales Order be partially reserved?
* Which exact outcome must Sales receive before the Sales Order can become `Confirmed`?
* Who owns customer notification after partial failure?
* Who coordinates the process: saga, process manager, application service, or agent?
* Who approves pre-approved payment terms for wholesale customers?
* How long is a payment terms approval valid?
* Can a payment terms approval be revoked?
* Does payment terms approval apply to the customer or to a specific Sales Order?
* What evidence does Sales need when confirmation is based on pre-approved payment terms?

## Required output

Return a concise context summary with the following sections:

```markdown
# BrewUp Sales Order Context

## Confirmed facts

- ...

## Owned by Sales

- ...

## Owned by Payment

- ...

## Owned by Warehouse

- ...

## External decision references

- ...

## Forbidden ownership

- ...

## Confirmation invariant

- ...

## Open questions

- ...

## Rules for /speckit.specify

- ...
```

## Output constraints

The output must be short enough to be pasted into, or referenced by, the next `/speckit.specify` command.

Do not create or modify files.

Do not generate implementation tasks.

Do not generate code.

Do not resolve open questions.

Do not invent domain policy.

Do not choose a coordination mechanism.

Do not turn external references into remote aggregate access.

Your job is to prepare the reasoning environment, not to design the full system.
