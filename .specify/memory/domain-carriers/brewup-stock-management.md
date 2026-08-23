# BrewUp Stock Management — Domain Carrier

## Purpose

This file contains the domain rules that must be carried into any feature specification for BrewUp Stock Management.

It exists before `/speckit.specify` creates the feature folder.

Its role is to provide durable domain context for the first specification pass.

This file is not an implementation plan.

It defines:

- domain language;
- bounded-context ownership;
- owned concepts;
- external inputs;
- confirmed and proposed policies;
- open questions;
- forbidden assumptions;
- rules that generated artifacts must respect.

Stock Management manages product availability and stock reservation for BrewUp orders.

It does not own the customer order lifecycle.

It does not confirm orders.

It does not authorize payments.

It does not create shipments.

---

# Domain Authorities

Stock Management participates in a feature that spans several domain authorities:

```text
Sales        — commercial commitment and order lifecycle
Payment      — payment authorization
Stock        — product availability and stock reservation
Shipment     — physical delivery
```

Stock Management owns product availability and stock reservation outcomes.

Other contexts may depend on the outcomes Stock Management produces.

Stock Management must not own the models or processes that belong to Sales, Payment, or Shipment.

> Implementation note: in the BrewUp solution this bounded context is realised as
> the `Warehouse` module (`src/Warehouse/BrewUp.Warehouse.*`). The module is the
> physical unit; Stock Management is the bounded context it implements.

---

# Ubiquitous Language

## ProductStock

Represents the available stock for a product that can be reserved for customer orders.

## StockReservation

Represents a temporary reservation of stock for a specific order.

## StockReserved

Domain event raised when the requested stock has been successfully reserved.

## StockReservationFailed

Domain event raised when the requested stock cannot be reserved.

## StockReleased

Domain event raised when previously reserved stock is released.

## StockReservationExpired

Domain event raised when a reservation expires before the order is confirmed.

---

# Owned Concepts

## Aggregates

- ProductStock
- StockReservation

## Commands

- ReserveStock
- ReleaseStockReservation
- ExpireStockReservation

## Internal Events

- StockReserved
- StockReservationFailed
- StockReleased
- StockReservationExpired

---

# External Inputs

- OrderSubmitted
- OrderCancelled
- OrderExpired

These are facts observed by Stock Management.

They are not events emitted by Stock Management.

---

# Bounded Context Rules

## BC-000 — Domain rules are authoritative

The rules in this file are authoritative for any Stock Management feature.

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

## BC-001 — Stock Management owns availability and reservation

Stock Management owns:

* product availability;
* stock reservation decisions;
* stock release;
* reservation expiration.

No other context decides whether stock can be reserved.

---

## BC-002 — Stock Management does not own the Order aggregate

Stock Management must not model Order as an aggregate it owns.

Invalid model:

```text
StockReservation
  Order
    Confirm()
```

Valid model:

```text
StockReservation
  OrderId?
```

The problem is not the number of classes.

The problem is decision authority.

---

## BC-003 — Stock Management does not confirm orders

Stock Management must not decide whether an order is confirmed.

Confirmation is a Sales decision that may depend on a stock reservation outcome.

Producing the outcome is not the same as consuming it.

---

## BC-004 — Order lifecycle events are external inputs

`OrderSubmitted`, `OrderCancelled`, and `OrderExpired` are external inputs.

They must not be modelled as internal Stock Management events.

An order-lifecycle fact that is not listed above must stay an open question until the team declares it.

It must not be silently promoted to an external input because it looks plausible.

---

## BC-005 — Stock facts are observable, not pushed

Stock Management emits stock-related events that other contexts may observe.

Stock Management must not decide how those contexts react.

The reaction to a failed reservation belongs to the context that owns the order lifecycle.

---

# Confirmed Policies

- Stock can be reserved only if enough quantity is available.
- Reserved stock reduces the quantity available for other reservations.
- Released stock becomes available again.

These are settled.

Generated artifacts may rely on them.

---

# Proposed Policies

- A stock reservation may expire after a defined reservation window.
- A failed reservation may trigger a reaction in Order Management.

These are not settled.

Generated artifacts must mark them as proposed.

They must not be promoted to confirmed rules without an explicit team decision.

---

# Open Questions

- How long should a stock reservation remain valid?
- Can stock be partially reserved?
- Should a failed reservation distinguish between unavailable stock and invalid product?
- Which context decides whether to retry a failed reservation?

An open question is domain knowledge.

It must survive into the generated specification as an open question or `[NEEDS CLARIFICATION]`.

Answering one of these questions inside a generated artifact is a rule violation, not a helpful completion.

---

# Forbidden Assumptions

- Do not assume that stock reservation confirms the order.
- Do not assume that payment authorization guarantees stock availability.
- Do not introduce shipment concerns inside Stock Management.
- Do not model Order as an aggregate owned by Stock Management.

These are written down because they are the mistakes that recur.

Payment and shipment appear next to stock in most e-commerce training data.

Their absence from this context is a decision, not an omission.

---

# Policy Invention Rules

The agent must not invent:

* a reservation window;
* a retry policy;
* a partial-reservation rule;
* a failure-classification rule;
* a compensation or refund behaviour.

If a needed policy is missing, the correct output is an open question.

A plausible default is still an invention.

---

# Output Expectations

When asked to reason about this context:

- Use only the vocabulary defined above.
- Separate internal events from external inputs.
- Mark uncertain concepts as proposed.
- Preserve open questions instead of inventing missing policies.
- Return structured output when requested.

---

# Rules for `/speckit.specify`

The generated specification must:

1. Assign to Stock Management only the responsibilities listed under Owned Concepts.
2. Treat order-lifecycle facts as external inputs.
3. Keep confirmation, payment authorization, and shipment out of scope.
4. Separate confirmed rules from proposed rules.
5. Carry every open question forward unanswered.

---

# Rules for `/speckit.plan`

The generated plan must:

1. Keep ProductStock and StockReservation as the only Stock Management aggregates.
2. Use external decision references instead of embedded Sales, Payment, or Shipment models.
3. Avoid planning work that depends on an unresolved policy.
4. Mark such work as blocked rather than choosing a default.

---

# Rules for `/speckit.tasks`

Generated tasks must:

1. Implement only responsibilities assigned to Stock Management.
2. Handle `OrderSubmitted`, `OrderCancelled`, and `OrderExpired` as external inputs.
3. Emit `StockReserved` and `StockReservationFailed` as internal events.
4. Mark tasks depending on unresolved decisions as `BLOCKED`.

Valid task examples:

```markdown
- [ ] Handle `OrderSubmitted` as an external input.
- [ ] Reserve stock when the requested quantity is available.
- [ ] Emit `StockReserved` after a successful reservation.
- [ ] Emit `StockReservationFailed` when stock cannot be reserved.
```

Invalid task examples:

```markdown
- [ ] Confirm the order after stock has been reserved.
- [ ] Authorize payment before reserving stock.
- [ ] Create a shipment after stock has been reserved.
- [ ] Retry a failed reservation three times before failing the order.
- [ ] Expire a reservation after fifteen minutes.
```

---

# Final Principle

SDD does not guarantee that the model will always obey.

SDD makes violations inspectable.

If an artifact violates these rules, the issue should be reported as a named rule violation, for example:

```text
The specification violates BC-003 and BC-004.
```

Not:

```text
This specification feels wrong.
```
