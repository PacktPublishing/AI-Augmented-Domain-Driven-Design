# BrewUp Sales Order Confirmation — Domain Carrier

## Purpose

This file contains the domain rules that must be carried into the feature specification for BrewUp Sales Order Confirmation.

It exists before `/speckit.specify` creates the feature folder.

Its role is to provide durable domain context for the first specification pass.

This file is not an implementation plan.

It defines:

- domain language;
- bounded-context ownership;
- external decision references;
- confirmation invariants;
- forbidden responsibilities;
- open questions;
- rules that generated artifacts must respect.

---

# Domain Authorities

BrewUp has three separate domain authorities for this feature:

```text
Sales        — commercial commitment
Payment      — payment authorization
Warehouse    — physical stock
````

Sales owns the Sales Order lifecycle.

Payment owns payment authorization outcomes.

Warehouse owns physical stock and stock reservation outcomes.

Sales may depend on decisions produced by Payment and Warehouse.

Sales must not own the models or processes that produce those decisions.

---

# Ubiquitous Language

## Sales Order

A commercial commitment made by a customer.

Sales owns the Sales Order lifecycle, commercial status, and customer demand.

Use `Sales Order`, not generic `Order`, when referring to the Sales-owned aggregate.

## Payment Authorization

An outcome produced by Payment indicating that a specified amount has been authorized.

Sales may depend on this outcome.

Sales does not produce this outcome.

## Stock Reservation

An outcome produced by Warehouse indicating that specified physical stock has been reserved.

Sales may depend on this outcome.

Sales does not produce this outcome.

Availability is not a durable fact.

Only a Stock Reservation is durable enough to support Sales Order confirmation.

## External Decision Reference

A reference stored by Sales as evidence that an external authority made a decision Sales depends on.

Examples:

* `PaymentAuthorizationId`
* `StockReservationId`
* `PaymentTermsApprovalId`

An external decision reference is not a back door through which Sales loads or manipulates another bounded context's aggregate.

---

# Bounded Context Rules

## BC-000 — Domain rules are authoritative

The rules in this file are authoritative for the Sales Order Confirmation feature.

They must be preserved in the generated `spec.md`.

They must constrain all subsequent artifacts:

* clarification questions;
* implementation plan;
* generated tasks;
* consistency analysis;
* source code;
* tests.

A generated artifact that violates these rules is not merely incomplete.

It is architecturally misaligned.

The agent must not override these rules to make an output look more complete, more convenient, or more technically coherent.

Unknown business decisions must remain explicit as open questions or `[NEEDS CLARIFICATION]`.

---

## BC-001 — Sales owns Sales Order lifecycle

Sales owns:

* Sales Order lifecycle;
* commercial status;
* customer demand;
* the transition of a Sales Order to `Confirmed`, once required external evidence is present.

Sales does not own the decisions that produce the external evidence.

---

## BC-002 — Sales Order is the aggregate owned by Sales

Sales owns the Sales Order aggregate.

Sales must not embed Payment or Warehouse domain models inside the Sales Order aggregate.

Invalid examples:

```text
SalesOrder contains Payment.
SalesOrder contains WarehouseReservation.
SalesOrder owns StockReservation.
SalesOrder owns PaymentAuthorization.
```

The problem is not the number of classes.

The problem is decision authority.

---

## BC-003 — Payment owns authorization outcomes

Payment owns:

* payment authorization;
* authorization outcome;
* payment-provider timeout interpretation;
* void behavior;
* refund behavior.

Sales must not authorize payment.

Sales must not interpret payment-provider timeouts unless the specification records an explicit Payment-domain decision.

Sales must not issue refunds or void payment authorizations unless explicitly assigned by a domain decision.

---

## BC-004 — Payment Authorization is an external decision

Sales may store `PaymentAuthorizationId` as evidence that Payment produced an authorization outcome.

Sales may react to a payment authorization outcome.

Sales may request payment authorization through an integration boundary.

Sales must not produce the payment authorization outcome.

Valid model:

```text
SalesOrder
  PaymentAuthorizationId?
```

Invalid model:

```text
SalesOrder
  Payment
    Authorize()
```

---

## BC-005 — Warehouse owns physical stock

Warehouse owns:

* physical stock;
* stock availability;
* stock reservation;
* stock release;
* reservation expiration.

Sales must not act as the authority of truth for physical stock.

Sales must not decide whether stock is physically available.

---

## BC-006 — Stock Reservation is an external decision

Sales may store `StockReservationId` as evidence that Warehouse produced a reservation outcome.

Sales may react to a stock reservation outcome.

Sales may request stock reservation through an integration boundary.

Sales must not produce the stock reservation outcome.

Valid model:

```text
SalesOrder
  StockReservationId?
```

Invalid model:

```text
SalesOrder
  WarehouseReservation
    Reserve()
