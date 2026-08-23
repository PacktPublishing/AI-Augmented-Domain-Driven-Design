# Command & Event Writer 9.4 — Human Decision

**Status:** complete — 36 of 36 primary candidates accepted.

**Review basis:** `raw-output.md`, SHA-256
`5abc8b75da79572185f53e06adbc9bdcac27097ec83079c37127613a8bc10813`.

All candidates were accepted as traceable modelling material. Acceptance does
not convert any `unresolved` candidate into business policy, command authority,
or implementation design.

## Domain-authority notes

- Reviewer: human evaluator acting as the authority over the synthetic fixture and its evaluation baseline
- Date: 2026-08-20
- General constraints: retain commands only as candidate descriptions of work
  observed in the evidence; treat Sales cancellation as the authority enabling
  the observed warehouse actions, not as proof of a separate Sales instruction;
  do not introduce an owner for retry, confirmation, expiry, or failed-batch
  policy.

## Accepted authority readings

| Candidates | Decision | Reason |
|---|---|---|
| CEW-01 | accepted | The Sales order is accepted as the external request that initiates an availability check; no broader Sales policy is inferred. |
| CEW-25, CEW-26 | accepted | The received Sales cancellation authorizes the observed warehouse actions; the source does not establish a new command route or a separate cancellation policy. |

**Effective accepted set:** 36 candidates, including 15 candidates whose
specialist status remains `unresolved`.
