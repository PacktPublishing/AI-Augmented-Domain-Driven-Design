# BrewUp BMAD Copilot Harness

This directory is the operator entry point for the BrewUp governance overlay
used by GitHub Copilot and prepared BMAD customizations.

The repository deliberately ships this overlay without a BMAD installation.
There is no vendored or generated BMAD core, no installer-owned configuration,
and no BMAD runtime command in this harness. Installing and upgrading a
compatible BMAD release is a separate operator action.

## What each directory owns

| Path | Responsibility |
|---|---|
| `.bmad-harness/governance/` | Client-neutral `BC-###` domain rules, `AR-###` architecture rules, and the common gate contract. |
| `.bmad-harness/manifest.json` | Machine-readable inventory of governance, overrides, Copilot agents, required rule IDs, and phase coverage. |
| `.bmad-harness/scripts/` | Dependency-free static validation of the committed overlay and optional Markdown artifacts. |
| `.bmad-harness/tests/` | Structural mutation tests plus representative passing and failing artifacts. |
| `_bmad-output/project-context.md` | Concise BrewUp implementation context. It summarizes but never overrides numbered governance. |
| `_bmad/custom/` | Sparse BMAD role and workflow customizations that a compatible later BMAD installation can merge. |
| `.github/agents/` | Explicit, read-only GitHub Copilot context and guard agents. |

## Governance source relationship

The numbered client-neutral rules are synchronized copies of the original Spec
Kit carriers:

- `.specify/memory/domain-carriers/brewup-sales-order-confirmation.md`
- `.specify/memory/architecture/brewup-module-structure.md`

The validator compares the complete ordered `BC-000` through `BC-011` and
`AR-000` through `AR-018` sets and whitespace-normalized numbered-rule content.
It reports every differing rule ID. A rule change must update both the original
carrier and its client-neutral copy in `.bmad-harness/governance/`.

This synchronization check is a repository maintenance guard, not a runtime
dependency. BMAD project context, customizations, and Copilot agents continue to
load only `.bmad-harness/governance/`; none of the `_bmad/custom/` overrides
references `.specify/`.

The harness does not put generated planning or implementation artifacts under
versioned governance. A BMAD installation normally creates those artifacts
under `_bmad-output/`; the guard selected for a review locates the relevant
current artifact deterministically.

## Official BMAD workflow names

The manifest and sparse overrides use these current workflow skill names
exactly:

1. `bmad-product-brief`
2. `bmad-prd`
3. `bmad-architecture`
4. `bmad-create-epics-and-stories`
5. `bmad-check-implementation-readiness`
6. `bmad-create-story`
7. `bmad-dev-story`
8. `bmad-code-review`

The role-wide overrides use `bmad-agent-analyst`, `bmad-agent-pm`,
`bmad-agent-architect`, and `bmad-agent-dev`.

## Copilot guard order

Use the explicit Copilot agents around the matching BMAD activity:

| Order | When | Copilot agent |
|---:|---|---|
| 1 | Before Analysis | `bmad-brewup-load-domain-context` |
| 2 | After `bmad-product-brief` | `bmad-brewup-product-brief-guard` |
| 3 | After `bmad-prd` | `bmad-brewup-prd-guard` |
| 4 | Before architecture | `bmad-brewup-architecture-readiness` |
| 5 | After `bmad-architecture` | `bmad-brewup-architecture-guard` |
| 6 | After `bmad-create-epics-and-stories` | `bmad-brewup-epics-stories-guard` |
| 7 | At the Solutioning readiness gate | `bmad-brewup-implementation-readiness` |
| 8 | Before `bmad-dev-story` | `bmad-brewup-story-guard` |
| 9 | During or after `bmad-code-review` | `bmad-brewup-code-review-guard` |

All guards are read-only. Their frontmatter explicitly sets
`disable-model-invocation: true` and `user-invocable: true`, so they are
manual-only and remain available for deliberate operator selection. They return
`PASS`, `CONCERNS`, or `FAIL` and use the report structure in
`governance/gate-contract.md`.

## Validate the overlay

From the repository root, run:

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1
```

The first command exercises the real validator against the repository, a
passing artifact set, semantic failure fixtures, and isolated structural
mutations. Expected nested `FAIL [...]` lines are evidence from those negative
fixtures; the test runner itself must end with `BMAD harness tests passed.` and
exit `0`.

The second command performs repository-only static validation and must exit
`0`. To lint a prepared Markdown artifact directory as well, pass
`-ArtifactRoot <path>`.

Static success proves the committed manifest, full governance synchronization,
strict supported sparse TOML syntax, strict supported Copilot frontmatter,
phase registration, manual-only agent contracts, and named fixture rules. The
dependency-free TOML grammar consumes each entire override and permits one
table, canonical sparse assignments, supported single-line quoted strings, and
multiline string arrays. The frontmatter grammar consumes exactly description,
read/search tools, and both manual-invocation Boolean fields; trailing,
unknown, duplicate, or malformed syntax fails.

Optional artifact linting uses bounded paragraph evidence and extracted
filesystem paths. It distinguishes `SalesOrder.Confirm` requiring external IDs
from Sales performing authorization/reservation, and it requires all six
standard `src/Payment/BrewUp.Payment.*` project paths when Payment behavior is
declared implemented. Static validation still does not prove a native BMAD
merge, a Copilot run, or workflow execution because BMAD is not installed.

## What activates after BMAD is installed

After a compatible BMAD installation, only the committed files under
`_bmad/custom/` become active as BMAD role and workflow customizations. The
installation supplies and owns its core runtime and configuration. The sparse
customizations then instruct the applicable BMAD roles and workflows to load
`_bmad-output/project-context.md` and the governance files.

The `.bmad-harness/` files remain governance and validation inputs. The
`.github/agents/` files remain explicit GitHub Copilot agents; installing BMAD
does not auto-run them.

Revalidate BMAD's customization schema and merge behavior against the installed
version before relying on native execution. Do not copy installed core files
into this repository to make the static validator pass.

## Updating governance safely

The meanings of existing `BC-###` and `AR-###` identifiers are durable
contracts. Do not silently renumber or redefine a rule.

When governance changes:

1. record the domain or architecture decision explicitly;
2. update the applicable original `.specify/memory/` carrier and the matching
   client-neutral `.bmad-harness/governance/` copy together;
3. add a new rule ID when the meaning changes, or make an intentional,
   reviewed edit when only wording is clarified;
4. update every affected manifest entry, project-context summary, override,
   guard, fixture, and traceability reference without weakening the original
   boundary accidentally;
5. run both validation commands above and inspect the differing-ID diagnostics
   and diff before committing.

The detailed rationale and replay guide is in
[`BrewUpDocs/brewup-bmad-harness.md`](../BrewUpDocs/brewup-bmad-harness.md).
