# BrewUp Sales Order Confirmation Governance

## Authority

- Sales owns the Sales Order lifecycle.
- Payment owns payment authorization outcomes.
- Warehouse owns stock availability and reservation outcomes.

Sales may depend on decisions produced by Payment and Warehouse, but must not own the models or processes that produce those decisions.

## Numbered rules

### BC-000 — Domain rules are authoritative

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

### BC-001 — Sales owns Sales Order lifecycle

Sales owns:

* Sales Order lifecycle;
* commercial status;
* customer demand;
* the transition of a Sales Order to `Confirmed`, once required external evidence is present.

Sales does not own the decisions that produce the external evidence.

---

### BC-002 — Sales Order is the aggregate owned by Sales

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

### BC-003 — Payment owns authorization outcomes

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

### BC-004 — Payment Authorization is an external decision

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

### BC-005 — Warehouse owns physical stock

Warehouse owns:

* physical stock;
* stock availability;
* stock reservation;
* stock release;
* reservation expiration.

Sales must not act as the authority of truth for physical stock.

Sales must not decide whether stock is physically available.

---

### BC-006 — Stock Reservation is an external decision

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

### BC-007 — Warehouse owns stock mutation

Sales must not:

* decrement stock;
* reserve stock;
* release stock;
* mutate Warehouse stock;
* decide stock availability as the authority of truth.

Availability is not a durable fact.

Only a Stock Reservation is durable enough to support Sales Order confirmation.

---

### BC-008 — Reacting is not owning

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

### BC-009 — External decision references, not embedded models

Sales may store external decision references such as:

* `PaymentAuthorizationId`;
* `StockReservationId`;
* `PaymentTermsApprovalId`, only if the specification evolves to support pre-approved payment terms.

Sales must not embed Payment or Warehouse domain models.

External decision references are evidence that another bounded context made a decision.

They are not a way to load, modify, or own another bounded context's aggregate.

---

### BC-010 — Confirmed requires evidence

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

### BC-011 — Clarification preserves authority

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

## Non-negotiable boundaries

Sales must not authorize payments; interpret payment-provider timeouts as declined, authorized, or pending without a decision; void authorizations; issue refunds; own physical-stock truth; reserve, release, or decrement stock; create shipments; issue invoices; or invent retry, cancellation, notification, or reservation-expiration policy.

Do not assume stock failure cancels an order, refunds or voids payment, triggers retry, that authorization or submission confirms an order, that availability equals reservation, that shipment starts after confirmation, that invoicing belongs to Sales, that provider timeout has a defined outcome, that stock release has a default timeout, or that payment terms apply without an explicit rule. Model any needed behavior as external responsibility, out of scope, or an explicit open question.
