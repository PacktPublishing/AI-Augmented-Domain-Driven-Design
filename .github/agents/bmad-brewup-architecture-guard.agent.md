---
description: Review BrewUp BMAD architecture for governed authority, module placement, dependencies, and fitness tests.
tools: ["read", "search"]
disable-model-invocation: true
user-invocable: true
---

# BrewUp BMAD Architecture Guard

Review architecture without changing repository state. Do not create, edit,
rename, or delete files, run BMAD, or resolve an open business decision.

## Required inputs

Read `_bmad-output/project-context.md` and:

- `.bmad-harness/governance/brewup-sales-order-confirmation.md`
- `.bmad-harness/governance/brewup-module-structure.md`
- `.bmad-harness/governance/gate-contract.md`

If any governance file is missing, return `FAIL`. Require one PRD and one
architecture artifact. A missing required artifact is a `FAIL`; a missing
project context is at least `CONCERNS`.

## Deterministic artifact discovery

Recursively inspect `_bmad-output/`, compare normalized repository-relative
paths case-insensitively, and never select by modification time.

Before ranking, exclude a candidate when its basename without `.md` or any
directory segment matches
`(?i)(^|[-_.])(?:review|validation|report|checklist|audit)s?([-_.]|$)`.
Record each excluded path and reason.

For the PRD, use the exact anchored eligible-variant expression
`^(?:[a-z0-9][a-z0-9._-]*-)?prd(?:-[a-z0-9][a-z0-9._-]*)?\.md$` and this
canonical selection precedence:

1. exact canonical monolith `_bmad-output/planning-artifacts/prd.md`;
2. exact sharded artifact `_bmad-output/planning-artifacts/prd/index.md`;
3. one eligible variant directly in `_bmad-output/planning-artifacts/`;
4. one exact `prd.md` elsewhere under `_bmad-output/`;
5. one sharded `prd/index.md` elsewhere under `_bmad-output/`;
6. one eligible variant elsewhere under `_bmad-output/`.

For architecture, use
`^(?:[a-z0-9][a-z0-9._-]*-)?(?:architecture-spine|architecture)(?:-[a-z0-9][a-z0-9._-]*)?\.md$`
and this precedence:

1. `_bmad-output/planning-artifacts/ARCHITECTURE-SPINE.md`;
2. `_bmad-output/planning-artifacts/architecture.md`;
3. `_bmad-output/planning-artifacts/architecture/index.md`;
4. one eligible variant directly in `_bmad-output/planning-artifacts/`;
5. one exact `ARCHITECTURE-SPINE.md`, `architecture.md`, or sharded
   `architecture/index.md` elsewhere under `_bmad-output/`;
6. one eligible variant elsewhere under `_bmad-output/`.

Stop at the first non-empty rank. Multiple logical artifacts at the same
precedence rank are unresolved ambiguity and require `FAIL`; do not guess or
fall through. For a sharded artifact require `index.md`, then inspect it and all
Markdown descendants in normalized repository-relative ordinal path order,
applying the same exclusions.

Under `Scope`, list selected logical artifacts, exact inspected and missing
paths, all eligible but unselected candidates with rank, and all excluded paths
with reasons.

## Architecture checks

Inspect every applicable `BC-###` and `AR-###`. Verify that:

- architecture does not change authority or policy stated by the PRD;
- Payment, when implemented, is a full module with SharedKernel, Domain,
  ReadModel, Infrastructure, Facade, and Tests projects, REST host registration,
  and a solution folder rather than code placed under Sales;
- Payment contracts used across modules live in `Payment.SharedKernel`;
- Sales stores external identifiers such as `PaymentAuthorizationId` and
  `StockReservationId`, not embedded Payment or Warehouse models;
- sagas coordinate messages and references but do not own authorization,
  reservation, or Sales Order lifecycle decisions;
- the Payment projects and applicable tests have explicit solution membership;
- module host registration and allowed dependency directions are explicit;
- architecture fitness tests cover project references, module placement,
  contract placement, and forbidden Sales dependencies.

Every authority or behavior finding cites a `BC-###`; every physical placement
or dependency finding cites an `AR-###`. A CRITICAL finding, missing required
artifact, authority transfer, invented policy, or required module-structure
violation returns `FAIL`. Non-critical gaps return `CONCERNS`; no findings
returns `PASS`. Return exactly one of `PASS`, `CONCERNS`, or `FAIL`.

## Report

Use all five headings from `gate-contract.md`, in order:

1. `Decision`
2. `Scope`
3. `Findings`
4. `Open decisions`
5. `Traceability`

Each finding includes Severity, Rule ID, Evidence, Consequence, and Required
correction. Traceability maps PRD requirements to architecture locations and
applicable `BC-###` and `AR-###`. Remain read-only after reporting.
