---

description: Validate generated tasks against BrewUp Sales Order ownership and unresolved policy rules.
handoffs:
  - label: Analyze Consistency
    agent: speckit.analyze
    prompt: Analyze the specification, plan, and tasks for consistency with BrewUp bounded-context rules and unresolved domain decisions.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Task Guard

You are reviewing the generated task list for the BrewUp Sales Order confirmation feature.

This command runs after `/speckit.tasks`.

Its purpose is not to implement the tasks.

Its purpose is to ensure that generated tasks do not turn external decisions, unresolved business policies, or out-of-scope responsibilities into implementation work inside Sales.

## Goal

Check whether the generated task list respects the Sales, Payment, and Warehouse ownership rules recorded in the specification and plan.

The most important risk is that the task list looks practical but reintroduces the original wrong design:

```text
Sales Order task list:
- Add Payment entity to SalesOrder
- Add WarehouseReservation entity to SalesOrder
- Implement Confirm() to authorize payment
- Implement Confirm() to reserve stock
- Mark SalesOrder as Confirmed
```

This is not just a tasking error.

It is architectural drift turned into implementation work.

## Files to read

Read the following files if they exist:

* `.specify/memory/constitution.md`
* `specs/001-sales-order-confirmation/domain-carrier.md`
* `specs/001-sales-order-confirmation/spec.md`
* `specs/001-sales-order-confirmation/clarify-log.md`
* `specs/001-sales-order-confirmation/plan.md`
* `specs/001-sales-order-confirmation/tasks.md`

If the active feature directory is different, resolve it from `.specify/feature.json` if available.

If `.specify/feature.json` does not exist, scan `specs/*/tasks.md` and use the task list that most clearly refers to Sales Order confirmation.

If no task file exists, stop and report:

