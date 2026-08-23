# Storyteller 9.3 — Human Decision

**Status:** complete — 11 of 11 primary candidates decided; correction 01 complete.

**Review basis:** `raw-output.md`, SHA-256
`51ad6561fb24af5e0319ffcdef8db3ad9090a95a24c46de77af1f94891fc01c5`.

| Candidate | Name | Decision | Reason |
|---|---|---|---|
| ST-01 | twelve cases of pale ale can be had | accepted | Concrete supported sequence; it preserves the unresolved relationship between physical set-aside and the board entry. |
| ST-02 | Andrea refuses a short order | accepted | Evidence-backed variation, not a selected policy. |
| ST-03 | Nadia sets aside the available part of a short order | accepted | Evidence-backed conflicting variation; the next decision remains visible. |
| ST-04 | pale ale is genuinely out until Thursday | referred back | The name made expected replenishment sound like a proved end date for the shortage. |
| ST-05 | product code is not a beer BrewUp makes | accepted | Separates the observed cause from its unresolved correction owner. |
| ST-06 | Thursday retry depends on somebody remembering | accepted | Preserves competing retry practices and absence of an owner. |
| ST-07 | Nadia clears an unattended hold on Friday | accepted | Shows an observed practice without treating its timing as policy. |
| ST-08 | Sales calls off a held order | accepted | Supported sequence retains Sales authority over cancellation. |
| ST-09 | held order turns out not to have been paid | accepted | Stops before inventing payment communication or a warehouse response. |
| ST-10 | Sales sends a held order through to shipping | referred back | The name implied a direct Sales-to-Shipping instruction not present in the evidence. |
| ST-11 | four stout holds are affected by a failed batch | accepted | Preserves the single observed occurrence and leaves its handling unresolved. |

## Domain-authority notes

- Reviewer: human evaluator acting as the authority over the synthetic fixture and its evaluation baseline
- Date: 2026-08-19
- General constraints: accept concrete, evidence-backed scenarios that preserve
  uncertainty; do not permit a name to assert an arrival, end state, or
  communication route not established by the evidence.

## Correction 01 outcome

The primary decisions above remain the immutable record of the first human gate.
ST-04 and ST-10 were corrected in `correction-01/raw-output.md` and accepted
after review because their revised names and uncertainty fields now preserve the
evidence boundary.

| Candidate | Effective corrected name | Final decision |
|---|---|---|
| ST-04 | pale ale is out; more is expected Thursday | accepted after correction |
| ST-10 | Sales decides a held order goes through | accepted after correction |

**Effective accepted set:** 11 candidates — nine unchanged primary records plus
the two accepted corrected versions. Ten scenario statuses remain unresolved.
