# BrewUp BMAD Copilot Harness

> Detailed explanation of the BMAD governance overlay built for the demo
> **Spec-Driven Development: Redefining the Software Architect in the AI Era**

This guide explains how BrewUp's Sales Order Confirmation decisions are carried
across BMAD artifacts and reviewed by explicit GitHub Copilot agents.

The repository intentionally provides the overlay without a BMAD installation.
It contains governance, sparse future customizations, read-only Copilot agents,
and static validation, but no BMAD core, installer configuration, or executable
BMAD workflow. That boundary keeps the demo inspectable without claiming native
BMAD behavior that has not run.

---

## 1. Why this harness exists

The feature sounds simple:

```text
Confirm a Sales Order after payment authorization and stock availability,
then reserve the stock.
```

A fluent generated artifact can still assign all three decisions to
`SalesOrder.Confirm()`. It may compile and remain wrong because Sales does not
own payment authorization or physical stock.

The harness makes those ownership decisions durable and gives every BMAD phase
the same review vocabulary:

```text
BC-###  domain authority and business-policy rules
AR-###  physical modules, projects, references, and wiring rules
```

Guards cite named rules and evidence instead of relying on an untestable
statement that an artifact "feels wrong."

---

## 2. Domain ownership is not physical architecture

The domain carrier assigns decision authority:

```text
Sales      owns the Sales Order lifecycle and confirmation transition.
Payment    owns authorization outcomes, including provider interpretation.
Warehouse  owns physical stock and reservation outcomes.
```

Sales may retain `PaymentAuthorizationId` and `StockReservationId` as evidence.
Those references do not let Sales load, embed, or mutate Payment and Warehouse
models. Depending on another authority's decision is not owning that decision.

The architecture carrier maps authority into repository structure. If Payment
is implemented here, it is a full BrewUp module with SharedKernel, Domain,
ReadModel, Infrastructure, Facade, and Tests projects. Cross-context Payment
contracts belong in `BrewUp.Payment.SharedKernel`; a saga may coordinate but
cannot take ownership of authorization, reservation, or confirmation.

This separation matters:

| Governance | Question answered | Example failure |
|---|---|---|
| `BC-###` | Who may decide or change business state? | Sales authorizes payment or invents timeout policy. |
| `AR-###` | Where does approved responsibility live physically? | Payment behavior is placed under `src/Sales/`. |

Changing folders cannot repair wrong authority, and correct authority still
needs an architecture that preserves it.

---

## 3. Harness structure

The overlay has four cooperating layers:

| Layer | Files | Purpose |
|---|---|---|
| Shared governance | `.bmad-harness/governance/*.md` | Client-neutral domain rules, architecture rules, and gate contract. |
| BMAD context and customizations | `_bmad-output/project-context.md`, `_bmad/custom/*.toml` | Summarize BrewUp conventions and prepare sparse facts for a compatible future BMAD installation. |
| Copilot review | `.github/agents/bmad-brewup-*.agent.md` | Locate and inspect the current BMAD artifact without editing it. |
| Static evidence | `.bmad-harness/manifest.json`, scripts, tests, fixtures | Verify registrations, shapes, references, absence of installed core, and representative rule failures. |

`_bmad-output/project-context.md` is intentionally concise. It summarizes the
technology and implementation conventions, but the numbered governance files
remain authoritative whenever wording conflicts.

### Original Spec Kit carriers and client-neutral copies

The original governance sources remain:

- `.specify/memory/domain-carriers/brewup-sales-order-confirmation.md`
- `.specify/memory/architecture/brewup-module-structure.md`

Their complete numbered rules are mirrored in
`.bmad-harness/governance/brewup-sales-order-confirmation.md` and
`.bmad-harness/governance/brewup-module-structure.md`. Static validation
compares the ordered `BC-000` through `BC-011` and `AR-000` through `AR-018`
sets plus whitespace-normalized content for every numbered rule. Missing,
additional, reordered, or changed rules fail with the differing IDs named.

The direction of runtime dependency is intentionally one-way:

```text
Spec Kit originals --maintenance synchronization--> client-neutral governance
                                               |
                                               +--> BMAD overrides and Copilot
```

