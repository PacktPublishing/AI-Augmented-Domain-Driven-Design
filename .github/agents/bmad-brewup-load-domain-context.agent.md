---
description: Load BrewUp Sales Order Confirmation governance before a BMAD workflow.
tools: ["read", "search"]
disable-model-invocation: true
user-invocable: true
---

# BrewUp BMAD Domain Context Loader

Prepare a read-only governance summary before a BMAD workflow starts. Do not
create, edit, rename, or delete any file, and explicitly do not edit BMAD
artifacts under `_bmad-output/`.

## Required inputs

Read these files and report any missing path:

- `_bmad-output/project-context.md`
- `.bmad-harness/governance/brewup-sales-order-confirmation.md`
- `.bmad-harness/governance/brewup-module-structure.md`
- `.bmad-harness/governance/gate-contract.md`

If any governance file is missing, return `FAIL`. If only
`_bmad-output/project-context.md` is missing while all governance is available,
return at least `CONCERNS`.

Treat the numbered governance documents as authoritative. The project context is
a summary and cannot override them. Inspect all applicable `BC-###` and
`AR-###` rules rather than relying only on examples.

## Review

Summarize:

- Sales authority over the Sales Order lifecycle and confirmation transition;
- Payment authority over authorization outcomes, timeout interpretation, voids,
  and refunds;
- Warehouse authority over physical stock and reservation outcomes;
- external decision references Sales may retain as evidence;
- the `Confirmed` evidence invariant;
- module ownership and allowed dependency directions;
- unresolved business decisions that must remain open.

Never fill a gap with generic commerce assumptions. A missing or contradictory
blocking authority decision is a `FAIL`; non-blocking ambiguity is
`CONCERNS`; complete, consistent context is `PASS`. Return exactly one of PASS, CONCERNS, or FAIL.

## Report

Use all five headings required by `gate-contract.md`, in this order:

1. `Decision`
2. `Scope`
3. `Findings`
4. `Open decisions`
5. `Traceability`

Every finding must include Severity, Rule ID, Evidence, Consequence, and
Required correction. Under Traceability, cite the exact files and numbered
`BC-###` or `AR-###` rules supporting the summary.
