# Chapter 9 — Integrated Cross-Specialist Evaluation at 9.5

Classification: **completed integrated evaluation against the primary Context
Mapper run, guided correction, and completed human gate**.

## Reachable carrier concepts

| Carrier concept reachable from evidence | Evidence | Chain representation |
|---|---|---|
| `ProductStock` | E-05 | ES-12 and downstream availability behavior distinguish physical quantity from quantity available to hold. |
| `StockReservation` | E-03 | ES-14/ES-15, scenarios, and CEW-03–CEW-06 preserve physical set-aside and the recorded hold. |
| `ReserveStock` | E-03 | CEW-03 and CEW-05 remain candidate instructions for the evidenced work. |
| `ReleaseStockReservation` | E-12 | CEW-25/CEW-26 preserve cancellation-triggered release without moving cancellation authority into Stock. |
| `ExpireStockReservation` | E-10 | CEW-20/CEW-22 retain time-based clearing as unresolved warehouse-side work. |
| `StockReserved` | E-03 | CEW-04/CEW-06 record the observed set-aside and hold outcomes. |
| `StockReservationFailed` | E-06 | CEW-10/CEW-15 preserve the unsuccessful availability outcome returned to Sales. |
| `StockReleased` | E-12, E-13 | CEW-27 records cancelled-order beer becoming available again. |
| `StockReservationExpired` | E-10, E-14 | CEW-21/CEW-23 preserve time-based clearing without turning it into Sales expiring an order. |
| external input — order placed | E-01 | ES-10/ES-11 and corrected CM-10 keep the request incoming from Sales. |
| external input — order called off | E-12 | ES-29/ES-30, CEW-24, and CM-11 keep cancellation incoming and Sales-owned. |

All 11 carrier concepts reachable from the evidence remain represented across
the chain. The carrier's order-expired external input remains absent because the
walkthrough does not evidence it.

## Cross-specialist discipline

FH-09 is not observed: all specialist outputs use only the four allowed
specialist statuses. Human decisions remain separate artifacts.

FH-10 is not observed in the effective chain. Source-level alternatives such as
`set aside`, `put a hold on`, and `earmark` remain traceable. The Context Mapper
introduces responsibility-level names only while disclosing that the business
has not approved them.

## Provenance and authority continuity

The known schema limitation remains: later specialists cite walkthrough
identifiers but do not carry a dedicated upstream-candidate link field. Reviewers
can reconstruct the chain from the staged accepted handoffs, but the schema does
not make every hop explicit.

Sales remains the order-progression authority; the lab remains the failed-batch
decision source; payment remains outside availability; and Shipping collection
authority remains unknown. Correction 01 prevents the physical sequence after a
Sales decision from becoming an invented Sales-to-Shipping instruction.

## Uncertainty continuity

Hold duration, partial availability, failure classification, and retry ownership
remain open. Product-code correction, failed-batch consequences, notification
contents, payment effects, and Shipping authorization also remain explicit
questions. No later artifact turns repetition into policy.

## Final checkpoint status

Checkpoint 9.5 is complete. The primary run, guided correction, evaluation,
human decision, and effective 18-candidate handoff are recorded. Acceptance
advances the candidates as modeling material while CM-06, CM-08, CM-12, and
CM-14 remain `unresolved`; it does not approve boundary names or implementation
structure.