```text
BrewUp Task Guard: BLOCKED
Reason: no task file found.
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

## Bounded-context rules to enforce

### BC-001 — Sales owns Sales Order lifecycle

Tasks may implement Sales Order lifecycle behavior inside Sales.

Tasks may implement commercial status transitions.

Tasks must not move Payment or Warehouse authority into Sales.

### BC-002 — Sales Order is the aggregate owned by Sales

Tasks may create or modify the Sales Order aggregate.

Tasks must not add Payment or Warehouse-owned domain models inside Sales Order.

### BC-003 — Payment owns authorization outcomes

Tasks must not implement payment authorization inside Sales.

Tasks must not implement payment-provider timeout interpretation inside Sales unless the specification records an explicit Payment-domain decision.

Tasks must not implement void or refund behavior inside Sales unless explicitly assigned by the specification.

### BC-004 — Payment Authorization is an external decision

Tasks may store `PaymentAuthorizationId`.

Tasks may implement handlers that receive or observe payment authorization outcomes.

Tasks may implement integration calls that request Payment Authorization through a boundary.

Tasks must not produce Payment Authorization outcomes inside Sales.

### BC-005 — Warehouse owns physical stock

Tasks must not implement authoritative stock availability inside Sales.

Tasks must not implement physical stock state inside Sales.

### BC-006 — Stock Reservation is an external decision

Tasks may store `StockReservationId`.

Tasks may implement handlers that receive or observe Warehouse reservation outcomes.

Tasks may implement integration calls that request Stock Reservation through a boundary.

Tasks must not produce Stock Reservation outcomes inside Sales.

### BC-007 — Warehouse owns stock mutation

Tasks must not decrement stock, reserve stock, release stock, or decide stock availability inside Sales.

Availability is not a durable fact.

Only a Stock Reservation is durable enough to support Sales Order confirmation.

### BC-008 — Reacting is not owning

Tasks may implement reactions to external outcomes.

Tasks must not implement the process that produces those outcomes inside Sales.

Dependency on a decision is not ownership of the decision.

### BC-009 — External decision references, not embedded models

Tasks may introduce external decision references such as:

* `PaymentAuthorizationId`
* `StockReservationId`
* `PaymentTermsApprovalId`, only if the evolved specification allows it.

Tasks must not introduce embedded domain models such as:

* `Payment`
* `PaymentAuthorization`
* `WarehouseReservation`
* `StockReservation`
* `WarehouseStock`

inside the Sales Order aggregate as owned entities, value objects, or local domain models.

An external decision reference is not a back door through which Sales loads or manipulates another bounded context's aggregate.

### BC-010 — Confirmed requires evidence

Tasks must preserve the invariant that a Sales Order cannot become `Confirmed` without required external decision references.

For the base scenario:

* `PaymentAuthorizationId` is required.
* `StockReservationId` is required.

During the lifecycle, these references may be empty.

At the moment a Sales Order becomes `Confirmed`, missing required evidence is an invalid state.

### BC-011 — Clarification preserves authority

Tasks must not implement behavior that the specification or clarify log kept unresolved.

Tasks depending on unresolved decisions must be marked as `BLOCKED`.

## Valid task patterns

These task patterns are valid.

```markdown
- [ ] Implement SalesOrder aggregate with commercial lifecycle state.
```

```markdown
- [ ] Add `PaymentAuthorizationId` to SalesOrder as an external decision reference.
```

```markdown
- [ ] Add `StockReservationId` to SalesOrder as an external decision reference.
```

```markdown
- [ ] Add guard preventing Confirmed when `PaymentAuthorizationId` is missing.
```

```markdown
- [ ] Add guard preventing Confirmed when `StockReservationId` is missing.
```

```markdown
- [ ] Handle `PaymentAuthorized` as an external input.
```

```markdown
- [ ] Handle `StockReserved` as an external input.
```

```markdown
- [ ] Handle `StockReservationFailed` as an external outcome without deciding cancellation, refund, void, or retry behavior.
```

```markdown
- [ ] Request payment authorization through the Payment boundary.
```

```markdown
- [ ] Request stock reservation through the Warehouse boundary.
```

```markdown
- [ ] Add tests proving SalesOrder cannot become Confirmed without required external decision references.
```

```markdown
- [ ] Add tests proving SalesOrder does not embed Payment or WarehouseReservation.
```

## Invalid task patterns

Fail the review if `tasks.md` contains tasks like these, unless explicitly allowed by the specification.

```markdown
- [ ] Add Payment entity inside SalesOrder.
```

```markdown
- [ ] Add PaymentAuthorization entity inside SalesOrder.
```

```markdown
- [ ] Add WarehouseReservation entity inside SalesOrder.
```

```markdown
- [ ] Add StockReservation entity inside SalesOrder.
```

```markdown
- [ ] Implement payment authorization in Sales.
```

```markdown
- [ ] Authorize payment from SalesOrder.Confirm().
```

```markdown
- [ ] Interpret payment-provider timeout as declined.
```

```markdown
- [ ] Interpret payment-provider timeout as authorized.
```

```markdown
- [ ] Void payment when stock reservation fails.
```

```markdown
- [ ] Issue refund when stock reservation fails.
```

```markdown
- [ ] Reserve stock from SalesOrder.Confirm().
```

```markdown
- [ ] Decrement Warehouse stock when confirming SalesOrder.
```

```markdown
- [ ] Release stock after a default timeout.
```

```markdown
- [ ] Automatically cancel SalesOrder after stock failure.
```

```markdown
- [ ] Automatically retry stock reservation after failure.
```

```markdown
- [ ] Generate invoice after confirmation.
```

```markdown
- [ ] Create shipment after confirmation.
```

These tasks may look useful.

They are invalid unless the specification explicitly records the required domain decision and assigns the responsibility to Sales.

## Blocked task patterns

If a task depends on an unresolved domain decision, it must be marked as `BLOCKED`.

Example:

```markdown
- [ ] BLOCKED Decide reaction to `StockReservationFailed`.
      Requires explicit domain decision.
      Do NOT implement cancellation, refund, void, retry, or notification by default.
