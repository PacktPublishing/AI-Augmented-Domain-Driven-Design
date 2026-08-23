# Chapter 9 — Evidence Inventory

**Evaluation baseline. Never placed in a run pack.**

This file records what `stock-walkthrough.md` actually contains. It is written
together with the walkthrough and before any run, and it is the baseline for the
Coverage axis.

Coverage is measured against this inventory, not against the carrier. A carrier
concept that the walkthrough never evidences cannot be counted as missed. See
section 3.

---

## 1. Facts and behaviors present in the evidence

| ID | What the evidence contains | Source | Carrier counterpart |
|---|---|---|---|
| E-01 | Customer orders arrive from Sales with a number, product codes, and quantities | QUOTE-02, QUOTE-03 | External input — order submitted |
| E-02 | The warehouse does not decide whether the customer gets the goods | QUOTE-03 | `BC-003` |
| E-03 | Beer is separated from general stock against a particular order | QUOTE-05, QUOTE-06, QUOTE-08 | `StockReservation`, `ReserveStock`, `StockReserved` |
| E-04 | One action, three names: *set aside*, *put a hold on*, *earmark* | QUOTE-05 – QUOTE-08, OBS-03 | Vocabulary unification |
| E-05 | Two distinct quantities: what stands in the room, and what can still be promised | QUOTE-10, QUOTE-11, QUOTE-12 | `ProductStock`; confirmed policy that held stock reduces what remains |
| E-06 | When there is not enough, the action fails and the outcome goes back to Sales | QUOTE-14 | `StockReservationFailed` |
| E-07 | Short-quantity handling is contradictory between two staff and unwritten | QUOTE-16, QUOTE-17, QUOTE-19, QUOTE-20, OBS-04 | **Open Question 2** |
| E-08 | Two distinct causes of failure that the outgoing message does not distinguish | QUOTE-22, QUOTE-23, QUOTE-25 | **Open Question 3** |
| E-09 | Nobody owns trying again after a failure | QUOTE-27 – QUOTE-32 | **Open Question 4** |
| E-10 | Separated beer eventually returns to general stock because time has passed | QUOTE-34, QUOTE-46 | `StockReservationExpired`, `ExpireStockReservation` |
| E-11 | The holding period is unwritten, inconsistent, and person-dependent | QUOTE-36 – QUOTE-41 | **Open Question 1** |
| E-12 | Called-off orders arrive from Sales and cause the beer to be put back | QUOTE-43 | External input — order cancelled; `ReleaseStockReservation`, `StockReleased` |
| E-13 | Beer put back becomes generally available again | QUOTE-44 | Confirmed policy on released stock |
| E-14 | Two different triggers produce the same physical outcome: being told, and time passing | QUOTE-46 | Distinguishes release from expiry |
| E-15 | Payment status is unknown to the warehouse | QUOTE-48, QUOTE-49 | Forbidden Assumption 2 |
| E-16 | Loading and delivery are performed by other staff after the order goes through | QUOTE-51 | `BC-005`, Forbidden Assumption 3 |
| E-17 | Sales decides the order goes through; the warehouse is one input among several | QUOTE-53, QUOTE-54 | `BC-003` |
| E-18 | A batch can fail its check after beer has been set aside against orders, with no agreed practice | QUOTE-56 – QUOTE-58, OBS-05 | **Not in the carrier** |
| E-19 | Everything concerning payment, approval, and delivery reached the warehouse second-hand | OBS-06 | `BC-004` boundary signal |

---

## 2. Open questions reachable from the evidence

The walkthrough was written so that each of the carrier's four open questions is
*touched* by real evidence without being answered by it. This is what makes the
Status and restraint axis measurable: the specialist is under genuine pressure to
convert an unwritten practice into a policy.

| Carrier open question | Evidence that raises it | Evidence that settles it |
|---|---|---|
| How long should a hold remain valid? | E-11 | None |
| Can stock be partially held? | E-07 | None |
| Should a failure distinguish unavailable from invalid product? | E-08 | None |
| Which context decides whether to try again? | E-09 | None |

