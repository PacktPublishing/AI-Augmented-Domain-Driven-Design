# Context Mapper 9.5 — Primary Evaluation

Classification: **primary experimental run**. The specialist was executed by
Codex in a fresh controlled-input workspace containing only the frozen contract,
the three accepted upstream handoffs, the walkthrough, glossary, constitution
excerpt, and handoff schema.

Review basis: `raw-output.md`, SHA-256
`84c779c250b3fb33f71531d240e135d3603cfbc81a2ab330be22f71f794e2c85`.

## Contract conformance

The raw output is one YAML document containing 18 sequential candidates: 8
`boundary` and 10 `relationship` records. The body contains 6 `inferred`, 8
`observed`, and 4 `unresolved` statuses. The summary incorrectly reports 5
unresolved candidates, so its count does not conform to the returned body.

Three candidate-level defects were referred for a guided correction:

- CM-10 cites `QUOTE-01`, an interviewer prompt that does not support the order
  contents or direction claimed by the relationship.
- CM-16 assigns `decision_owner: warehouse` because warehouse staff performed
  one notification, although the evidence does not establish the authority that
  decides what must be passed.
- CM-17 names held beer as released for delivery and assigns Sales as the
  relationship owner, although the evidence establishes only that Sales decides
  an order goes through and Shipping later collects beer.

`correction-01/raw-output.md` corrects those three candidates without replacing
or editing the primary output. The effective body count remains 18 candidates,
of which 4 remain `unresolved`.

## Coverage

Every item in the pre-registered Context Mapper subset is surfaced:

| Evidence item | Effective candidate(s) | Reading |
|---|---|---|
| E-01 | CM-02, CM-03, CM-09, CM-10 | Orders originate with customers, pass through Sales, and reach availability with identifiers, products, and quantities. |
| E-02 | CM-01, CM-02 | Availability answers the request; Sales retains order progression. |
| E-12 | CM-01, CM-11 | A cancellation crosses from Sales and causes warehouse-side release work. |
| E-15 | CM-05 | Payment remains outside and no direct payment edge is invented. |
| E-16 | CM-07, corrected CM-17, CM-18 | Delivery remains separate; the map records only physical collection and onward delivery. |
| E-17 | CM-02, CM-13, corrected CM-17 | Sales decides that an order goes through; warehouse availability is one input. |
| E-18 | CM-04, CM-06, CM-15, corrected CM-16 | The lab decision crosses a boundary; failed-batch resolution remains unsettled. |
| E-19 | CM-05, CM-07, corrected CM-17 | Payment, approval, and delivery are not turned into warehouse decisions. |

CM-08, CM-12, and CM-14 surface accepted material outside the scored subset:
product-code correction, retry, and partial-quantity notification. They are
retained as observations, not counted as additional Coverage credit.

## Boundary

No boundary is named `Warehouse`; CM-01 discloses that its responsibility-based
name is proposed. Sales retains order progression, the lab retains the batch
decision, payment remains with the office, and delivery authorization remains
unknown. The correction to CM-17 removes the unsupported implication that Sales
releases beer or instructs Shipping.

## Status and restraint

The four carrier open questions remain visible rather than answered:

- hold duration remains unsettled in CM-01;
- partial availability remains unresolved in CM-14;
- failure classification remains unsettled in CM-08 and CM-13;
- retry ownership remains unresolved in CM-12.

The new failed-batch responsibility also remains unresolved in CM-06, while
corrected CM-16 keeps notification authority unknown. Candidate acceptance must
not settle any of those questions or turn a proposed boundary name into business
terminology.

## Provenance

Every effective candidate has at least one stable source identifier that exists
and supports its claim. Correction 01 removes the unsupported `QUOTE-01` citation
from CM-10 and narrows CM-16 and CM-17 to the authority and exchanges actually
present in the walkthrough.

## Pre-registered hypotheses

### FH-07 — NOT OBSERVED

The primary output uses a responsibility-based name for CM-01 and explicitly
states that the business names the implementing group rather than the proposed
boundary.

### FH-08 — OBSERVED in the primary output; removed by correction

Primary CM-17 turns sequence into authority by assigning Sales as the owner of a
relationship named `held beer released for delivery`. Corrected CM-17 records
only physical collection by Shipping and restores `decision_owner: unknown`.

### FH-09 — NOT OBSERVED

Neither the primary nor correction output uses `accepted` or `rejected` as a
specialist status.

## Human-gate recommendation

Accept the effective 18-candidate set as traceable modeling material, using the
corrected versions of CM-10, CM-16, and CM-17. Preserve CM-06, CM-08, CM-12, and
CM-14 as `unresolved`; preserve every proposed boundary name as unapproved
business language; and assign no delivery, retry, failed-batch, or partial-stock
authority beyond the evidence. This recommendation is not the human decision.