BMAD context, `_bmad/custom/*.toml`, and Copilot guards reference only the
client-neutral files. The validator reads the originals solely to prevent
maintenance drift; installing or executing BMAD never requires `.specify/`.

---

## 4. BMAD phase mapping

The manifest registers the official workflow names in this order:

| BMAD phase | Workflow override | Review point |
|---|---|---|
| Analysis | `bmad-product-brief` | Load context, then review the product brief. |
| Planning | `bmad-prd` | Review governed requirements, acceptance criteria, and traceability. |
| Solutioning | `bmad-architecture` | Check readiness first, then review authority and physical architecture. |
| Solutioning | `bmad-create-epics-and-stories` | Review decomposition, ownership, tests, and unresolved policy. |
| Solutioning | `bmad-check-implementation-readiness` | Aggregate PRD, architecture, epics, and stories. |
| Implementation | `bmad-create-story` | Review one detailed story before development. |
| Implementation | `bmad-dev-story` | Implement only approved story decisions. |
| Implementation | `bmad-code-review` | Review source and tests against the approved story and governance. |

These are the current names used by the committed manifest:
`bmad-product-brief`, `bmad-prd`, `bmad-architecture`,
`bmad-create-epics-and-stories`, `bmad-check-implementation-readiness`,
`bmad-create-story`, `bmad-dev-story`, and `bmad-code-review`.

---

## 5. Every BMAD override

All overrides are sparse TOML under `_bmad/custom/`. They do not copy an
installed skill's full customization file.

### Role-wide overrides

- `bmad-agent-analyst.toml` loads governance and preserves ubiquitous language
  and open questions.
- `bmad-agent-pm.toml` loads governance and expresses ownership in requirements
  and acceptance criteria.
- `bmad-agent-architect.toml` loads governance and maps business authority to
  explicit module structure.
- `bmad-agent-dev.toml` loads governance and limits implementation to approved
  story decisions while preserving module boundaries.

Their official role skill names are `bmad-agent-analyst`, `bmad-agent-pm`,
`bmad-agent-architect`, and `bmad-agent-dev`.

### Workflow overrides

- `bmad-product-brief.toml` requires authorities and unresolved policies to be
  identified.
- `bmad-prd.toml` requires requirements and acceptance criteria to be checked
  against `BC-###`.
- `bmad-architecture.toml` requires `BC-###` and `AR-###` traceability, module
  placement, dependencies, and tests.
- `bmad-create-epics-and-stories.toml` preserves traceability and prevents open
  decisions from becoming implementation work.
- `bmad-check-implementation-readiness.toml` requires exactly `PASS`,
  `CONCERNS`, or `FAIL`.
- `bmad-create-story.toml` requires affected modules, rule IDs, acceptance
  criteria, and tests.
- `bmad-dev-story.toml` implements only approved decisions and stops on
  unresolved blocking policy.
- `bmad-code-review.toml` requires file evidence and changes requested for
  critical violations.

Every workflow also loads the project context and all three governance files,
reports applicable `BC-###` and `AR-###` traceability, and refuses to claim
readiness while a critical rule is violated.

Because BMAD is not installed, these files are committed customization inputs,
not evidence of a native merge. After a compatible installation, only
`_bmad/custom/` becomes active as BMAD customization; the installed release
must supply and own its core runtime and configuration.

---

## 6. Every GitHub Copilot agent

The Copilot agents are explicit, manual-only, and read-only. Every frontmatter
block uses only read and search tools and sets
`disable-model-invocation: true` plus `user-invocable: true`. The model cannot
select these guards automatically, while an operator can invoke the intended
gate deliberately. They never edit reviewed artifacts and all report with the
gate contract.

1. `bmad-brewup-load-domain-context` loads the project context and governance,
   then summarizes Sales, Payment, Warehouse, evidence, and open decisions.
2. `bmad-brewup-product-brief-guard` selects the current product brief
   deterministically and checks authority, invariant, scope, and invented
   policy.
3. `bmad-brewup-prd-guard` selects the current PRD and also checks acceptance
   criteria plus requirement-to-rule traceability.
