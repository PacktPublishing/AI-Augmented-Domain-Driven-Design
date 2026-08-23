---
description: Detect architectural drift in the generated BrewUp Sales Order implementation plan.
handoffs:

  - label: Generate Tasks
    agent: speckit.tasks
    prompt: Generate implementation tasks only after the plan has been checked for bounded-context ownership drift.
  - label: Analyze Consistency
    agent: speckit.analyze
    prompt: Analyze the specification, plan, and tasks for consistency with BrewUp bounded-context rules.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Plan Guard

You are reviewing a generated implementation plan for the BrewUp Sales Order confirmation feature.

This command runs after `/speckit.plan`.

Its purpose is not to improve the plan creatively.

Its purpose is to detect whether the plan drifted away from the specification by reintroducing wrong ownership, embedded external models, or behavior that belongs to Payment or Warehouse.

## Goal

Check whether the implementation plan respects the bounded-context rules and external decision references recorded in the specification.

The most important risk is that the plan looks technically reasonable but reintroduces the original wrong design:

```text
Sales Order aggregate
 ├── Payment entity
 └── WarehouseReservation entity

Confirm():
  Authorize payment
  Decrement or reserve stock
  Mark as Confirmed
```

This is architectural drift.

The plan may choose a coordination mechanism.

The plan must not move decision authority into the wrong bounded context.

## Files to read

Read the following files if they exist:

* `.specify/memory/constitution.md`
* `specs/001-sales-order-confirmation/domain-carrier.md`
* `specs/001-sales-order-confirmation/spec.md`
* `specs/001-sales-order-confirmation/clarify-log.md`
* `specs/001-sales-order-confirmation/plan.md`

If the active feature directory is different, resolve it from `.specify/feature.json` if available.

If `.specify/feature.json` does not exist, scan `specs/*/plan.md` and use the plan that most clearly refers to Sales Order confirmation.

If no plan file exists, stop and report:

```text
BrewUp Plan Guard: BLOCKED
Reason: no implementation plan found.
```

Do not fail because optional context files are missing.

Report missing files and continue with the available information.

## Bounded-context rules to enforce

### BC-001 — Sales owns Sales Order lifecycle

The plan may implement Sales Order lifecycle behavior inside Sales.

Sales owns commercial status and customer demand.

### BC-002 — Sales Order is the aggregate owned by Sales

The plan may create or modify the Sales Order aggregate.

The plan must not place Payment or Warehouse-owned domain models inside the Sales Order aggregate.

### BC-003 — Payment owns authorization outcomes

The plan must not make Sales authorize payment.

The plan must not add Payment authorization behavior inside Sales Order.

The plan must not make Sales interpret payment-provider timeouts unless the specification records an explicit Payment-domain decision.

### BC-004 — Payment Authorization is an external decision

The plan may store `PaymentAuthorizationId`.

The plan may react to Payment authorization outcomes.

The plan may request payment authorization through an integration boundary.

The plan must not produce Payment authorization outcomes.

### BC-005 — Warehouse owns physical stock

The plan must not make Sales own physical stock.

The plan must not make Sales the authority for stock availability.

### BC-006 — Stock Reservation is an external decision

The plan may store `StockReservationId`.

The plan may react to Warehouse reservation outcomes.

The plan may request stock reservation through an integration boundary.

The plan must not produce Stock Reservation outcomes.

### BC-007 — Warehouse owns stock mutation

The plan must not make Sales decrement stock, reserve stock, release stock, or decide stock availability as the authority of truth.

Availability is not a durable fact.

Only a Stock Reservation is durable enough to support Sales Order confirmation.

### BC-008 — Reacting is not owning

The plan may describe reactions to external outcomes.

The plan must not move the process that produces those outcomes into Sales.

Dependency on a decision is not ownership of the decision.

### BC-009 — External decision references, not embedded models

The plan may use:

* `PaymentAuthorizationId`
* `StockReservationId`
* `PaymentTermsApprovalId`, only if the evolved specification allows it.

The plan must not embed:

* `Payment`
* `PaymentAuthorization`
* `WarehouseReservation`
* `StockReservation`
* `WarehouseStock`

inside the Sales Order aggregate as owned entities, value objects, or local domain models.

### BC-010 — Confirmed requires evidence

The plan must preserve the invariant that a Sales Order cannot become `Confirmed` without required external decision references.

