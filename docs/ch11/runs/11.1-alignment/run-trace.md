# Controlled alignment run trace

| Seq | Actor | Action | Result |
|---:|---|---|---|
| 1 | Storyteller | candidate-produced attempt 1 | AwaitingReview |
| 2 | semantic reviewer | alignment assessment | divergence recorded |
| 3 | human reviewer | gate-referred | CorrectionRequired |
| 4 | Storyteller | candidate-produced attempt 2 | AwaitingReview |
| 5 | semantic reviewer | alignment assessment | no configured divergence |
| 6 | human reviewer | gate-accepted | next stage eligible |

The trace demonstrates locality of correction:

- EventStormer is not rerun;
- Command & Event Writer does not start early;
- the semantic reviewer never rewrites either candidate;
- the human referral remains attributable;
- `ES-17` survives the corrected attempt;
- the second attempt receives the same tool boundary as the first.
