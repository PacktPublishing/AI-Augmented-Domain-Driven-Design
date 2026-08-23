---
description: Review a BrewUp BMAD PRD for governed requirements and traceability without changing it.
tools: ["read", "search"]
disable-model-invocation: true
user-invocable: true
---

# BrewUp BMAD PRD Guard

Review the PRD without changing repository state. Do not create, edit, rename,
or delete files, do not run a BMAD workflow, and do not fix the PRD.

## Locate inputs

Read:

- `_bmad-output/project-context.md`
- `.bmad-harness/governance/brewup-sales-order-confirmation.md`
- `.bmad-harness/governance/brewup-module-structure.md`
- `.bmad-harness/governance/gate-contract.md`

If any governance file is missing, return `FAIL`. If only
`_bmad-output/project-context.md` is missing while all governance is available,
return at least `CONCERNS`.

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
and every excluded supplemental path with its reason.

## Checks

Compare every requirement and acceptance criterion with applicable `BC-###`
and `AR-###` rules. Verify that:

- Sales, Payment, and Warehouse authorities remain separate;
- external outcomes are represented by references rather than embedded domain
  models;
- acceptance criteria make the confirmation evidence invariant testable;
- acceptance criteria do not make Sales produce Payment or Warehouse outcomes;
- every governed requirement and acceptance criterion traces to its applicable
  numbered rule, and every in-scope rule traces to a requirement or an explicit
  exclusion;
- unknown policy is identified as an open decision rather than invented;
- scope does not silently add shipment, invoicing, retry, cancellation,
  notification, timeout, refund, void, stock-release, or expiration behavior.

Cite exact `BC-###` evidence for every authority, policy-invention,
acceptance-criteria, or requirement-to-rule traceability finding. Return `FAIL`
for blocking ownership, invariant, acceptance-criteria, or invented-policy
violations; `CONCERNS` for non-blocking ambiguity or incomplete traceability;
otherwise return `PASS`. Return exactly one of PASS, CONCERNS, or FAIL.

## Report

Use all five headings required by `gate-contract.md`, in this order:

1. `Decision`
2. `Scope`
3. `Findings`
4. `Open decisions`
5. `Traceability`

Scope must list exact inspected and missing files. Every finding must include
Severity, Rule ID, Evidence, Consequence, and Required correction. Traceability
must map requirements and acceptance criteria to exact artifact locations and
numbered rules. Remain read-only after reporting.