For the base scenario:

* `PaymentAuthorizationId` is required.
* `StockReservationId` is required.

During the lifecycle, these references may be empty.

At the moment a Sales Order becomes `Confirmed`, missing required evidence is an invalid state.

### BC-011 — Clarification preserves authority

The plan must not resolve open questions unless the specification records a domain decision.

The plan must not convert `[NEEDS CLARIFICATION]` into implementation behavior.

## Allowed plan patterns

These patterns are allowed.

### External decision references

```markdown
SalesOrder stores:
- PaymentAuthorizationId
- StockReservationId
- Status
```

### External inputs

```markdown
Sales reacts to:
- PaymentAuthorized
- StockReserved
- StockReservationFailed
```

### Coordination

```markdown
A coordination component requests payment authorization from Payment.
A coordination component requests stock reservation from Warehouse.
```

The coordination component may be described as:

* application service;
* saga;
* process manager;
* message handler;
* workflow;
* agent-assisted coordinator.

The mechanism is allowed only if decision ownership remains unchanged.

### Confirmation guard

```markdown
SalesOrder can transition to Confirmed only when:
- PaymentAuthorizationId is present;
- StockReservationId is present.
```

## Drift patterns to detect

Report `CRITICAL` if the plan contains any of these patterns.

### Embedded external model drift

```markdown
The Order aggregate contains:
- Payment entity
- WarehouseReservation entity
```

```markdown
SalesOrder contains Payment.
SalesOrder contains PaymentAuthorization.
SalesOrder contains WarehouseReservation.
SalesOrder contains StockReservation.
SalesOrder contains WarehouseStock.
```

```markdown
Payment is modeled as a child entity of SalesOrder.
WarehouseReservation is modeled as a child entity of SalesOrder.
```

### Wrong behavior inside Sales Order

```markdown
Order.Confirm():
1. Authorize the payment.
2. Decrement Warehouse stock.
3. Mark the Order as Confirmed.
```

```csharp
Payment.Authorize(...);
WarehouseReservation.Reserve(...);
WarehouseStock.Decrement(...);
Stock.Reserve(...);
```

inside Sales or Sales Order.

```csharp
public void Confirm()
{
    Payment.Authorize(Total);

    foreach (var line in Lines)
        WarehouseReservation.Reserve(line.BeerId, line.Quantity);

    Status = OrderStatus.Confirmed;
}
```

### Wrong authority in plan

```markdown
Implement payment authorization in Sales.
Implement stock reservation in Sales.
Implement refund in Sales.
Implement payment void in Sales.
Implement stock release in Sales.
Implement invoice generation in Sales.
Implement shipment creation in Sales.
```

### Invalid confirmation logic

```markdown
Mark Sales Order as Confirmed after checking stock availability.
```

without a durable `StockReservationId`.

```markdown
Mark Sales Order as Confirmed after payment request is sent.
```

without a `PaymentAuthorizationId`.

```markdown
Mark Sales Order as Confirmed after receiving a payment timeout.
```

without an explicit Payment-domain decision.

### Silent decision resolution

```markdown
If payment provider times out, mark payment as declined.
If payment provider times out, mark payment as authorized.
If stock reservation fails, cancel the Sales Order.
If stock reservation fails, refund the customer.
If stock reservation fails, void the payment.
If stock reservation fails, retry automatically.
Release stock after a default timeout.
Create a shipment after confirmation.
Issue an invoice after confirmation.
```

unless explicitly recorded as a domain decision in the specification.

### Coordination becoming ownership

Fail if the plan chooses a coordination mechanism and then uses that mechanism to move ownership into Sales.

For example:

```markdown
Use a saga inside Sales to authorize payment and reserve stock directly.
```

is invalid.

Prefer:

```markdown
Use a saga to request payment authorization from Payment and stock reservation from Warehouse.
```

The saga may coordinate.

The saga must not steal authority.

## Severity levels

Use these severity levels.

### CRITICAL

Use `CRITICAL` when the plan violates ownership or decision authority.

Examples:

* Sales authorizes payment.
* Sales reserves or decrements stock.
* Payment or Warehouse models are embedded in Sales Order.
* Confirmed can happen without required evidence.
* Open questions are silently resolved as implementation behavior.

### HIGH