```

---

## BC-007 — Warehouse owns stock mutation

Sales must not:

* decrement stock;
* reserve stock;
* release stock;
* mutate Warehouse stock;
* decide stock availability as the authority of truth.

Availability is not a durable fact.

Only a Stock Reservation is durable enough to support Sales Order confirmation.

---

## BC-008 — Reacting is not owning

Sales may react to Payment and Warehouse outcomes.

Sales must not produce those outcomes.

Dependency on a decision is not ownership of the decision.

Valid statement:

```text
Sales reacts to PaymentAuthorized.
```

Invalid statement:

```text
Sales authorizes the payment.
```

Valid statement:

```text
Sales reacts to StockReserved.
```

Invalid statement:

```text
Sales reserves stock.
```

---

## BC-009 — External decision references, not embedded models

Sales may store external decision references such as:

* `PaymentAuthorizationId`;
* `StockReservationId`;
* `PaymentTermsApprovalId`, only if the specification evolves to support pre-approved payment terms.

Sales must not embed Payment or Warehouse domain models.

External decision references are evidence that another bounded context made a decision.

They are not a way to load, modify, or own another bounded context's aggregate.

---

## BC-010 — Confirmed requires evidence

A Sales Order must not become `Confirmed` unless the required external decision references are present.

For the base scenario, confirmation requires:

* `PaymentAuthorizationId`;
* `StockReservationId`.

During the lifecycle, these references may be empty.

At the moment a Sales Order becomes `Confirmed`, missing required evidence is an invalid state.

Invalid state:

```text
Status = Confirmed
PaymentAuthorizationId = null
StockReservationId = present
```

Invalid state:

```text
Status = Confirmed
PaymentAuthorizationId = present
StockReservationId = null
```

Valid state:

```text
Status = Confirmed
PaymentAuthorizationId = present
StockReservationId = present
```

---

## BC-011 — Clarification preserves authority

When business behavior is unclear, the specification must preserve the ambiguity as an open question or `[NEEDS CLARIFICATION]`.

The agent must not silently decide.

Examples of unresolved decisions:

* What happens if payment is authorized but stock cannot be reserved?
* How long can a stock reservation remain active while payment is pending?
* What does a payment-provider timeout mean: declined, pending, or unknown?
* May a Sales Order be partially reserved?
* Which outcome must Sales receive before the Sales Order can become `Confirmed`?
* Who owns customer notification after partial failure?
* Who coordinates the process: saga, process manager, application service, or agent?
* Who approves pre-approved payment terms for wholesale customers?
* How long is a payment terms approval valid?
* Can a payment terms approval be revoked?
* Does payment terms approval apply to the customer or to a specific Sales Order?
* What evidence does Sales need when confirmation is based on pre-approved payment terms?

The agent may identify ambiguity.

The domain expert must resolve it.

The specification must preserve the resolution.

---

# Forbidden Responsibilities

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

---

# Policy Invention Rules

The agent must not invent business policy.

In particular, the agent must not assume that:

* stock failure automatically cancels the Sales Order;
* stock failure automatically triggers a refund;
* stock failure automatically voids the payment authorization;
* stock failure automatically triggers a retry;
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

---

# Rules for `/speckit.specify`

The generated specification must:

1. Preserve BC-000 through BC-011.
2. Use `Sales Order`, not generic `Order`, unless `Order` is explicitly defined as Sales Order within this feature.
3. Model Sales, Payment, and Warehouse as separate authorities.
4. Model Payment Authorization as an external decision produced by Payment.
5. Model Stock Reservation as an external decision produced by Warehouse.
6. Model `PaymentAuthorizationId` and `StockReservationId` as external decision references stored by Sales.
7. Prevent Sales from embedding Payment or Warehouse domain models.
8. Prevent Sales from authorizing payments.
9. Prevent Sales from reserving, releasing, or decrementing stock.
10. Prevent Sales from owning refunds, voids, shipment, or invoicing.
11. Preserve unresolved business decisions as open questions or `[NEEDS CLARIFICATION]`.
12. Describe coordination behavior before choosing a coordination mechanism.
13. Avoid inventing domain policy to make the model look complete.

---

# Rules for `/speckit.plan`

The generated plan must:

1. Respect BC-000 through BC-011.
2. Choose technical coordination mechanisms without changing decision ownership.
3. Use external decision references instead of embedded Payment or Warehouse models.
4. Preserve the confirmation invariant.
5. Keep unresolved policies as open questions or blocked work.
6. Avoid turning missing domain decisions into implementation choices.

Allowed coordination mechanisms include:

* application service;
* saga;
* process manager;
* message handler;
* workflow;
* agent-assisted coordinator.

A coordination mechanism may coordinate.

It must not steal authority.

---

# Rules for `/speckit.tasks`

Generated tasks must:

1. Implement only responsibilities assigned to Sales.
2. Store external decision references.
3. Preserve the confirmation invariant.
4. Avoid embedding Payment or Warehouse models.
5. Avoid implementing unresolved policy.
6. Mark tasks depending on unresolved decisions as `BLOCKED`.

Valid task examples:

```markdown
- [ ] Add `PaymentAuthorizationId` to SalesOrder as an external decision reference.
- [ ] Add `StockReservationId` to SalesOrder as an external decision reference.
- [ ] Add guard preventing Confirmed without required external decision references.
- [ ] Handle `PaymentAuthorized` as an external input.
- [ ] Handle `StockReserved` as an external input.
```

Invalid task examples:

```markdown
- [ ] Add Payment entity inside SalesOrder.
- [ ] Add WarehouseReservation entity inside SalesOrder.
- [ ] Authorize payment from SalesOrder.Confirm().
- [ ] Reserve stock from SalesOrder.Confirm().
- [ ] Cancel and refund when stock reservation fails.
```

---

# Final Principle

SDD does not guarantee that the model will always obey.

SDD makes violations inspectable.

If an artifact violates these rules, the issue should be reported as a named rule violation, for example:

```text
The plan violates BC-003, BC-007, and BC-009.
```

Not:

```text
This architecture feels wrong.
```
