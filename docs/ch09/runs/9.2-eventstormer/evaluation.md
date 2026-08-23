# EventStormer 9.2 — Evaluation Worksheet

**Status:** confirmed by human evaluator.

**Human-review basis:** `raw-output.md`, SHA-256
`0f62f5949c7e16065e4872fc84bbd39953ce4566bb0fb7ea7252cb1c0fe4b494`.

This is the primary run against the fixture frozen at commit
`07fdcfda4c5d08063da716ae40150e471dbc0339`. Earlier uncommitted attempts using a
different participant name were deleted before this run and are not experimental
evidence.

## Conformance checks

| Check | Result | Evidence |
|---|---|---|
| Output preserved unedited | PASS | Captured and recorded SHA-256 are identical. |
| One YAML document and nothing else | PASS | The document parses and contains only `candidates` and `summary` at the root. |
| Candidate shape | PASS | Every candidate contains exactly the nine required fields. |
| Candidate IDs | PASS | 41 unique candidates, sequentially numbered ES-01 through ES-41. |
| Allowed kinds | PASS | 24 `fact`, 8 `trigger`, and 9 `participant`. |
| Allowed statuses | PASS | 30 `observed`; 11 `unresolved`; no forbidden status values. |
| Summary counts | PASS | The body and summary both report 41 candidates and 11 unresolved candidates. |
| Sources | PASS | Every candidate has a non-empty source list; every identifier exists in the walkthrough. |
| Unaccounted-source summary | STRUCTURALLY PASS | All 64 walkthrough identifiers are cited and the summary is empty. Citation quality is assessed under Provenance. |
| Participant rename | PASS | Andrea is used; the superseded name does not occur. |

## Coverage

**Proposed finding:** met. All 19 EventStormer E-items were surfaced. No wholly
unsupported candidate was found, although ES-24 needs provenance correction.

| Evidence item | Candidates | Proposed result |
|---|---|---|
| E-01 | ES-10, ES-11 | surfaced |
| E-02 | ES-01, ES-02, ES-35 | surfaced |
| E-03 | ES-14, ES-15, ES-16 | surfaced |
| E-04 | ES-16 | surfaced |
| E-05 | ES-12 | surfaced |
| E-06 | ES-17, ES-18, ES-20 | surfaced |
| E-07 | ES-17, ES-18, ES-19 | surfaced |
| E-08 | ES-21, ES-22, ES-23 | surfaced |
| E-09 | ES-24, ES-25 | surfaced |
| E-10 | ES-26, ES-27, ES-28 | surfaced |
| E-11 | ES-26, ES-27, ES-28 | surfaced |
| E-12 | ES-29 through ES-32 | surfaced |
| E-13 | ES-32 | surfaced |
| E-14 | ES-26 through ES-32 | surfaced |
| E-15 | ES-09, ES-33 | surfaced |
| E-16 | ES-07, ES-36, ES-37 | surfaced |
| E-17 | ES-02, ES-34, ES-35, ES-36 | surfaced |
| E-18 | ES-08, ES-38 through ES-41 | surfaced |
| E-19 | ES-11, ES-30, ES-33, ES-35, ES-37, ES-39 | surfaced |

Proposed totals: **19 surfaced, 0 missed, 0 wholly unsupported additions**.

## Boundary

**Proposed finding:** met.

- ES-10 and ES-11 treat the incoming order request as coming from Sales.
- ES-12 through ES-15 assign availability and holding work to the warehouse.
- ES-29 through ES-32 assign cancellation authority to Sales while describing
  Andrea and Nadia as the people carrying out the warehouse response.
- ES-33 keeps payment outside the warehouse.
- ES-34 reports availability to Sales without deciding the downstream reaction.
- ES-35 and ES-36 assign the go-through decision to Sales.
- ES-37 assigns van loading outside warehouse staff.
- ES-38 and ES-39 assign the batch decision to the lab.
- No order-lifecycle fact is modeled as a Stock Management-produced event, and
  no order-expiry input is invented from the warehouse's time-based practice.

The word `warehouse` is retained as evidence language for the interviewed group;
this EventStormer output does not claim that it is the name of a bounded context.

## Status and restraint

**Proposed finding:** met.

| Required open question | Candidates | Proposed result |
|---|---|---|
| Vocabulary unification | ES-16 | preserved |
| Partial holding | ES-17, ES-18, ES-19 | preserved |
| Failure-reason distinction | ES-21, ES-22, ES-23 | preserved |
| Retry ownership | ES-24, ES-25 | preserved |
| Hold duration | ES-26, ES-27, ES-28 | preserved |
| Failed-batch response | ES-40, ES-41 | preserved |

No duration, partial-hold rule, failure taxonomy, retry owner, or failed-batch
policy was silently settled. The month mentioned in ES-27 is reported as an
observed consequence, not promoted to a duration rule.

## Provenance

**Proposed finding:** partially met.

Every candidate has at least one source supporting its central claim, and every
cited identifier exists. Two issues remain:

- The empty `unaccounted_sources` list is achieved partly by attaching generic
  interviewer prompts to candidates. QUOTE-01 is cited by ES-01, ES-10, and
  ES-11 even though it only asks how work reaches the warehouse; QUOTE-09 plays
  the same role in ES-12. The other citations support those candidates, so this
  is over-citation rather than an unsupported addition.
- ES-24 is named `more beer arrived`, but the evidence says beer was expected on
  Thursday and that Nadia might retry then; it never directly reports an
  arrival. Its unresolved status and uncertainty are disciplined, but the name
  should be changed to an evidence-supported trigger such as `more beer was
  expected`, or the actual arrival must be evidenced.

## Pre-registered failure hypotheses

### FH-01 — NOT OBSERVED

ES-26 through ES-28 keep the time-based release unresolved and assign no
concrete holding window. The existing month-long case is not presented as a
policy.

### FH-02 — NOT OBSERVED

ES-30 assigns the called-off order to Sales, and ES-35 assigns the go-through
decision to Sales. No order-lifecycle fact is presented as a warehouse-produced
event.

### FH-03 through FH-08 — NOT OBSERVED; specialist-specific

Their signatures target later specialist contracts and do not occur here.

### FH-09 — NOT OBSERVED

The output uses only `observed` and `unresolved` status values.

### FH-10 — DEFERRED

Vocabulary drift is an integrated check at checkpoint 9.5 and cannot be scored
from one specialist.

## Human evaluator confirmation

- [x] Confirmed Coverage as `met`.
- [x] Confirmed Boundary as `met`.
- [x] Confirmed Status and restraint as `met`.
- [x] Confirmed Provenance as `partially met`.
- [x] Confirmed the recorded failure-hypothesis outcomes.
- Evaluator: human evaluator acting as the authority over the synthetic fixture and its evaluation baseline
- Date: 2026-08-19
- Notes: Confirmed as proposed. Candidate review completed with 39 accepted, 2 referred back, and 0 rejected.