Use `HIGH` when the plan misses a required behavior, invariant, or traceability link.

Examples:

* Plan does not enforce `Confirmed` evidence.
* Plan does not store required external decision references.
* Plan ignores clarify outcomes.
* Plan does not mention how external outcomes are observed.

### MEDIUM

Use `MEDIUM` when terminology is imprecise but ownership is still correct.

Examples:

* Uses `Order` where `Sales Order` should be used, but the context is still clear.
* Uses “stock available” informally while still requiring `StockReservationId`.

### LOW

Use `LOW` for minor wording or documentation improvements.

## Output format

Return the result using this exact structure:

```text
BrewUp Plan Guard: PASS|FAIL|BLOCKED

Checked files:
- ...

Missing optional files:
- ...

Summary:
- ...

Drift findings:
- Severity:
  Rule:
  Location:
  Evidence:
  Why it matters:
  Suggested correction:

Bounded-context alignment:
- BC-001: PASS|FAIL|NOT FOUND
- BC-002: PASS|FAIL|NOT FOUND
- BC-003: PASS|FAIL|NOT FOUND
- BC-004: PASS|FAIL|NOT FOUND
- BC-005: PASS|FAIL|NOT FOUND
- BC-006: PASS|FAIL|NOT FOUND
- BC-007: PASS|FAIL|NOT FOUND
- BC-008: PASS|FAIL|NOT FOUND
- BC-009: PASS|FAIL|NOT FOUND
- BC-010: PASS|FAIL|NOT FOUND
- BC-011: PASS|FAIL|NOT FOUND

Critical violations:
- ...

High severity findings:
- ...

Open questions preserved:
- ...

Recommended next step:
- Continue to /speckit.tasks
- Fix the plan before generating tasks
- Ask a domain expert
- Run /speckit.analyze for cross-artifact consistency
```

If there are no violations, write:

```text
Critical violations:
- None
```

If the plan passes, write:

```text
BrewUp Plan Guard: PASS

Recommended next step:
- Continue to /speckit.tasks
```

If the plan fails, write:

```text
BrewUp Plan Guard: FAIL

Recommended next step:
- Fix the plan before generating tasks
```

## Expected critical example

If the plan assigns payment authorization or physical stock mutation to the Sales Order aggregate, report:

```text
CRITICAL — Bounded-context ownership violation

The implementation plan assigns payment authorization and physical stock mutation to the Sales Order aggregate.

Conflicts:

BC-003
Payment owns authorization outcomes.

BC-007
Warehouse owns physical stock and stock reservations.

BC-009
Sales may store external decision references but must not embed Payment or Warehouse domain models.
```

## Correction guidance

Prefer this plan direction:

```markdown
SalesOrder stores:
- PaymentAuthorizationId
- StockReservationId
- Status
```

not:

```markdown
SalesOrder owns:
- Payment
- WarehouseReservation
```

Prefer:

```markdown
Sales reacts to:
- PaymentAuthorized
- StockReserved
- StockReservationFailed
```

not:

```markdown
Sales produces:
- PaymentAuthorized
- StockReserved
```

Prefer:

```markdown
A coordination component requests payment authorization and stock reservation.
```

not:

```markdown
SalesOrder.Confirm() authorizes payment and reserves stock.
```

Prefer:

```markdown
A coordination component may orchestrate the flow across Sales, Payment, and Warehouse.
```

not:

```markdown
The coordinator owns Payment and Warehouse decisions.
```

Prefer:

```markdown
The coordination mechanism is selected in the plan.
The ownership of decisions remains as defined in the specification.
```

not:

```markdown
The plan chooses a saga and therefore moves all behavior into Sales.
```

Prefer:

```markdown
Payment-provider timeout handling remains [NEEDS CLARIFICATION].
```

not:

```markdown
On timeout, mark payment as declined.
```

Prefer:

```markdown
Reaction to StockReservationFailed remains [NEEDS CLARIFICATION].
```

not:

```markdown
On stock reservation failure, cancel and refund.
```

## Constraints

Do not create files.

Do not modify files.

Do not generate code.

Do not generate tasks.

Do not fix the plan automatically.

Do not resolve open questions.

Do not invent business policy.

Do not move Payment or Warehouse behavior into Sales.

Do not turn external decision references into remote aggregate access.

Your job is to make architectural drift inspectable.
