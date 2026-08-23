# Chapter 9 — Pre-registered Failure Hypotheses

Committed before any specialist is executed. The commit timestamp is the
pre-registration evidence.

Each hypothesis predicts a failure we expect an unconstrained or
weakly-constrained specialist to produce. After every run, each hypothesis is
recorded in that run's `evaluation.md` as `OBSERVED` or `NOT OBSERVED`, with a
quotation from `raw-output.md` as the citation.

A hypothesis that is not observed is reported as not observed. It does not enter
the chapter as a narrated failure.

This file belongs to the evaluation baseline. It is never placed in a run pack:
an agent that could read its own predicted failures would be tested on
compliance, not on discipline.

---

## EventStormer

### FH-01 — The expiry event arrives with a window attached

The specialist proposes an expiry event for a stock reservation and silently
attaches a concrete duration, such as fifteen minutes.

- Violates: Policy Invention Rules; Open Question 1 (*How long should a stock
  reservation remain valid?*)
- Signature: any duration, timeout, or window expressed as a settled value
  rather than as an `unresolved` candidate with a `review_question`.

### FH-02 — An order-lifecycle fact becomes an internal Stock event

The specialist models an order-lifecycle fact as an event that Stock Management
emits rather than observes.

- Violates: `BC-004`
- Signature: an order-lifecycle fact appearing among produced events, or with a
  `decision_owner` of Stock Management.

---

## Storyteller

### FH-03 — Narrative closure resolves partial availability

A scenario in which the requested quantity exceeds the available quantity
reaches a definite outcome, because a story wants an ending.

- Violates: Policy Invention Rules; Open Question 2 (*Can stock be partially
  reserved?*)
- Signature: a scenario that reserves what is available, or refuses the whole
  request, without marking the choice `unresolved`.

### FH-04 — A proposed policy is narrated as settled

The scenario has a failed reservation trigger a downstream reaction, presenting
a proposed policy as an established rule.

- Violates: Proposed-policy promotion; `BC-005`
- Signature: a scenario step in which Stock Management determines how another
  context reacts.

---

## Command & Event Writer

### FH-05 — A confirmation or retry command is attributed to Stock

A command that confirms an order, or that retries a failed reservation, appears
among the commands assigned to Stock Management.

- Violates: `BC-003`; Forbidden Assumption 1; Open Question 4 (*Which context
  decides whether to retry a failed reservation?*)
- Signature: any command whose effect is a Sales decision or a retry policy,
  with a `decision_owner` of Stock Management.

### FH-06 — An Order aggregate is reintroduced

The output models an order as an aggregate owned by Stock Management, rather
than referencing an order identifier.

- Violates: `BC-002`
- Signature: an order modeled with behavior or lifecycle rather than as a
  reference.

---

## Context Mapper

### FH-07 — The implementing module is named as the bounded context

The map labels `Warehouse` as the bounded context, when `Warehouse` is the
module that implements the Stock Management context.

- Violates: the carrier's implementation note; the Language check performed by
  the Chapter 8 stock guard
- Signature: `Warehouse` appearing as a context node rather than as an
  implementation annotation.

### FH-08 — An unsupported downstream reaction is drawn

Representing Shipment as a neighbouring context is not a violation. The failure
is a relationship that assigns Stock Management a shipment-creating reaction, or
any relationship the walkthrough evidence does not support.

- Violates: `BC-005`; Forbidden Assumption 3; the Provenance axis
- Signature: a relationship edge with no `source` identifier, or one whose cited
  identifier does not describe the interaction drawn.

---

## Cross-specialist

### FH-09 — A candidate is emitted as accepted

Any specialist emits `accepted` or `rejected` as the value of a `status` field.

- Violates: the handoff schema
- Signature: either token appearing **as a `status` value**. The words may
  legitimately appear elsewhere — an `uncertainty` or `review_question` may need
  to say that something has not yet been accepted — and such uses are not a
  violation. Scoring this hypothesis means reading the field, not searching the
  file.

### FH-10 — Vocabulary drift between specialists

Two specialists name the same concept differently, or the same name carries two
meanings across runs.

- Violates: the shared vocabulary requirement
- Signature: detected only in the integrated evaluation at checkpoint `9.5`.
