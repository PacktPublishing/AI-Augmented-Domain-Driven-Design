# Decision rights

## Orchestrator

May decide:

- eligibility;
- legal protocol transition;
- duplicate or stale delivery;
- mechanical artifact validity;
- retry exhaustion;
- timeout or cancellation outcome;
- allowed tool set.

Must not decide:

- domain policy;
- semantic equivalence of terms;
- accepted ubiquitous language;
- ownership when evidence leaves it unresolved;
- whether a proposed interpretation becomes domain truth.

## Specialist

May transform accepted input according to its contract.

It produces candidate material only.

## Semantic reviewer

May report possible divergence.

It does not rewrite the candidate and does not accept it.

## Human reviewer

May accept, refer, reject, or explicitly resolve a domain question when the
reviewer has the required authority.

The historical Chapter 9 `decision_owner` field is not treated as one uniform
machine-enforceable semantic field. Chapter 10 uses explicit orchestration rights
instead.