4. `bmad-brewup-architecture-readiness` decides whether architecture can
   proceed without inventing a blocking business policy.
5. `bmad-brewup-architecture-guard` checks authority preservation, Payment
   module structure, contract placement, saga boundaries, dependencies, host
   registration, solution membership, and architecture tests.
6. `bmad-brewup-epics-stories-guard` checks upstream requirements, rule IDs,
   module/file ownership, acceptance criteria, tests, and unresolved policies
   for every epic and story.
7. `bmad-brewup-implementation-readiness` aggregates the PRD, architecture,
   epics, and stories and produces cross-artifact traceability.
8. `bmad-brewup-story-guard` validates one detailed story, including upstream
   policy, exact paths, test-first work, fitness tests, and stop conditions.
9. `bmad-brewup-code-review-guard` compares the approved story with changed
   source and tests, citing paths and line numbers when available and requesting
   changes for critical rule violations.

Planning guards use deterministic artifact selection. They prefer canonical
artifacts under `_bmad-output/planning-artifacts/`, support sharded artifacts,
exclude review/report/checklist/audit supplements, and fail rather than guess
when equal-precedence candidates are ambiguous. The story and code-review
guards likewise require an explicit or unambiguous current artifact.

---

## 7. Recommended Copilot guard sequence

Use the context loader before starting Analysis. Then pair each created artifact
with its explicit review:

```text
load domain context
  -> bmad-product-brief
  -> product brief guard
  -> bmad-prd
  -> PRD guard
  -> architecture readiness
  -> bmad-architecture
  -> architecture guard
  -> bmad-create-epics-and-stories
  -> epics/stories guard
  -> implementation readiness
  -> bmad-create-story
  -> story guard
  -> bmad-dev-story
  -> bmad-code-review
  -> code review guard
```

The overlay does not auto-run Copilot agents. The operator selects the relevant
agent and supplies or confirms the artifact being reviewed.

---

## 8. Readiness decisions

Every guard returns exactly one decision and the headings `Decision`, `Scope`,
`Findings`, `Open decisions`, and `Traceability`.

| Decision | Meaning |
|---|---|
| `PASS` | Required artifacts are present and there are no findings for the inspected scope. |
| `CONCERNS` | No critical violation exists, but one or more non-blocking ambiguities, gaps, or incomplete traceability items remain. |
| `FAIL` | A critical authority, invented-policy, invariant, required artifact, ambiguity, or mandatory module-structure problem blocks the next step. |

The aggregate implementation-readiness agent applies the precise threshold:
any CRITICAL finding means `FAIL`; MAJOR or MINOR findings without a CRITICAL
mean `CONCERNS`; no findings with all required artifacts means `PASS`.
Duplicate findings are removed only when both Rule ID and Evidence match.

Each finding includes Severity, Rule ID, Evidence, Consequence, and Required
correction. A `PASS` is not a proof that generated output is deterministic; it
is a scoped statement about the files actually inspected.

---

## 9. Expected demo failures

The fixture suite makes four characteristic failures visible:

| Fixture | Deliberate defect | Expected finding |
|---|---|---|
| `fail-authority/prd.md` | `SalesOrder.Confirm` authorizes payment and reserves inventory. | `FAIL [BC-003/BC-007]` because Payment and Warehouse own those outcomes. |
| `fail-module/ARCHITECTURE-SPINE.md` | Payment authorization is implemented under `src/Sales/`. | `FAIL [AR-001/AR-016]` because a separate authority is collapsed into Sales. |
| `fail-missing-payment-module/ARCHITECTURE-SPINE.md` | Payment owns implemented authorization, but no physical Payment module/projects are defined. | `FAIL [AR-001/AR-002/AR-016]` because conceptual ownership lacks required physical structure. |
| `fail-policy/story-order-confirmation.md` | A story invents exactly three retries and maps timeout to declined. | `FAIL [BC-000]` because missing provider policy must remain open. |

The passing fixtures instead keep Payment and Warehouse behavior in their own
modules and let Sales store only external decision evidence. The realistic
fixture deliberately states that `SalesOrder.Confirm` requires
`PaymentAuthorizationId` and `StockReservationId`, mentions separate Sales and
Payment paths, and enumerates the six standard Payment project paths. Those
statements must pass rather than becoming regex false positives.

