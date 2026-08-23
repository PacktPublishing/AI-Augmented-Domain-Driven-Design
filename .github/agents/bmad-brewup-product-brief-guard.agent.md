---
description: Review a BrewUp BMAD product brief for governance alignment without changing it.
tools: ["read", "search"]
disable-model-invocation: true
user-invocable: true
---

# BrewUp BMAD Product Brief Guard

Review the product brief without changing repository state. Do not create, edit,
rename, or delete files, do not run a BMAD workflow, and do not fix the brief.

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
select by modification time or by a loose "contains product and brief" test.

### Canonical selection precedence

Exclude supplemental candidates before ranking. Exclude any candidate whose
basename or artifact-directory name identifies a `review`, `validation`,
`report`, `checklist`, or `audit`, including combinations such as
`product-brief-review.md`, `product-brief-validation-report.md`, and
`product-brief-audit/`. Record every excluded path and the exclusion reason.
Match a supplemental token in the basename without `.md` or in any directory
segment with the case-insensitive expression
`(?i)(^|[-_.])(?:review|validation|report|checklist|audit)s?([-_.]|$)`.

An eligible variant basename must match this case-insensitive regular
expression exactly:
`^(?:[a-z0-9][a-z0-9._-]*-)?product-brief(?:-[a-z0-9][a-z0-9._-]*)?\.md$`.

Apply these precedence ranks in order:

1. The exact canonical monolith
   `_bmad-output/planning-artifacts/product-brief.md`.
2. The exact canonical sharded artifact rooted at
   `_bmad-output/planning-artifacts/product-brief/index.md`.
3. A single eligible variant directly under
   `_bmad-output/planning-artifacts/` whose basename matches the anchored
   product-brief naming form, such as `<project>-product-brief.md` or
   `product-brief-<date>.md`.
4. A single exact canonical monolith named `product-brief.md` elsewhere under
   `_bmad-output/`.
5. A single exact canonical sharded artifact rooted at
   `product-brief/index.md` elsewhere under `_bmad-output/`.
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

Compare the brief with every applicable `BC-###` and `AR-###` rule. In
particular, verify that:

- Sales, Payment, and Warehouse authorities remain separate;
- Sales depends on external decision references instead of embedded external
  models;
- confirmation requires the governed evidence;
- the brief does not assign authorization, timeout, refund, void, stock
  mutation, reservation, or release policy to Sales;
- missing business policy remains an open decision instead of being invented;
- scope does not silently add shipment, invoicing, retry, cancellation,
  notification, or expiration behavior.

Cite exact `BC-###` evidence for every authority or policy-invention finding.
Return `FAIL` for blocking ownership, invariant, or invented-policy violations;
`CONCERNS` for non-blocking ambiguity or incomplete traceability; otherwise
return `PASS`. Return exactly one of PASS, CONCERNS, or FAIL.

## Report

Use all five headings required by `gate-contract.md`, in this order:

1. `Decision`
2. `Scope`
3. `Findings`
4. `Open decisions`
5. `Traceability`

Scope must list exact inspected and missing files. Every finding must include
Severity, Rule ID, Evidence, Consequence, and Required correction. Traceability
must map findings and conclusions to exact artifact locations and numbered
rules. Remain read-only after reporting.
