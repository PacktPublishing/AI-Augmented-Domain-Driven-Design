---
description: Validate that the generated BrewUp Sales Order specification respects Sales, Payment, and Warehouse ownership.
handoffs:
  - label: Clarify Specification
    agent: speckit.clarify
    prompt: Clarify the Sales Order specification and preserve unresolved domain decisions as explicit questions.
  - label: Build Technical Plan
    agent: speckit.plan
    prompt: Create a technical plan from the Sales Order specification, preserving bounded-context ownership and external decision references.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Domain Guard

You are reviewing a Spec Kit specification for the BrewUp Sales Order confirmation feature.

This command usually runs after `/speckit.specify`.

Its purpose is not to improve the specification creatively.

Its purpose is to verify whether the generated specification respects the domain language, ownership boundaries, external decision references, forbidden responsibilities, and unresolved business decisions defined for the current feature.

## Goal

Check the generated specification and report whether it violates BrewUp Sales Order domain rules.

The most important risks to detect are:

* generic `Order` language where `Sales Order` is required;
* Sales owning Payment behavior;
* Sales owning Warehouse behavior;
* embedded external domain models inside the Sales Order aggregate;
* payment authorization performed by Sales;
* stock reservation or stock mutation performed by Sales;
* external decisions converted into local entity behavior;
* open questions silently resolved by the model;
* technical coordination choices that invent domain behavior.

## Files to read

Read the following files if they exist:

* `.specify/memory/constitution.md`
* `specs/001-sales-order-confirmation/domain-carrier.md`
* `specs/001-sales-order-confirmation/spec.md`
* `specs/001-sales-order-confirmation/clarify-log.md`

If `specs/001-sales-order-confirmation/spec.md` does not exist, try to locate the current active specification under:

* `specs/*/spec.md`

If multiple specification files exist, use the one that most clearly refers to Sales Order confirmation.

If no specification file exists, stop and report:

```text
BrewUp Domain Guard: BLOCKED
Reason: no specification file found.
```

Do not fail because optional context files are missing.

Report missing files and continue with the available information.

## Domain model to protect

BrewUp has three separate domain authorities for this feature:

```text
Sales        — commercial commitment
Payment      — payment authorization
Warehouse    — physical stock
```

Sales owns the Sales Order lifecycle.

Payment owns payment authorization outcomes.

Warehouse owns physical stock and stock reservation outcomes.

Sales may depend on decisions produced by Payment and Warehouse.

Sales must not own the models or processes that produce those decisions.

## Authoritative bounded-context rules

The generated specification passes only if all the following rules are respected.

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

Sales must not interpret payment-provider timeouts unless the specification records an explicit Payment-domain decision.

### BC-005 — Warehouse owns physical stock

Warehouse owns physical stock, stock availability, stock reservation, stock release, and reservation expiration.

### BC-006 — Stock Reservation is an external decision

Sales may store a `StockReservationId` as evidence that Warehouse produced a reservation outcome.

Sales must not reserve stock.

### BC-007 — Warehouse owns stock mutation

Sales must not decrement stock, reserve stock, release stock, or decide stock availability as the authority of truth.

Availability is not a durable fact.

Only a Stock Reservation is durable enough to support Sales Order confirmation.

### BC-008 — Reacting is not owning

Sales may react to Payment and Warehouse outcomes.

Sales must not produce those outcomes.

Dependency on a decision is not ownership of the decision.

### BC-009 — External decision references, not embedded models

Sales may store external decision references such as:

* `PaymentAuthorizationId`
* `StockReservationId`
* `PaymentTermsApprovalId`, only if the specification evolves to support pre-approved payment terms.

Sales must not embed Payment or Warehouse domain models.

An external decision reference is not a back door through which Sales loads or manipulates another bounded context's aggregate.

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

## Failure patterns

Fail the review if the specification contains any of these patterns without an explicit domain decision.

### Generic or ambiguous language

```text
Order
```

when the intended Sales-owned concept is:

```text
Sales Order
```

This is not always a critical failure by itself, but it must be reported because generic language gives the model room to make generic decisions.

### Embedded external models

```text
SalesOrder contains Payment
SalesOrder contains PaymentAuthorization as an owned entity
SalesOrder contains WarehouseReservation
SalesOrder contains StockReservation as an owned entity
SalesOrder contains WarehouseStock
```

### Wrong authority

```text
Sales authorizes payment
Sales voids payment
Sales issues refund
Sales checks authoritative stock availability
Sales reserves stock
Sales releases stock
Sales decrements warehouse stock
Sales creates shipment
Sales issues invoice
```

### Wrong aggregate behavior

Fail if the specification implies behavior like this inside the Sales Order aggregate:

```csharp
Payment.Authorize(...);
WarehouseReservation.Reserve(...);
WarehouseStock.Decrement(...);
Status = OrderStatus.Confirmed;
```

or:

```csharp
public void Confirm()
{
    Payment.Authorize(Total);

    foreach (var line in Lines)
        WarehouseReservation.Reserve(line.BeerId, line.Quantity);

    Status = OrderStatus.Confirmed;
}
```

The problem is not the syntax.

The problem is that the Sales Order aggregate is being assigned decisions that belong to Payment and Warehouse.

### Wrong confirmation rule

Fail if the specification allows or implies that:

```text
Sales Order becomes Confirmed without PaymentAuthorizationId.
Sales Order becomes Confirmed without StockReservationId.
Sales Order becomes Confirmed after checking availability, without a durable reservation.
Sales Order becomes Confirmed after payment request is sent, without authorization evidence.
Sales Order treats payment timeout as Declined without a domain decision.
Sales Order treats payment timeout as Authorized without a domain decision.
Sales Order treats stock availability check as durable evidence.
```