---

## 10. Static validation

Run both commands from the repository root:

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1
```

The test runner invokes the real validator against:

- the committed repository;
- a passing artifact set;
- the three semantic failure fixtures;
- isolated structural mutations for JSON, TOML, registration, and installed
  core boundaries.

The nested semantic and structural checks intentionally print `FAIL [...]`.
The parent test suite verifies each failure's exit code and rule/check ID, then
ends with `BMAD harness tests passed.` and exit `0`.

The standalone validator verifies manifest types and references, complete
Spec Kit/client-neutral numbered-rule synchronization, exact phase coverage,
sparse override facts, Copilot contracts, and that no installed BMAD core or
core reference is present. Optional artifact linting is available with:

```powershell
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1 -ArtifactRoot <path>
```

The validator dependency-free parses a deliberately small, strict subset. Each
TOML override must be fully consumed as one bare table with canonical sparse
fields, supported quoted strings, and multiline string arrays; trailing,
unknown, malformed, or duplicate syntax fails. Each Copilot frontmatter block
must be fully consumed as exactly a non-empty description, the read/search
tools array, `disable-model-invocation: true`, and `user-invocable: true`.

Artifact linting evaluates bounded Markdown paragraphs and extracted path
tokens instead of applying `Singleline` patterns across an entire file.
Evidence includes deterministic path and line information. If Payment behavior
is stated as implemented, physical evidence must include `src/Payment/` and
SharedKernel, Domain, ReadModel, Infrastructure, Facade, and Tests projects.

This remains deliberately static validation. It does not parse every TOML or
YAML feature outside the documented subset, execute a Copilot agent, install
BMAD, or prove native BMAD customization merge behavior.

---

## 11. Why BMAD is deliberately absent

The repository is a governance demo, not a vendored BMAD distribution.
Committing only sparse `_bmad/custom/` files:

- keeps project decisions separate from tool-owned core files;
- avoids freezing generated installer output into the demo;
- allows the harness and negative examples to be validated offline;
- makes the installation boundary honest and reviewable;
- leaves BMAD version and upgrade ownership with the operator.

No `_bmad/bmm/`, `_bmad/core/`, `_bmad/scripts/`, or installer-owned
`_bmad/config.toml` belongs in this overlay. A later installation must be
validated against its own supported schema; the committed harness must not
pretend that static checks equal runtime evidence.

---

## 12. Governance maintenance

The existing `BC-###` and `AR-###` meanings are stable traceability contracts.
Do not silently redefine or renumber them to accommodate a generated artifact.

When a domain or architecture decision changes, record the decision and update
the original `.specify/memory/` carrier plus its client-neutral
`.bmad-harness/governance/` copy together. Introduce a new rule ID when
semantics change, update every affected summary, override, guard, fixture, and
manifest reference, then run both validation commands. Clarifying wording is
acceptable only when it preserves the original boundary, remains synchronized,
and receives normal review. BMAD runtime inputs continue to reference only the
client-neutral copies.

---

## 13. Later Codex branch boundary

This branch targets GitHub Copilot. The files under `.github/agents/` are
Copilot-specific and should not be treated as a Codex integration.

A later Codex port should be developed on the recommended branch
`codex/bmad-codex-harness`. It should reuse the client-neutral governance,
manifest concepts, project context, fixtures, and validator where their
contracts remain valid, while replacing the Copilot agent layer with
Codex-appropriate controls.

Before that branch relies on BMAD integration, revalidate the installed BMAD
version's role/workflow skill names, TOML customization schema, merge order,
activation behavior, and artifact layout. Do not create the Codex branch or
install BMAD as part of this Copilot documentation task.

---

## 14. What this harness demonstrates

The harness does not make an LLM deterministic and does not replace domain
expert review. It makes decisions persistent enough to inspect across product
brief, PRD, architecture, epics, stories, implementation, and review.

The practical outcome is:

```text
not deterministic output
but durable authority, explicit uncertainty, and verifiable drift
```

For the short operator checklist, see
[`../.bmad-harness/README.md`](../.bmad-harness/README.md).
