# BrewUp BMAD Harness for GitHub Copilot

## Purpose

Create a standalone governance overlay that applies the stronger BrewUp Spec Kit
harness to BMAD's native lifecycle without installing or vendoring BMAD.

The first delivery targets GitHub Copilot. A later branch will adapt the same
client-neutral governance model to Codex.

The harness continues to use the Sales Order Confirmation example:

- Sales owns the Sales Order lifecycle and confirmation decision.
- Payment owns payment authorization outcomes.
- Warehouse owns stock availability and reservation outcomes.
- Cross-context coordination must not transfer decision authority.
- A business authority implemented in BrewUp must have the required physical
  module structure.

## Goals

1. Preserve the existing numbered bounded-context rules (`BC-###`).
2. Preserve the existing numbered architecture rules (`AR-###`).
3. Apply those rules to BMAD-native artifacts rather than copying Spec Kit's
   phase names.
4. Provide continuous context injection through BMAD customization overrides.
5. Provide explicit GitHub Copilot agents for repeatable quality gates.
6. Produce BMAD-native `PASS`, `CONCERNS`, or `FAIL` findings.
7. Remain useful and statically verifiable before BMAD is installed.
8. Avoid modifying application source code or installing BMAD dependencies.

## Non-goals

- Installing BMAD or copying BMAD's generated core files into the repository.
- Replacing BMAD workflows, agents, templates, or installers.
- Implementing the Sales Order Confirmation feature.
- Resolving domain questions that the current carrier intentionally leaves open.
- Creating the Codex integration in this branch.
- Building a general-purpose harness for unrelated BrewUp features.

## Recommended approach

Use a hybrid overlay:

1. BMAD-compatible customization files under `_bmad/custom/` inject BrewUp
   governance into native agents and workflows when BMAD is later installed.
2. GitHub Copilot custom agents under `.github/agents/` expose the same checks
   explicitly, making the harness demonstrable without BMAD.
3. Shared governance documents hold domain and architecture facts so the two
   integration surfaces do not duplicate policy.
4. A deterministic validation script checks the overlay's structure,
   references, rule coverage, and configuration shape without executing BMAD.

An agents-only overlay would work immediately but would not participate in
native BMAD workflow activation. An overrides-only overlay would integrate well
after installation but could not be exercised clearly beforehand. The hybrid
keeps both benefits.

## Repository layout

The intended layout is:

```text
_bmad/
  custom/
    bmad-agent-analyst.toml
    bmad-agent-pm.toml
    bmad-agent-architect.toml
    bmad-agent-dev.toml
    bmad-product-brief.toml
    bmad-prd.toml
    bmad-create-architecture.toml
    bmad-create-epics-and-stories.toml
    bmad-check-implementation-readiness.toml
    bmad-create-story.toml
    bmad-dev-story.toml
    bmad-code-review.toml

_bmad-output/
  project-context.md

.bmad-harness/
  README.md
  governance/
    brewup-sales-order-confirmation.md
    brewup-module-structure.md
    gate-contract.md
  scripts/
    validate-harness.ps1

.github/
  agents/
    bmad-brewup-load-domain-context.agent.md
    bmad-brewup-product-brief-guard.agent.md
    bmad-brewup-prd-guard.agent.md
    bmad-brewup-architecture-readiness.agent.md
    bmad-brewup-architecture-guard.agent.md
    bmad-brewup-epics-stories-guard.agent.md
    bmad-brewup-implementation-readiness.agent.md
    bmad-brewup-story-guard.agent.md
    bmad-brewup-code-review-guard.agent.md

BrewUpDocs/
  brewup-bmad-harness.md
```

Exact filenames may be consolidated during planning if BMAD's current
customization surface makes a smaller set sufficient. The boundaries and gate
coverage must remain unchanged.

## Sources of truth

The existing Spec Kit files contain valuable rules, but the BMAD overlay must
not depend operationally on `.specify/`. This prevents a BMAD workflow from
silently requiring Spec Kit to remain installed.