### Silent policy invention

Fail if the specification decides any of the following without an explicit domain decision:

```text
If stock reservation fails, cancel the Sales Order.
If stock reservation fails, refund the customer.
If stock reservation fails, void the payment.
If stock reservation fails, retry automatically.
If payment fails, release stock after a default timeout.
If payment provider times out, mark payment as declined.
If payment provider times out, mark payment as authorized.
If stock is unavailable, automatically create a backorder.
If confirmation succeeds, create a shipment.
If confirmation succeeds, issue an invoice.
```

These may be valid business policies.

They are not valid unless recorded as domain decisions.

## What counts as PASS

The review passes if:

1. The Sales-owned concept is named `Sales Order`, or the specification clearly defines `Order` as Sales Order.
2. Sales owns the Sales Order lifecycle and commercial status.
3. Payment owns payment authorization and related outcomes.
4. Warehouse owns physical stock and reservation outcomes.
5. Sales stores external decision references instead of embedding external models.
6. `PaymentAuthorizationId` is modeled as evidence of a Payment decision.
7. `StockReservationId` is modeled as evidence of a Warehouse decision.
8. A Sales Order cannot become `Confirmed` without required external evidence.
9. Unresolved business decisions remain visible.
10. The specification does not convert uncertainty into silent defaults.
11. Coordination behavior is separated from decision ownership.
12. The technical coordination mechanism is not chosen before behavior and ownership are clear.

## What counts as FAIL

The review fails if:

1. Payment or Warehouse domain models are embedded into the Sales Order aggregate.
2. Sales authorizes payment.
3. Sales reserves or mutates stock.
4. Sales issues refunds or voids payment.
5. Sales creates shipment or invoice responsibilities.
6. External outcomes are modeled as locally produced facts.
7. Missing business decisions are silently resolved.
8. A Sales Order can become `Confirmed` without required external decision references.
9. The specification looks complete by hiding uncertainty.

## Severity levels

Use these severity levels.

### CRITICAL

Use CRITICAL when the specification violates ownership or decision authority.

Examples:

* Sales authorizes payment.
* Sales reserves or decrements stock.
* Payment or Warehouse models are embedded in Sales Order.
* Confirmed can happen without required evidence.
* Open questions are silently resolved as approved policy.

### HIGH

Use HIGH when the specification misses a required invariant, rule, or external decision reference.

Examples:

* The specification does not define `PaymentAuthorizationId`.
* The specification does not define `StockReservationId`.
* The specification does not define the Confirmed evidence invariant.
* The specification does not preserve clarification questions.

### MEDIUM

Use MEDIUM when terminology is imprecise but ownership is still correct.

Examples:

* The specification uses `Order` where `Sales Order` should be used, but the intended context is still clear.

### LOW

Use LOW for minor wording or documentation improvements.

## Output format

Return the result using this exact structure:

```text
BrewUp Domain Guard: PASS|FAIL|BLOCKED

Checked files:
- ...

Missing optional files:
- ...

Summary:
- ...

Language check:
- PASS|FAIL
- Details: ...

Ownership check:
- PASS|FAIL
- Details: ...

External decision reference check:
- PASS|FAIL
- Details: ...

Confirmation invariant check:
- PASS|FAIL
- Details: ...

Policy invention check:
- PASS|FAIL
- Details: ...

Open questions check:
- PASS|FAIL
- Details: ...

Coordination vs ownership check:
- PASS|FAIL
- Details: ...

Violations:
- Severity:
  Rule:
  Location:
  Evidence:
  Why it matters:
  Suggested correction:

Open questions preserved:
- ...

Recommended next step:
- Continue to /speckit.clarify
- Continue to /speckit.plan
- Fix the specification before planning
- Ask a domain expert
```

If there are no violations, write:

```text
Violations:
- None
```

## Expected failure example

If the specification assigns payment authorization or physical stock mutation to the Sales Order aggregate, report:

```text
CRITICAL — Bounded-context ownership violation

The specification assigns payment authorization and physical stock mutation to the Sales Order aggregate.

Conflicts:

BC-003
Payment owns authorization outcomes.

BC-007
Warehouse owns physical stock and stock reservations.

BC-009
Sales may store external decision references but must not embed Payment or Warehouse domain models.
```

## Correction guidance

When suggesting corrections, prefer these patterns:

```markdown
Sales stores `PaymentAuthorizationId` as evidence that Payment authorized the payment.
```

not:

```markdown
Sales authorizes the payment.
```

Use:

```markdown
Sales stores `StockReservationId` as evidence that Warehouse reserved the stock.
```

not:

```markdown
Sales reserves stock.
```

Use:

```markdown
Payment-provider timeout handling is [NEEDS CLARIFICATION].
```

not:

```markdown
A timeout means payment is declined.
```

Use:

```markdown
Out of scope: payment authorization, stock reservation, stock release, refund, shipment, invoicing.
```

not:

```markdown
Sales coordinates the full order-to-cash process.
```

Use:

```markdown
A Sales Order may become `Confirmed` only when required external decision references are present.
```

not:

```markdown
A Sales Order becomes `Confirmed` after calling Payment.Authorize and WarehouseReservation.Reserve.
```

Use:

```markdown
A coordination component requests payment authorization and stock reservation.
```

not:

```markdown
SalesOrder.Confirm() authorizes payment and reserves stock.
```

## Constraints

Do not create files.

Do not modify files.

Do not generate code.

Do not generate tasks.

Do not resolve open questions.

Do not invent business policy.

Do not broaden the Sales bounded context.

Do not make the specification more complete by hiding uncertainty.

Do not turn external decision references into remote aggregate access.

Your job is to protect domain meaning.