```

Example:

```markdown
- [ ] BLOCKED Define payment-provider timeout interpretation.
      Requires Payment domain decision.
      Do NOT treat timeout as declined, authorized, or pending by default.
```

Example:

```markdown
- [ ] BLOCKED Define stock reservation expiration policy.
      Requires Warehouse domain decision.
      Do NOT release stock after a default timeout.
```

Example:

```markdown
- [ ] BLOCKED Define customer notification after partial failure.
      Requires explicit domain decision.
      Do NOT notify customers automatically by default.
```

Example:

```markdown
- [ ] BLOCKED Define pre-approved payment terms evidence.
      Requires domain decision.
      Do NOT confirm SalesOrder using `PaymentTermsApprovalId` unless the evolved specification allows it.
```

## Evolved-specification checks

If the specification has evolved to support pre-approved payment terms for wholesale customers, check that tasks reflect the evolved decision model.

Allowed only if explicitly present in the specification:

```markdown
- [ ] Add `PaymentTermsApprovalId` as an alternative external decision reference.
```

```markdown
- [ ] Allow confirmation when `StockReservationId` is present and either `PaymentAuthorizationId` or `PaymentTermsApprovalId` is present.
```

Invalid unless explicitly present in the specification:

```markdown
- [ ] Make `PaymentAuthorizationId` optional for every SalesOrder.
```

```markdown
- [ ] Confirm wholesale orders without any external payment decision reference.
```

```markdown
- [ ] Copy the payment terms approval model into SalesOrder.
```

The commercial prerequisite may change.

The need for external evidence does not disappear.

## Traceability rules

For each task, check whether it traces to at least one of:

* a functional requirement;
* a bounded-context rule;
* a clarification result;
* an explicit open question;
* an out-of-scope marker;
* a plan section that is itself compliant with the specification.

Tasks without traceability must be reported.

A task must not be considered valid only because it appears useful.

Usefulness is not authority.

## Severity levels

Use these severity levels.

### CRITICAL

Use `CRITICAL` when a task violates ownership or decision authority.

Examples:

* Add Payment entity inside SalesOrder.
* Add WarehouseReservation entity inside SalesOrder.
* Authorize payment in Sales.
* Reserve or decrement stock in Sales.
* Confirm SalesOrder without required external evidence.
* Implement unresolved policy as if approved.

### HIGH

Use `HIGH` when a task misses a required invariant or weakens a specification rule.

Examples:

* No task protects the Confirmed evidence invariant.
* No task stores required external decision references.
* No task tests the external-reference invariant.
* No task marks unresolved policy as blocked.

### MEDIUM

Use `MEDIUM` when terminology is imprecise but ownership is still correct.

Examples:

* Uses `Order` instead of `SalesOrder`, but context and ownership remain clear.
* Says “stock available” informally while still requiring `StockReservationId`.

### LOW

Use `LOW` for minor wording or documentation improvements.

## Output format

Return the result using this exact structure:

```text
BrewUp Task Guard: PASS|FAIL|BLOCKED

Checked files:
- ...

Missing optional files:
- ...

Summary:
- ...

Invalid tasks:
- Severity:
  Task:
  Reason:
  Violated rule:
  Suggested correction:

Blocked tasks:
- ...

Missing blocked tasks:
- ...

Traceability:
- Requirements covered:
- Requirements missing tasks:
- Tasks without requirement, BC rule, clarification, or compliant plan section:
- BC rules covered:
- BC rules missing task coverage:

Ownership check:
- PASS|FAIL
- Details: ...

External decision reference check:
- PASS|FAIL
- Details: ...

Confirmation invariant check:
- PASS|FAIL
- Details: ...

Open question preservation:
- PASS|FAIL
- Details: ...

