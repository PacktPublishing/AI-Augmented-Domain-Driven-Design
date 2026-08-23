---
description: Aggregate BrewUp BMAD implementation readiness across PRD, architecture, epics, and stories.
tools: ["read", "search"]
disable-model-invocation: true
user-invocable: true
---

# BrewUp BMAD Implementation Readiness

Assess readiness without changing repository state. Do not create, edit,
rename, or delete artifacts, run BMAD, or resolve an open decision.

## Required inputs and deterministic selection

Read `_bmad-output/project-context.md`, the three governance files, and
`.bmad-harness/governance/gate-contract.md`. If any governance file is missing, return `FAIL`.
Require a PRD, architecture, and epic/story collection.

Recursively inspect `_bmad-output/`, normalize repository-relative paths,
compare case-insensitively, and never select by modification time. Before
ranking, exclude a path whose basename without `.md` or directory segment
matches
`(?i)(^|[-_.])(?:review|validation|report|checklist|audit)s?([-_.]|$)`.

Recognize only these anchored case-insensitive eligible-variant expressions:

- `^(?:[a-z0-9][a-z0-9._-]*-)?prd(?:-[a-z0-9][a-z0-9._-]*)?\.md$`
- `^(?:[a-z0-9][a-z0-9._-]*-)?(?:architecture-spine|architecture)(?:-[a-z0-9][a-z0-9._-]*)?\.md$`
- `^(?:[a-z0-9][a-z0-9._-]*-)?(?:epics|epics-and-stories)(?:-[a-z0-9][a-z0-9._-]*)?\.md$`

The canonical planning paths are:

- `_bmad-output/planning-artifacts/prd.md`
- `_bmad-output/planning-artifacts/prd/index.md`
- `_bmad-output/planning-artifacts/ARCHITECTURE-SPINE.md`
- `_bmad-output/planning-artifacts/architecture.md`
- `_bmad-output/planning-artifacts/architecture/index.md`
- `_bmad-output/planning-artifacts/epics.md`
- `_bmad-output/planning-artifacts/epics-and-stories.md`
- `_bmad-output/planning-artifacts/epics/index.md`

For each artifact kind use canonical selection precedence: exact canonical
monolith under `_bmad-output/planning-artifacts/`; exact sharded `index.md`
root there; one eligible variant there; exact canonical monolith elsewhere;
exact sharded root elsewhere; then one eligible variant elsewhere.
Architecture prefers `ARCHITECTURE-SPINE.md`, then `architecture.md`;
epic/story collections use `epics.md`, `epics-and-stories.md`, or
`epics/index.md`.

Stop at the first non-empty rank. Multiple logical artifacts at the same
precedence rank require `FAIL` for unresolved ambiguity. A sharded artifact
requires `index.md`; inspect it and every Markdown descendant in normalized
repository-relative ordinal path order with the same exclusions.
Multiple candidates at the same precedence rank require `FAIL`; never select
by modification time.

Under `Scope`, list exact selected, inspected, missing, eligible but unselected,
and excluded paths with ranks and reasons.

## Aggregate checks

Inspect every applicable `BC-###` and `AR-###` across the PRD, architecture,
epics, and stories. Verify cross-artifact consistency for:

- Sales, Payment, and Warehouse authority;
- external evidence references and the confirmation invariant;
- Payment module and `Payment.SharedKernel` contract placement;
- saga coordination without decision ownership;
- host registration, solution membership, dependency rules, and architecture
  fitness tests;
- story ownership, independently verifiable acceptance criteria, explicit
  tests, and preservation of open policy.

Create findings with Severity, Rule ID, Evidence, Consequence, and Required
correction. De-duplicate only exact duplicates by `Rule ID + Evidence`; do not
merge different evidence or hide a higher severity.

Apply exactly:

```text
Decision: PASS | CONCERNS | FAIL
Any CRITICAL finding -> FAIL
No CRITICAL but one or more MAJOR/MINOR findings -> CONCERNS
No findings and all required artifacts present -> PASS
```

A missing required artifact or unresolved same-rank ambiguity is CRITICAL.
Return exactly one of `PASS`, `CONCERNS`, or `FAIL`.

## Report

Use all five headings from `gate-contract.md`, in order:

1. `Decision`
2. `Scope`
3. `Findings`
4. `Open decisions`
5. `Traceability`

`Traceability` must be a cross-artifact map from each governed PRD requirement
through architecture decision, epic, story, acceptance criteria, planned test,
and exact `BC-###` or `AR-###`. Remain read-only after reporting.