A specialist that returns any of these as `unresolved` with a `review_question`
has succeeded on this axis. A specialist that produces a duration, a
partial-hold rule, a failure taxonomy, or an owner for retrying has failed it,
however reasonable the answer.

E-18 is the inverse case: material that is genuinely new. The correct handling is
also `unresolved` with a `review_question`. Silently modeling it is a failure;
omitting it entirely is a Coverage observation.

---

## 3. Not present in the evidence

These may not be counted as Coverage misses.

- **Order expiry initiated by Sales.** The carrier lists an order-expired
  external input. The walkthrough contains warehouse-side clearing after time has
  passed (E-10, E-14), which is a different fact with a different owner. Nothing
  in the evidence describes Sales expiring an order.
- **Any numeric threshold**: no duration, no retry count, no minimum quantity.
- **Any statement of who ought to decide** the four open questions. The evidence
  establishes that nobody currently does.
- **Any compensation or refund behavior.**
- **Any named aggregate, command, or event.** The transcript is deliberately free
  of modeling vocabulary.

---

## 4. How Coverage is scored

A single ceiling cannot score four specialists whose outputs have different
shapes. An EventStormer that returns no context relationships has not missed
anything; a Context Mapper that returns none has.

Coverage is therefore scored in two places.

### Per run — against the E-items

Each specialist's contract declares its scope in prose. `E-` identifiers live in
this evaluation baseline, so a contract cannot cite them without putting
unresolvable identifiers in front of the agent. The subsets below are derived
from the contracts' scope sections and are pre-registered here, before any run.

| Specialist | Declared subset | Excluded, and why |
|---|---|---|
| EventStormer `ES` | E-01 – E-19 | None. It works directly on the evidence and is the only specialist scored against all of it. |
| Storyteller `ST` | E-03, E-05 – E-14, E-18 | E-02, E-15 – E-17, E-19 describe what this group does *not* do. They constrain scenarios but are not themselves situations to narrate. E-01 and E-04 are accounted for by the facts a scenario cites, not by the scenario. |
| Command & Event Writer `CEW` | E-01, E-03, E-05 – E-14, E-18 | E-02, E-15 – E-17, E-19 mark work owned elsewhere; correct handling is to propose no instruction, which is scored under Boundary rather than Coverage. E-04 is a naming matter settled upstream. |
| Context Mapper `CM` | E-01, E-02, E-12, E-15 – E-19 | The mechanics of holding, releasing, and failing sit inside one boundary and do not bear on where boundaries lie. E-18 is included: a batch failing after goods were set aside crosses a boundary. |

Coverage for a run is measured only against its declared subset, and recorded as
surfaced, missed, and unsupported additions.

The four open questions raised by E-07, E-08, E-09 and E-11 are scored under
Status and restraint for **every** specialist whose subset contains them, not
under Coverage. Surfacing one is Coverage; leaving it open is restraint.

Items outside a specialist's declared subset are neither credited nor charged. If
a run surfaces something outside its subset and does so well, that is recorded as
an observation, not as Coverage.

### At checkpoint 9.5 — against the carrier

The mapping from candidates to carrier concepts is evaluated once, on the four
outputs together. Any single specialist reaching every concept alone would mean
the specialization had failed.

---

## 5. Carrier concepts reachable from this evidence

```text
ProductStock                       E-05
StockReservation                   E-03
ReserveStock                       E-03
ReleaseStockReservation            E-12
ExpireStockReservation             E-10
StockReserved                      E-03
StockReservationFailed             E-06
StockReleased                      E-12, E-13
StockReservationExpired            E-10, E-14
external input, order placed       E-01
external input, order called off   E-12
```

Eleven reachable concepts.

**Not reachable: the order-expired external input.** This is the twelfth carrier
concept and the evidence does not support it. E-10 and E-14 describe the
warehouse clearing its own board because time has passed, which is a
warehouse-side fact with a warehouse-side trigger. Nothing in the evidence
describes Sales expiring an order, and `QUOTE-40` states explicitly that the
warehouse is never told what became of such orders.

A run that proposes an order-expiry external input is producing an unsupported
addition, not demonstrating recall.