Evolved specification check:
- PASS|FAIL|NOT APPLICABLE
- Details: ...

Decision:
- Safe to continue to implementation: YES|NO
```

If there are no invalid tasks, write:

```text
Invalid tasks:
- None
```

If there are no missing blocked tasks, write:

```text
Missing blocked tasks:
- None
```

If implementation is safe, write:

```text
Decision:
- Safe to continue to implementation: YES
```

If implementation is not safe, write:

```text
Decision:
- Safe to continue to implementation: NO
- Required action: fix generated tasks or ask a domain expert.
```

## Expected FAIL example

If the task list reintroduces the original wrong aggregate design, report:

```text
BrewUp Task Guard: FAIL

Summary:
- The task list turns Payment and Warehouse decisions into Sales implementation work.

Invalid tasks:
- Severity: CRITICAL
  Task: Add Payment entity inside SalesOrder.
  Reason: Payment is owned by the Payment bounded context.
  Violated rule: BC-003, BC-009
  Suggested correction: Store `PaymentAuthorizationId` as an external decision reference.

- Severity: CRITICAL
  Task: Reserve stock from SalesOrder.Confirm().
  Reason: Stock reservation is owned by Warehouse.
  Violated rule: BC-006, BC-007
  Suggested correction: Request stock reservation through the Warehouse boundary and store `StockReservationId`.

Decision:
- Safe to continue to implementation: NO
- Required action: fix generated tasks or ask a domain expert.
```

## Expected PASS example

A valid task list may include:

```text
BrewUp Task Guard: PASS

Summary:
- Tasks preserve Sales, Payment, and Warehouse ownership.
- Tasks use external decision references instead of embedded models.
- Tasks preserve the Confirmed evidence invariant.
- Unresolved policies remain blocked.

Invalid tasks:
- None

Blocked tasks:
- Decide reaction to StockReservationFailed.
- Define payment-provider timeout interpretation.
- Define stock reservation expiration policy.

Decision:
- Safe to continue to implementation: YES
```

## Correction guidance

Prefer:

```markdown
- [ ] Add `PaymentAuthorizationId` to SalesOrder as an external decision reference.
```

not:

```markdown
- [ ] Add `Payment` entity to SalesOrder.
```

Prefer:

```markdown
- [ ] Add `StockReservationId` to SalesOrder as an external decision reference.
```

not:

```markdown
- [ ] Add `WarehouseReservation` entity to SalesOrder.
```

Prefer:

```markdown
- [ ] Add guard preventing Confirmed without required external decision references.
```

not:

```markdown
- [ ] In Confirm(), call Payment.Authorize and WarehouseReservation.Reserve.
```

Prefer:

```markdown
- [ ] Request payment authorization through the Payment boundary.
```

not:

```markdown
- [ ] Implement payment authorization inside Sales.
```

Prefer:

```markdown
- [ ] Request stock reservation through the Warehouse boundary.
```

not:

```markdown
- [ ] Reserve stock inside Sales.
```

Prefer:

```markdown
- [ ] BLOCKED Define reaction to StockReservationFailed.
```

not:

```markdown
- [ ] Cancel and refund when stock reservation fails.
```

Prefer:

```markdown
- [ ] BLOCKED Define payment-provider timeout interpretation.
```

not:

```markdown
- [ ] Treat payment timeout as declined.
```

Prefer:

```markdown
- [ ] Add `PaymentTermsApprovalId` only if the evolved specification explicitly allows pre-approved payment terms.
```

not:

```markdown
- [ ] Make payment authorization optional for wholesale customers.
```

## Constraints

Do not create files.

Do not modify files.

Do not generate code.

Do not implement tasks.

Do not resolve open questions.

Do not invent business policy.

Do not move Payment or Warehouse behavior into Sales.

Do not turn external decision references into remote aggregate access.

Do not make implementation appear safe by hiding uncertainty.

Your job is to protect the implementation backlog from architectural drift.
