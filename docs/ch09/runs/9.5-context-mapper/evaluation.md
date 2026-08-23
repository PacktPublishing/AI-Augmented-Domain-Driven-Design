# Context Mapper 9.5 — Reference Evaluation

Classification: **authoring reference, not primary experimental evidence**. The candidate output was prepared in a session that had already inspected the evaluation baseline. This file records what the reference output would score; it must not be presented as the pre-registered experiment result until the specialist is rerun from the staged run pack without evaluator-only material.

Review basis: `raw-output.md`, SHA-256 `567ffe9b06794160ecb53c45a3e452a077c11ed5019deb2dbc409783b308cc43`.

## Contract conformance

The reference output is one valid YAML document with 12 sequential `CM` candidates, all required fields, non-empty evidence source lists, and only permitted statuses. It contains 6 `boundary` and 6 `relationship` candidates: 2 `unresolved`, 5 `inferred`, and 5 `observed`. No candidate uses `accepted` or `rejected` as a specialist status.

## Coverage

All Context Mapper items in the pre-registered subset are surfaced:

| Evidence item | Reference candidate(s) | Reading |
|---|---|---|
| E-01 | CM-02, CM-07 | Sales sends order details to the availability responsibility. |
| E-02 | CM-01, CM-02, CM-08 | The availability responsibility does not acquire the Sales decision about whether the customer gets the goods. |
| E-12 | CM-09 | A cancellation crosses from Sales and remains Sales-owned. |
| E-15 | CM-03 | Payment remains outside; no direct payment edge is invented. |
| E-16 | CM-04, CM-12 | Delivery is separate and the edge records physical collection only. |
| E-17 | CM-02, CM-08 | Sales decides the order goes through; warehouse availability is one input. |
| E-18 | CM-05, CM-06, CM-10, CM-11 | The lab decision crosses a boundary; the unsettled office handling remains unresolved. |
| E-19 | CM-03, CM-04, CM-05, CM-12 | Second-hand payment, approval, and delivery are kept outside the availability responsibility. |

`unaccounted_sources` is empty because accepted internal holding/release/failure material can be placed inside CM-01 without creating additional external edges.

## Boundary

The map uses responsibility names rather than the implementing module name. `warehouse` appears only as the observed group/decision owner inside evidence statements; no candidate boundary is named `Warehouse`. Sales remains the commercial authority, the lab remains the source of the failed-batch decision, payment stays with the office, and delivery is represented without assigning dispatch authority to Stock.

The payment responsibility is intentionally left without a direct relationship to the availability boundary because the walkthrough says the warehouse learns payment information second-hand but does not establish the route. No Sales-to-delivery edge is drawn for the same reason.

## Status and restraint

CM-06 and CM-11 remain `unresolved`: the office's failed-batch responsibility, the required handoff, and what the office does next are not settled. CM-04 and CM-12 preserve `decision_owner: unknown` for dispatch rather than assigning shipment authority to the warehouse. Proposed responsibility names are disclosed in `uncertainty` and remain review questions.

### FH-07 — NOT OBSERVED in the reference output

No boundary candidate is named `Warehouse`; CM-01 is named for the responsibility (`beer availability and holds`) and explicitly says the business has not used that boundary name.

### FH-08 — NOT OBSERVED in the reference output

Every relationship cites a passage that shows the stated exchange. CM-12 records only that held beer is physically taken by the shipping lads after Sales decides the order goes through, and explicitly refuses to claim that the warehouse creates, requests, or authorizes a shipment.

## Provenance

Every relationship has direct walkthrough evidence for the exchange it draws. CM-03 is deliberately unconnected where the evidence identifies payment as a neighbour but does not show a direct exchange. CM-06 keeps payment handling and failed-batch order handling separate rather than inferring that the same office label proves one shared responsibility.

## Human-gate recommendation

The 12 candidates are traceable enough to review as modeling material. The recommended gate decision is to accept them **as candidate boundary material while preserving CM-06 and CM-11 as unresolved**. This recommendation is not itself the human decision.
