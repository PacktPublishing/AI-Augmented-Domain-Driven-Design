# Chapter 9 — Integrated Cross-Specialist Evaluation at 9.5

Classification: **draft integrated evaluation against the authoring-reference Context Mapper output**. The 9.2–9.4 repository runs remain the recorded specialist evidence. The 9.5 result in this package must be rerun in an evaluator-blind staged workspace before this document can become the final checkpoint evaluation.

## What is being checked

The integrated checkpoint asks whether the four transformations together preserve the carrier concepts that are reachable from the walkthrough, avoid importing the carrier concept that is not evidenced, and keep provenance, authority, unresolved policy, and vocabulary distinctions visible across handoffs.

## Reachable carrier concepts

| Carrier concept reachable from evidence | Evidence | Chain representation |
|---|---|---|
| `ProductStock` | E-05 | ES-12 and the later availability behavior distinguish quantity in the room from quantity available to hold. |
| `StockReservation` | E-03 | ES-14/ES-15, scenario material, and CEW-03–CEW-06 preserve physical set-aside and recorded hold. |
| `ReserveStock` | E-03 | CEW-03 and CEW-05 are candidate instructions for the evidenced set-aside/hold work. |
| `ReleaseStockReservation` | E-12 | CEW-25/CEW-26 describe the cancellation-triggered release work without moving cancellation authority into Stock. |
| `ExpireStockReservation` | E-10 | CEW-20/CEW-22 retain time-based clearing as unresolved warehouse-side work. |
| `StockReserved` | E-03 | CEW-04/CEW-06 record the observed set-aside/hold outcomes. |
| `StockReservationFailed` | E-06 | CEW-10/CEW-15 preserve the unsuccessful availability outcome and return to Sales. |
| `StockReleased` | E-12, E-13 | CEW-27 records cancelled-order beer returned to general stock and immediately available again. |
| `StockReservationExpired` | E-10, E-14 | CEW-21/CEW-23 preserve the distinct time-based clearing path without turning it into a Sales order-expiry event. |
| external input — order placed | E-01 | ES-10/ES-11 and CM-07 keep the request incoming from Sales. |
| external input — order called off | E-12 | ES-29/ES-30, CEW-24, and CM-09 keep cancellation incoming and Sales-owned. |

All **11 carrier concepts reachable from the evidence** can be mapped across the chain. No single specialist contains all 11, which is consistent with the intended specialization.

## Carrier concept intentionally absent

The carrier's external input for an order expired is **not reachable** from the walkthrough and is not recovered by the chain. The time-based clearing records remain warehouse-side hold behavior; they are not reinterpreted as Sales expiring an order. This preserves the distinction pre-registered in the evidence inventory.

## Cross-specialist status discipline

### FH-09 — NOT OBSERVED in the four specialist outputs examined

The recorded ES, ST, and CEW outputs use only allowed specialist statuses, and the 9.5 reference output does the same. `accepted` and `rejected` occur only in human-decision/derived-handoff material, not as specialist `status` values.

## Vocabulary continuity

### FH-10 — NOT OBSERVED in the reference chain

The walkthrough itself contains the competing expressions `set aside`, `put a hold on`, and `earmark`; that source-level variation is not, by itself, cross-specialist drift. The specialists keep the ambiguity traceable rather than silently replacing it with different canonical terms: ES-16 records the three expressions explicitly, the Storyteller keeps the distinction between physical set-aside and the recorded hold open, CEW-06 carries the same terminology in `uncertainty`, and CM-01 moves to a responsibility-level boundary name while stating that the business has not used that name.

No pair of specialist outputs was found to give the same domain concept incompatible names as though both were established, and no reused name was found carrying two incompatible meanings. A human language decision is still required before the source vocabulary can be canonicalized.

## Provenance across handoffs

The chain is strongest from evidence to facts and from facts to scenarios. The known 9.4 limitation remains: CEW candidates cite walkthrough evidence directly rather than scenario identifiers, so a reviewer reconstructs the scenario link by opening the staged accepted-scenarios file. The 9.5 reference output likewise cites walkthrough identifiers as required by the shared schema. The final map therefore preserves source provenance but does not repair the schema-level missing explicit upstream-link field.

## Authority continuity

Sales remains the order decision authority; the lab remains the failed-batch decision source; payment remains outside the availability responsibility; and dispatch authority remains unknown. The Context Mapper does not convert information direction into decision ownership. In particular, CM-12 records physical collection by shipping staff without making Stock the creator or owner of shipment work.

## Uncertainty continuity

The four carrier open questions remain unresolved where they appear downstream: hold duration, partial availability, failure classification, and retry ownership are not settled by the Context Mapper. The new failed-batch case likewise remains open at CM-06/CM-11. No later artifact turns repetition into acceptance.

## Final checkpoint status

The integrated interpretation is coherent enough to use as the **expected review outcome**, but checkpoint 9.5 is not experimentally complete in this package because the Context Mapper reference was generated after evaluator-baseline exposure and the human gate is still pending. A fresh Codex run plus a human decision is required before tagging 9.5 or quoting its candidate counts as observed experimental results in the chapter.
