# EventStormer 9.2 — Human Decision

**Status:** complete — 41 of 41 primary candidates decided; correction 01 complete.

**Review basis:** `raw-output.md`, SHA-256
`0f62f5949c7e16065e4872fc84bbd39953ce4566bb0fb7ea7252cb1c0fe4b494`.

For each candidate, the domain authority must enter `accepted`, `rejected`, or
`referred back`, together with a reason. Blank cells are intentionally
undecided.

| Candidate | Name | Decision | Reason |
|---|---|---|---|
| ES-01 | warehouse | accepted | Evidence-backed participant; this does not establish a formal bounded-context name. |
| ES-02 | Sales | accepted | Evidence-backed commercial participant and authority over order progression. |
| ES-03 | customer | accepted | Evidence-backed external participant whose actions reach the warehouse through Sales. |
| ES-04 | the board | accepted | Evidence-backed participant in the current workflow, not a commitment to the future model. |
| ES-05 | the screen | accepted | Evidence-backed operational source; the meaning of its quantity remains open. |
| ES-06 | end bay | accepted | Evidence-backed physical participant, not automatically a domain concept. |
| ES-07 | the shipping lads | accepted | Evidence-backed business wording; a formal name may be normalized downstream. |
| ES-08 | the lab | accepted | Evidence-backed participant and source of the batch decision; precise role and criteria remain open. |
| ES-09 | the office | accepted | Evidence-backed collective participant; its precise roles and authority remain unresolved. |
| ES-10 | order came through from Sales | accepted | Evidence-backed external trigger; the extra interviewer-prompt citation remains an evaluation issue. |
| ES-11 | order request was received | accepted | Evidence-backed fact received from Sales and explicitly not caused by the warehouse. |
| ES-12 | quantity available to hold was worked out | accepted | Accepted as an observed calculation practice, not as a newly approved policy. |
| ES-13 | enough beer could be had | accepted | Evidence-backed trigger for separating beer and recording a hold. |
| ES-14 | beer was set aside | accepted | Evidence-backed physical action performed by Nadia. |
| ES-15 | hold was put on | accepted | Evidence-backed recorded action performed by Andrea; its relationship to the physical action remains open. |
| ES-16 | set aside, put a hold on, and earmark were treated as the same thing | accepted | Accepts the observed vocabulary ambiguity without resolving whether it represents one fact or two. |
| ES-17 | requested quantity was short | accepted | Evidence-backed trigger that preserves both contradictory practices without selecting a policy. |
| ES-18 | short order was put back as not done | accepted | Accepted as Andrea's observed practice, not as an agreed short-quantity policy. |
| ES-19 | available part of a short order was put in the end bay | accepted | Accepted as Nadia's observed conflicting practice, not as an agreed policy. |
| ES-20 | order was returned to Sales as not done | accepted | Evidence-backed warehouse response whose failure reason remains undisclosed. |
| ES-21 | beer was genuinely out | accepted | Evidence-backed distinction for a temporary stock shortage. |
| ES-22 | product code was not a beer BrewUp made | accepted | Evidence-backed distinct failure cause; correction authority remains unknown. |
| ES-23 | reasons for a no were reported as the same result | accepted | Accepts the observed reporting behavior while leaving future classification open. |
| ES-24 | more beer arrived | referred back | The evidence establishes expected replenishment, not an actual arrival; rename the trigger to match the evidence. |
| ES-25 | out-of-stock order was tried again | accepted | Evidence-backed inconsistent retry practice with no settled trigger or owner. |
| ES-26 | time had passed | accepted | Preserves the absence of a defined duration and decision authority. |
| ES-27 | board was cleared because time had passed | accepted | Accepts Nadia's observed practice without promoting Friday or one month to policy. |
| ES-28 | beer was put back after a hold was cleared | accepted | Evidence-backed physical outcome whose time-based authority remains unresolved. |
| ES-29 | order was called off | referred back | Rename the trigger to distinguish receipt of the Sales cancellation from the identically named fact ES-30. |
| ES-30 | order was called off | accepted | Evidence-backed external order-lifecycle fact communicated by Sales and not caused by the warehouse. |
| ES-31 | cancelled order was rubbed off the board | accepted | Evidence-backed warehouse action with authority derived from the Sales cancellation. |
| ES-32 | cancelled order's beer was put back | accepted | Evidence-backed return to general availability after cancellation. |
| ES-33 | order was found not to have been paid for | accepted | Evidence-backed second-hand payment fact kept outside warehouse authority. |
| ES-34 | Sales was told the beer was there | accepted | Evidence-backed warehouse notification that leaves the downstream Sales decision outside the warehouse. |
| ES-35 | order went through | accepted | Evidence-backed Sales decision observed by the warehouse and not caused by it. |
| ES-36 | order went through properly | accepted | Evidence-backed trigger for shipping collection, distinct from ES-35 by its trigger role. |
| ES-37 | beer was taken onto a van | accepted | Evidence-backed shipping action kept outside warehouse staff and authority. |
| ES-38 | batch failed its check | accepted | Evidence-backed trigger owned by the lab. |
| ES-39 | batch was said not to be going out | accepted | Evidence-backed external lab fact observed by the warehouse. |
| ES-40 | failed batch holds were taken off the board | accepted | Accepts the one observed occurrence without promoting it to an agreed practice. |
| ES-41 | office was told about failed batch holds | accepted | Accepts the observed notification while preserving the unresolved office procedure and authority. |

## Domain-authority notes

- Reviewer: domain authority participating in the Chapter 9 review
- Date: 2026-08-19
- General reason or constraints: Accepted candidates preserve observed facts and explicit uncertainty. ES-24 and ES-29 were referred back for evidence-aligned naming; no candidate was rejected.

## Correction 01 outcome

The primary decisions above remain an immutable record of the first human gate.
The referred candidates were corrected in `correction-01/raw-output.md` and
accepted in `correction-01/human-decision.md`.

| Candidate | Effective corrected name | Final decision |
|---|---|---|
| ES-24 | more beer was expected | accepted after correction |
| ES-29 | order cancellation was received from Sales | accepted after correction |

**Effective accepted set:** 41 candidates — 39 primary candidates plus the two
accepted corrected versions. No rejected or outstanding candidate remains.