The implementation will therefore establish client-neutral governance files
under `.bmad-harness/governance/`. Their content will be derived from:

- `.specify/memory/domain-carriers/brewup-sales-order-confirmation.md`
- `.specify/memory/architecture/brewup-module-structure.md`
- `.specify/memory/constitution.md`
- `.github/copilot-instructions.md`

The BMAD project context, customization overrides, and Copilot guards will all
reference the client-neutral copies. The documentation will state how these
relate to the original Spec Kit harness and how rule changes must be kept in
sync until a future consolidation is explicitly chosen.

## Native BMAD phase mapping

### Phase 1: Analysis

Artifacts include the product brief and optional research.

Controls:

- Load the Sales Order Confirmation domain carrier.
- Require explicit Sales, Payment, and Warehouse language.
- Detect generic e-commerce assumptions that collapse decision authority.
- Keep unresolved business policies visible.

Gate output:

- Product brief guard with `BC-###` findings.

### Phase 2: Planning

The primary artifact is `prd.md`.

Controls:

- Validate requirements against bounded-context ownership.
- Ensure acceptance criteria describe observable outcomes without assigning
  Payment or Warehouse behavior to Sales.
- Reject silent invention of timeout, retry, compensation, refund, reservation,
  or partial-availability policy.
- Check traceability from requirements to relevant `BC-###` rules.

Gate output:

- PRD guard with `PASS`, `CONCERNS`, or `FAIL`.

### Phase 3: Solutioning

Artifacts include `architecture.md`, epics, and stories.

Before architecture:

- Confirm the PRD is sufficiently explicit to make technical decisions without
  inventing domain policy.

After architecture:

- Validate both decision ownership (`BC-###`) and physical structure
  (`AR-###`).
- Require a Payment module if Payment implementation is in scope.
- Validate message ownership, project placement, allowed dependencies, host
  registration, orchestration boundaries, and architecture tests.

After epics and stories:

- Ensure every implementation item preserves authority and module structure.
- Require traceability to PRD requirements and applicable governance rules.
- Prevent stories from turning open questions into hidden implementation work.

Implementation readiness:

- Aggregate findings across the PRD, architecture, epics, and stories.
- Return a single BMAD-native readiness decision.
- A critical ownership or module-structure violation produces `FAIL`.

### Phase 4: Implementation

Before development:

- Validate each detailed story independently.
- Require explicit acceptance criteria, affected modules, contracts, tests,
  dependencies, and rule references.

During development:

- Inject project context into the developer workflow.
- Preserve test-first and architecture-fitness requirements.

At code review:

- Compare code and tests with the story, PRD, architecture, `BC-###`, and
  `AR-###`.
- Report file-specific violations.
- Do not modify code; the guard is a review gate.

## Gate contract

Every explicit guard follows one response contract:

```text
Decision: PASS | CONCERNS | FAIL

Scope:
- artifacts inspected
- artifacts missing

Findings:
- Severity
- Rule ID
- Evidence
- Consequence
- Required correction

Open decisions:
- unresolved policy or architecture choices

Traceability:
- requirement/story -> BC rule -> AR rule -> expected module/test
```

Decision rules:

- `PASS`: no material violations and all required inputs are present.
- `CONCERNS`: no critical violation, but ambiguity, missing traceability, or a
  non-blocking gap remains.
- `FAIL`: the artifact transfers authority, collapses a bounded context,
  invents necessary business policy, contradicts upstream artifacts, or omits
  required module structure.

Guards must cite evidence rather than reporting that a design merely "feels
wrong."

## BMAD customization strategy

The override files will use BMAD's update-safe customization surface rather
than editing generated workflow or agent files.

Agent overrides provide persistent facts appropriate to each role:

