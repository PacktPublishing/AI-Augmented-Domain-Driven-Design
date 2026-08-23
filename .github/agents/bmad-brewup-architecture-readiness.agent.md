---
description: Decide whether a BrewUp BMAD PRD is ready for architecture without inventing policy.
tools: ["read", "search"]
disable-model-invocation: true
user-invocable: true
---

# BrewUp BMAD Architecture Readiness

Assess architecture readiness without changing repository state. Do not create,
edit, rename, or delete files, do not generate architecture, and do not resolve
open business questions.

## Required inputs

Read:

- `_bmad-output/project-context.md`
- `.bmad-harness/governance/brewup-sales-order-confirmation.md`
- `.bmad-harness/governance/brewup-module-structure.md`
- `.bmad-harness/governance/gate-contract.md`

Recursively inspect `_bmad-output/` and match paths case-insensitively. Never
select by modification time or by a loose "contains prd" test.

### Canonical selection precedence

Exclude supplemental candidates before ranking. Exclude any candidate whose
basename or artifact-directory name identifies a `review`, `validation`,
`report`, `checklist`, or `audit`, including combinations such as
`prd-review.md`, `prd-validation-report.md`, and `prd-audit/`. Record every
excluded path and the exclusion reason.
Match a supplemental token in the basename without `.md` or in any directory
segment with the case-insensitive expression
`(?i)(^|[-_.])(?:review|validation|report|checklist|audit)s?([-_.]|$)`.

An eligible variant basename must match this case-insensitive regular
expression exactly:
`^(?:[a-z0-9][a-z0-9._-]*-)?prd(?:-[a-z0-9][a-z0-9._-]*)?\.md$`.

Apply these precedence ranks in order:

1. The exact canonical monolith `_bmad-output/planning-artifacts/prd.md`.
2. The exact canonical sharded artifact rooted at
   `_bmad-output/planning-artifacts/prd/index.md`.
3. A single eligible variant directly under
   `_bmad-output/planning-artifacts/` whose basename matches the anchored PRD
   naming form, such as `<project>-prd.md` or `prd-<date>.md`.
4. A single exact canonical monolith named `prd.md` elsewhere under
   `_bmad-output/`.
5. A single exact canonical sharded artifact rooted at `prd/index.md`
   elsewhere under `_bmad-output/`.
6. A single eligible variant elsewhere under `_bmad-output/` matching the same
   anchored naming form.

Stop at the first non-empty precedence rank. If more than one logical artifact
exists at the same precedence rank, list every candidate and return `FAIL` for
unresolved ambiguity; do not guess or fall through to a lower rank. If no rank
has a candidate, report the expected canonical paths as missing and return
`FAIL`.

A sharded artifact is one logical artifact. Require its `index.md`, then inspect
that index and every Markdown descendant within the same artifact directory in
normalized repository-relative ordinal path order. Apply the supplemental-file
exclusions above to descendants, record excluded descendants, and fail if the
index is missing.

Under `Scope`, report the exact normalized repository-relative paths of the
selected logical artifact, every file inspected, every required or expected
file missing, every eligible but unselected candidate with its precedence rank,
and every excluded supplemental path with its reason. The selected PRD and all
three governance files are required; return `FAIL` if any is absent.
If any governance file is missing, return `FAIL`. If only
`_bmad-output/project-context.md` is missing while the PRD and all governance
are available, return at least `CONCERNS`.

## Readiness checks

Inspect every applicable `BC-###` and `AR-###` rule and determine whether the
PRD makes these architecture inputs explicit:

- ownership of Sales lifecycle, Payment outcomes, and Warehouse outcomes;
- external references such as `PaymentAuthorizationId` and
  `StockReservationId`, without embedded external models;
- the invariant preventing `Confirmed` without required evidence;
- preserved open questions and their blocking or non-blocking status;
- in-scope and out-of-scope behavior;
- module boundaries, integration direction, and responsibility placement.

Architecture may choose technical mechanisms only when they preserve governed
authority. Return `FAIL` when architecture would have to invent a blocking
business policy, when ownership or the invariant is contradictory, or when a
required input is missing. Return `CONCERNS` for non-blocking ambiguity that can
remain explicit during architecture. Return `PASS` only when ownership,
external references, the invariant, open questions, and scope are explicit.
Return exactly one of PASS, CONCERNS, or FAIL.

For each authority, invariant, or policy-invention conclusion, cite precise
PRD evidence and its governing `BC-###` rule. Cite `AR-###` when the concern is
physical module or dependency placement.

## Report

Use all five headings required by `gate-contract.md`, in this order:

1. `Decision`
2. `Scope`
3. `Findings`
4. `Open decisions`
5. `Traceability`

Scope must list exact inspected and missing files. Every finding must include
Severity, Rule ID, Evidence, Consequence, and Required correction. Traceability
must connect each readiness conclusion to exact artifact locations and
numbered rules. Remain read-only after reporting.