- Analyst: ubiquitous language, authority boundaries, open-question discipline.
- Product Manager: requirement ownership and policy-invention constraints.
- Architect: `BC-###`, `AR-###`, module structure, dependency boundaries.
- Developer: story adherence, test-first implementation, file placement.

Workflow overrides add phase-specific activation steps and load only the
governance needed for that workflow. Overrides must not assume that BMAD files
exist during repository validation.

Because BMAD is absent, the static validator checks the override files but does
not claim that BMAD has merged or executed them.

## GitHub Copilot integration

Each custom agent is an explicit, read-only workflow or guard.

The agents will:

1. locate BMAD output using the conventional `_bmad-output/` layout;
2. load the client-neutral governance sources;
3. identify the relevant phase artifacts;
4. apply the gate contract;
5. report missing inputs transparently;
6. avoid generating or editing upstream BMAD artifacts unless its specific
   purpose is context preparation.

Agent descriptions and handoffs will make the expected order discoverable in
Copilot. They will not rely on Spec Kit hook dispatch.

## Context and data flow

```text
Client-neutral governance
  -> BMAD project context
  -> role/workflow customization overrides
  -> BMAD planning and implementation artifacts
  -> explicit Copilot guards
  -> PASS / CONCERNS / FAIL findings
  -> corrected upstream artifact
```

Corrections flow back to the earliest artifact that introduced the violation.
For example, a story-level ownership failure caused by ambiguous PRD language
must not be patched only in the story.

## Error handling

- Missing BMAD installation: expected; static validation continues.
- Missing optional artifact: report it and evaluate only when the gate contract
  permits.
- Missing required artifact: produce `CONCERNS` or `FAIL` according to whether
  a safe evaluation is possible.
- Malformed TOML or broken path reference: deterministic validation failure.
- Unknown `BC-###` or `AR-###` reference: deterministic validation failure.
- Conflicting governance copies: deterministic validation failure with the
  differing rule IDs identified.
- Unrelated dirty working-tree files: preserve them and exclude them from
  harness commits.

## Verification

The harness will be verified without BMAD by checking:

1. every declared override is valid TOML;
2. every referenced governance file exists;
3. all Copilot agent frontmatter is present and parseable;
4. all required BMAD phases have an explicit gate;
5. all referenced `BC-###` and `AR-###` IDs exist;
6. gate outputs require the common decision contract;
7. no overlay file assumes an installed `_bmad` core;
8. no application source file is changed by the harness;
9. documentation and actual filenames agree.

Manual fixture-based checks will exercise at least:

- a compliant external-reference design;
- Payment behavior incorrectly embedded in Sales;
- Payment recognized conceptually but missing as a physical module;
- unresolved payment or reservation policy silently converted into a story.

## Documentation

`BrewUpDocs/brewup-bmad-harness.md` will explain:

- why the BMAD harness exists;
- how BMAD phases differ from Spec Kit phases;
- the gate sequence;
- the hybrid override/agent design;
- operation before and after BMAD installation;
- expected demo failures;
- validation commands;
- the boundary for the later Codex branch.

## Branch and change isolation

The GitHub Copilot harness is implemented on the current `bmad-harness` branch.
Only harness and documentation files are staged and committed. Existing
unrelated `.csproj` modifications remain untouched.

The Codex adaptation will be created later from an agreed baseline on a
separate branch. It will reuse `.bmad-harness/governance/` and replace only the
client integration surface where possible.

## Success criteria

The design is complete when:

- the repository contains a BMAD-compatible overlay but no BMAD installation;
- GitHub Copilot can run every lifecycle gate explicitly;
- future BMAD installation can consume the customization overrides;
- the Sales Order Confirmation example yields precise `BC-###` and `AR-###`
  findings at the correct BMAD phases;
- readiness and review decisions use `PASS`, `CONCERNS`, or `FAIL`;
- validation succeeds independently of BMAD;
- unrelated working-tree changes remain untouched;
- the later Codex port does not require redesigning shared governance.
