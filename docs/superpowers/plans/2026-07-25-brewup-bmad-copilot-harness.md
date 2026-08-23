# BrewUp BMAD Copilot Harness Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a standalone GitHub Copilot overlay that enforces BrewUp's Sales Order Confirmation domain and architecture rules across BMAD's native lifecycle without installing BMAD.

**Architecture:** Client-neutral governance lives under `.bmad-harness/governance/` and is consumed by sparse, update-safe `_bmad/custom/*.toml` overrides plus explicit GitHub Copilot guard agents. A dependency-free PowerShell validator and fixture suite verify configuration, references, rule coverage, phase coverage, and representative pass/fail cases without requiring BMAD.

**Tech Stack:** Markdown, GitHub Copilot custom agents, TOML BMAD customization overrides, PowerShell 7+, built-in `ConvertFrom-Json`, .NET 10 repository conventions

## Global Constraints

- Do not install, vendor, initialize, or execute BMAD.
- Use current BMAD skill names: `bmad-product-brief`, `bmad-prd`, `bmad-architecture`, `bmad-create-epics-and-stories`, `bmad-check-implementation-readiness`, `bmad-create-story`, `bmad-dev-story`, and `bmad-code-review`.
- Store committed team overrides only under `_bmad/custom/{skill-name}.toml`.
- Keep overrides sparse; do not copy an installed skill's full `customize.toml`.
- Agent customizations use `[agent]`; workflow customizations use `[workflow]`.
- File references in TOML use `file:{project-root}/...`.
- Preserve the existing `BC-###` and `AR-###` meanings.
- A critical authority, policy-invention, or required module-structure violation returns `FAIL`.
- Explicit guards are read-only and report `PASS`, `CONCERNS`, or `FAIL`.
- Do not modify application source code.
- Do not stage or commit the unrelated existing `.csproj` changes.
- Target GitHub Copilot only in this branch; the Codex integration is deferred.

---

## File map

### Shared governance

- `.bmad-harness/governance/brewup-sales-order-confirmation.md`: client-neutral `BC-###` domain carrier.
- `.bmad-harness/governance/brewup-module-structure.md`: client-neutral `AR-###` architecture memory.
- `.bmad-harness/governance/gate-contract.md`: common decision, severity, evidence, and traceability contract.
- `.bmad-harness/manifest.json`: machine-readable list of governance, overrides, agents, phases, and required rule IDs.

### BMAD overlay

- `_bmad-output/project-context.md`: concise BrewUp implementation context for BMAD agents.
- `_bmad/custom/bmad-agent-{analyst,pm,architect,dev}.toml`: role-wide persistent facts.
- `_bmad/custom/bmad-{product-brief,prd,architecture,create-epics-and-stories,check-implementation-readiness,create-story,dev-story,code-review}.toml`: phase-specific workflow activation steps and facts.

### GitHub Copilot controls

- `.github/agents/bmad-brewup-load-domain-context.agent.md`: context bootstrap.
- `.github/agents/bmad-brewup-product-brief-guard.agent.md`: Analysis gate.
- `.github/agents/bmad-brewup-prd-guard.agent.md`: Planning gate.
- `.github/agents/bmad-brewup-architecture-readiness.agent.md`: pre-Solutioning gate.
- `.github/agents/bmad-brewup-architecture-guard.agent.md`: architecture domain and structure gate.
- `.github/agents/bmad-brewup-epics-stories-guard.agent.md`: epic/story decomposition gate.
- `.github/agents/bmad-brewup-implementation-readiness.agent.md`: aggregate BMAD readiness gate.
- `.github/agents/bmad-brewup-story-guard.agent.md`: detailed story gate.
- `.github/agents/bmad-brewup-code-review-guard.agent.md`: source/test review gate.
- `.github/copilot-instructions.md`: short BMAD overlay discovery section.

### Validation and documentation

- `.bmad-harness/scripts/validate-harness.ps1`: deterministic structural and semantic validator.
- `.bmad-harness/tests/validate-harness.tests.ps1`: dependency-free test runner.
- `.bmad-harness/tests/fixtures/{pass,fail-*}/`: representative BMAD artifacts.
- `.bmad-harness/README.md`: overlay operation and validation entry point.
- `BrewUpDocs/brewup-bmad-harness.md`: detailed rationale and demo guide.

---

### Task 1: Establish client-neutral governance and manifest

**Files:**
- Create: `.bmad-harness/governance/brewup-sales-order-confirmation.md`
- Create: `.bmad-harness/governance/brewup-module-structure.md`
- Create: `.bmad-harness/governance/gate-contract.md`
- Create: `.bmad-harness/manifest.json`
- Test: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: `.specify/memory/domain-carriers/brewup-sales-order-confirmation.md`, `.specify/memory/architecture/brewup-module-structure.md`
- Produces: manifest properties `schemaVersion`, `governance`, `requiredRuleIds`, `overrides`, `agents`, and `phaseCoverage`; decision contract headings `Decision`, `Scope`, `Findings`, `Open decisions`, and `Traceability`

- [ ] **Step 1: Write the failing manifest and governance test**

Create `.bmad-harness/tests/validate-harness.tests.ps1` with a minimal assertion runner:

```powershell
[CmdletBinding()]
param([string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path)

$ErrorActionPreference = 'Stop'
$failures = [System.Collections.Generic.List[string]]::new()

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { $script:failures.Add($Message) }
}

$manifestPath = Join-Path $RepositoryRoot '.bmad-harness\manifest.json'
Assert-True (Test-Path -LiteralPath $manifestPath) 'manifest.json must exist'

if (Test-Path -LiteralPath $manifestPath) {
    $manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
    Assert-True ($manifest.schemaVersion -eq 1) 'schemaVersion must be 1'
    Assert-True ($manifest.governance.Count -eq 3) 'three governance documents are required'
    Assert-True ($manifest.requiredRuleIds.bc.Count -gt 0) 'BC rule IDs are required'
    Assert-True ($manifest.requiredRuleIds.ar.Count -gt 0) 'AR rule IDs are required'
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output 'BMAD harness tests passed.'
```

- [ ] **Step 2: Run the test and verify it fails**

Run:

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
```

Expected: exit code `1` with `manifest.json must exist`.

- [ ] **Step 3: Create the governance documents**

Copy the complete numbered domain and architecture rules into client-neutral
documents. Remove Spec Kit command instructions, but retain:

```markdown
# BrewUp Sales Order Confirmation Governance

## Authority

- Sales owns the Sales Order lifecycle.
- Payment owns payment authorization outcomes.
- Warehouse owns stock availability and reservation outcomes.

## Numbered rules

### BC-000 — Domain rules are authoritative
...
```

and:

```markdown
# BrewUp Module Structure Governance

## Numbered rules

### AR-000 — Architecture rules are authoritative
...
```

Create `.bmad-harness/governance/gate-contract.md` with the exact output schema:

```markdown
# BrewUp BMAD Gate Contract

## Decision

Return exactly one of `PASS`, `CONCERNS`, or `FAIL`.

## Finding fields

Every finding contains Severity, Rule ID, Evidence, Consequence, and Required correction.

## Required report headings

1. Decision
2. Scope
3. Findings
4. Open decisions
5. Traceability
```

- [ ] **Step 4: Create the machine-readable manifest**

Create `.bmad-harness/manifest.json` with exact relative paths and phase mapping:

```json
{
  "schemaVersion": 1,
  "governance": [
    ".bmad-harness/governance/brewup-sales-order-confirmation.md",
    ".bmad-harness/governance/brewup-module-structure.md",
    ".bmad-harness/governance/gate-contract.md"
  ],
  "requiredRuleIds": {
    "bc": ["BC-000", "BC-003", "BC-007", "BC-009"],
    "ar": ["AR-000", "AR-001", "AR-002", "AR-003", "AR-008", "AR-010", "AR-016"]
  },
  "overrides": [],
  "agents": [],
  "phaseCoverage": {
    "analysis": [],
    "planning": [],
    "solutioning": [],
    "implementation": []
  }
}
```

The complete arrays will be extended by later tasks.

- [ ] **Step 5: Run the test and verify it passes**

Run:

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
```

Expected: `BMAD harness tests passed.`

- [ ] **Step 6: Commit the governance foundation**

```powershell
git add -- .bmad-harness/governance .bmad-harness/manifest.json .bmad-harness/tests/validate-harness.tests.ps1
git commit -m "feat: add BMAD governance foundation"
```

### Task 2: Add BMAD project context and role overrides

**Files:**
- Create: `_bmad-output/project-context.md`
- Create: `_bmad/custom/bmad-agent-analyst.toml`
- Create: `_bmad/custom/bmad-agent-pm.toml`
- Create: `_bmad/custom/bmad-agent-architect.toml`
- Create: `_bmad/custom/bmad-agent-dev.toml`
- Modify: `.bmad-harness/manifest.json`
- Modify: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: the three governance files from Task 1
- Produces: four sparse `[agent]` overrides with `persistent_facts`; BMAD implementation context at `_bmad-output/project-context.md`

- [ ] **Step 1: Extend the test for role overrides**

Add assertions that each expected file exists, begins with `[agent]`, contains
`persistent_facts`, and references governance through a project-root path:

```powershell
$roleOverrides = @(
    '_bmad/custom/bmad-agent-analyst.toml',
    '_bmad/custom/bmad-agent-pm.toml',
    '_bmad/custom/bmad-agent-architect.toml',
    '_bmad/custom/bmad-agent-dev.toml'
)

foreach ($relativePath in $roleOverrides) {
    $path = Join-Path $RepositoryRoot $relativePath
    Assert-True (Test-Path -LiteralPath $path) "$relativePath must exist"
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True ($text -match '(?m)^\[agent\]$') "$relativePath must use [agent]"
        Assert-True ($text -match 'persistent_facts\s*=') "$relativePath must define persistent_facts"
        Assert-True ($text -match 'file:\{project-root\}/\.bmad-harness/governance/') "$relativePath must load governance"
    }
}
```

- [ ] **Step 2: Run the test and verify it fails**

Expected: failures naming all four missing role override files.

- [ ] **Step 3: Create the BMAD project context**

Create `_bmad-output/project-context.md` with:

- .NET 10, C#, Muflone, CQRS, Event Sourcing, RabbitMQ, EventStore, MongoDB;
- DDD modular monolith constraints;
- standard module project structure;
- allowed dependency directions;
- test-first `CommandSpecification<T>` discipline;
- `Guid.CreateVersion7()` and `ConfigureAwait(false)`;
- links to all three client-neutral governance documents;
- explicit statement that `project-context.md` summarizes but does not override numbered rules.

- [ ] **Step 4: Create sparse agent overrides**

Use this exact shape for each role, varying only the role-specific literal:

```toml
[agent]
activation_steps_prepend = [
  "Load {project-root}/_bmad-output/project-context.md before reasoning about BrewUp artifacts."
]
persistent_facts = [
  "file:{project-root}/.bmad-harness/governance/brewup-sales-order-confirmation.md",
  "file:{project-root}/.bmad-harness/governance/brewup-module-structure.md",
  "file:{project-root}/.bmad-harness/governance/gate-contract.md",
  "Do not silently invent missing business policy."
]
```

Role-specific final facts:

- Analyst: preserve ubiquitous language and open questions.
- PM: express ownership in requirements and acceptance criteria.
- Architect: map business authority to explicit module structure.
- Developer: implement only approved story decisions and preserve module boundaries.

- [ ] **Step 5: Register the four files in the manifest**

Set `overrides` entries to objects with:

```json
{
  "path": "_bmad/custom/bmad-agent-analyst.toml",
  "kind": "agent",
  "skill": "bmad-agent-analyst"
}
```

Repeat for PM, Architect, and Developer.

- [ ] **Step 6: Run the test and commit**

Run the test script; expect success. Then:

```powershell
git add -- _bmad-output/project-context.md _bmad/custom .bmad-harness/manifest.json .bmad-harness/tests/validate-harness.tests.ps1
git commit -m "feat: inject BrewUp context into BMAD roles"
```

### Task 3: Add native BMAD workflow overrides

**Files:**
- Create: `_bmad/custom/bmad-product-brief.toml`
- Create: `_bmad/custom/bmad-prd.toml`
- Create: `_bmad/custom/bmad-architecture.toml`
- Create: `_bmad/custom/bmad-create-epics-and-stories.toml`
- Create: `_bmad/custom/bmad-check-implementation-readiness.toml`
- Create: `_bmad/custom/bmad-create-story.toml`
- Create: `_bmad/custom/bmad-dev-story.toml`
- Create: `_bmad/custom/bmad-code-review.toml`
- Modify: `.bmad-harness/manifest.json`
- Modify: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: shared governance and gate contract
- Produces: eight sparse `[workflow]` overrides that inject phase-appropriate facts and completion instructions

- [ ] **Step 1: Extend tests for workflow override schema**

Add expected paths and assert `[workflow]`, `persistent_facts`, and
`activation_steps_prepend`:

```powershell
$workflowSkills = @(
    'bmad-product-brief',
    'bmad-prd',
    'bmad-architecture',
    'bmad-create-epics-and-stories',
    'bmad-check-implementation-readiness',
    'bmad-create-story',
    'bmad-dev-story',
    'bmad-code-review'
)

foreach ($skill in $workflowSkills) {
    $relativePath = "_bmad/custom/$skill.toml"
    $path = Join-Path $RepositoryRoot $relativePath
    Assert-True (Test-Path -LiteralPath $path) "$relativePath must exist"
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True ($text -match '(?m)^\[workflow\]$') "$relativePath must use [workflow]"
        Assert-True ($text -match 'activation_steps_prepend\s*=') "$relativePath must prepend activation"
        Assert-True ($text -match 'persistent_facts\s*=') "$relativePath must define persistent facts"
    }
}
```

- [ ] **Step 2: Run tests and verify missing-file failures**

Run the test runner. Expected: eight missing workflow override failures.

- [ ] **Step 3: Create phase-specific workflow overrides**

Each file uses:

```toml
[workflow]
activation_steps_prepend = [
  "Load {project-root}/_bmad-output/project-context.md.",
  "Load the BrewUp governance files before reading or writing this workflow's artifact."
]
persistent_facts = [
  "file:{project-root}/.bmad-harness/governance/brewup-sales-order-confirmation.md",
  "file:{project-root}/.bmad-harness/governance/brewup-module-structure.md",
  "file:{project-root}/.bmad-harness/governance/gate-contract.md"
]
on_complete = "Report applicable BC-### and AR-### traceability and do not claim readiness when a critical rule is violated."
```

Set workflow-specific completion requirements:

- Product brief: identify authorities and unresolved policies.
- PRD: validate requirements and acceptance criteria against `BC-###`.
- Architecture: validate `BC-###`, `AR-###`, module placement, dependencies, and tests.
- Epics/stories: preserve traceability and do not convert open decisions into work.
- Implementation readiness: emit exactly `PASS`, `CONCERNS`, or `FAIL`.
- Create story: require affected modules, rule IDs, acceptance criteria, and tests.
- Dev story: implement only approved decisions and stop on unresolved blocking policy.
- Code review: cite file evidence and return changes requested for critical violations.

- [ ] **Step 4: Register workflow overrides and phase coverage**

Append manifest entries with `kind: "workflow"` and populate:

```json
"phaseCoverage": {
  "analysis": ["bmad-product-brief"],
  "planning": ["bmad-prd"],
  "solutioning": [
    "bmad-architecture",
    "bmad-create-epics-and-stories",
    "bmad-check-implementation-readiness"
  ],
  "implementation": ["bmad-create-story", "bmad-dev-story", "bmad-code-review"]
}
```

- [ ] **Step 5: Run tests and commit**

Expect `BMAD harness tests passed.` Then:

```powershell
git add -- _bmad/custom .bmad-harness/manifest.json .bmad-harness/tests/validate-harness.tests.ps1
git commit -m "feat: add BrewUp BMAD workflow overrides"
```

### Task 4: Add Analysis and Planning Copilot agents

**Files:**
- Create: `.github/agents/bmad-brewup-load-domain-context.agent.md`
- Create: `.github/agents/bmad-brewup-product-brief-guard.agent.md`
- Create: `.github/agents/bmad-brewup-prd-guard.agent.md`
- Create: `.github/agents/bmad-brewup-architecture-readiness.agent.md`
- Modify: `.bmad-harness/manifest.json`
- Modify: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: `_bmad-output/project-context.md`, governance files, BMAD artifacts under `_bmad-output/`
- Produces: four read-only Copilot agents using the common gate contract

- [ ] **Step 1: Extend tests for Copilot agent frontmatter and contract**

Add a helper:

```powershell
function Test-CopilotAgent {
    param([string]$RelativePath)
    $path = Join-Path $RepositoryRoot $RelativePath
    Assert-True (Test-Path -LiteralPath $path) "$RelativePath must exist"
    if (-not (Test-Path -LiteralPath $path)) { return }

    $text = Get-Content -Raw -LiteralPath $path
    Assert-True ($text -match '\A---\r?\n') "$RelativePath must have frontmatter"
    Assert-True ($text -match '(?m)^description:\s*.+$') "$RelativePath needs description"
    Assert-True ($text -match 'PASS.*CONCERNS.*FAIL') "$RelativePath must use the gate decisions"
    Assert-True ($text -match 'BC-###') "$RelativePath must inspect BC rules"
    Assert-True ($text -match 'gate-contract\.md') "$RelativePath must load the gate contract"
}
```

Invoke it for the four agent paths.

- [ ] **Step 2: Run tests and verify the four agents are missing**

Expected: one missing-file failure for each agent.

- [ ] **Step 3: Create the context-loader agent**

Use frontmatter:

```yaml
---
description: Load BrewUp Sales Order Confirmation governance before a BMAD workflow.
---
```

The body must load the project context and three governance files, summarize
authorities, list open decisions, and explicitly avoid editing BMAD artifacts.

- [ ] **Step 4: Create product-brief and PRD guards**

Both agents must:

1. locate conventional BMAD planning artifacts case-insensitively;
2. report the exact files inspected and missing;
3. check authority and policy invention;
4. cite `BC-###` evidence;
5. use all five gate-contract headings;
6. remain read-only.

The PRD guard additionally checks acceptance criteria and requirement-to-rule
traceability.

- [ ] **Step 5: Create architecture-readiness agent**

Require the PRD plus governance. Return:

- `FAIL` if architecture would need to invent a blocking business policy;
- `CONCERNS` for non-blocking ambiguity;
- `PASS` when ownership, external references, invariant, open questions, and
  scope are explicit.

- [ ] **Step 6: Register agents, run tests, and commit**

Register each manifest agent as:

```json
{
  "path": ".github/agents/bmad-brewup-prd-guard.agent.md",
  "gate": "planning.prd",
  "readOnly": true
}
```

Run tests and commit:

```powershell
git add -- .github/agents/bmad-brewup-*.agent.md .bmad-harness/manifest.json .bmad-harness/tests/validate-harness.tests.ps1
git commit -m "feat: add BMAD analysis and planning guards"
```

### Task 5: Add Solutioning and Implementation Copilot agents

**Files:**
- Create: `.github/agents/bmad-brewup-architecture-guard.agent.md`
- Create: `.github/agents/bmad-brewup-epics-stories-guard.agent.md`
- Create: `.github/agents/bmad-brewup-implementation-readiness.agent.md`
- Create: `.github/agents/bmad-brewup-story-guard.agent.md`
- Create: `.github/agents/bmad-brewup-code-review-guard.agent.md`
- Modify: `.bmad-harness/manifest.json`
- Modify: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: PRD, `ARCHITECTURE-SPINE.md` or compatible `architecture.md`, epic/story artifacts, source/test changes
- Produces: five explicit Copilot gates spanning Solutioning through code review

- [ ] **Step 1: Add the five agents to the frontmatter/contract test**

Call `Test-CopilotAgent` for all five new files. Add assertions that:

- architecture and readiness agents contain `AR-###`;
- implementation readiness contains `Decision: PASS | CONCERNS | FAIL`;
- code review requires file-path evidence.

- [ ] **Step 2: Run tests and verify missing-file failures**

Expected: five new missing-file failures.

- [ ] **Step 3: Create architecture and epic/story guards**

The architecture guard checks:

- authority is unchanged from the PRD;
- Payment is a full module when implemented;
- Payment contracts are in `Payment.SharedKernel`;
- Sales stores external references rather than embedded models;
- Sagas coordinate without owning decisions;
- host registration, solution membership, dependency rules, and architecture
  tests are planned.

The epic/story guard checks each story for:

- upstream requirement;
- applicable `BC-###` and `AR-###`;
- affected module and exact file ownership;
- independently verifiable acceptance criteria;
- explicit tests;
- no hidden resolution of open policy.

- [ ] **Step 4: Create aggregate readiness agent**

Inspect PRD, architecture, epics, and stories. De-duplicate findings by
`Rule ID + Evidence`. Apply:

```text
Any CRITICAL finding -> FAIL
No CRITICAL but one or more MAJOR/MINOR findings -> CONCERNS
No findings and all required artifacts present -> PASS
```

The report must include cross-artifact traceability.

- [ ] **Step 5: Create story and code-review guards**

Story guard:

- validates one detailed story before development;
- fails if required business policy is absent upstream;
- fails if paths place Payment/Warehouse behavior in Sales;
- requires a test-first sequence and architecture fitness coverage.

Code-review guard:

- reads the approved story and changed files;
- reports precise file paths and line numbers when available;
- checks behavior, contracts, dependencies, module registration, and tests;
- never edits code;
- requests changes for critical `BC-###` or `AR-###` violations.

- [ ] **Step 6: Register agents, run tests, and commit**

Update the manifest, run the test runner, and commit:

```powershell
git add -- .github/agents/bmad-brewup-*.agent.md .bmad-harness/manifest.json .bmad-harness/tests/validate-harness.tests.ps1
git commit -m "feat: add BMAD solutioning and implementation guards"
```

### Task 6: Implement deterministic harness validation and fixtures

**Files:**
- Create: `.bmad-harness/scripts/validate-harness.ps1`
- Create: `.bmad-harness/tests/fixtures/pass/prd.md`
- Create: `.bmad-harness/tests/fixtures/pass/ARCHITECTURE-SPINE.md`
- Create: `.bmad-harness/tests/fixtures/pass/story-order-confirmation.md`
- Create: `.bmad-harness/tests/fixtures/fail-authority/prd.md`
- Create: `.bmad-harness/tests/fixtures/fail-module/ARCHITECTURE-SPINE.md`
- Create: `.bmad-harness/tests/fixtures/fail-policy/story-order-confirmation.md`
- Modify: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: `-RepositoryRoot <path>` and optional `-ArtifactRoot <path>`
- Produces: process exit `0` on structural success, `1` on validation failure; output records formatted `PASS|CONCERNS|FAIL [check-id] message`

- [ ] **Step 1: Write failing validator invocation tests**

Add:

```powershell
$validator = Join-Path $RepositoryRoot '.bmad-harness\scripts\validate-harness.ps1'
Assert-True (Test-Path -LiteralPath $validator) 'validator script must exist'

if (Test-Path -LiteralPath $validator) {
    & $validator -RepositoryRoot $RepositoryRoot
    Assert-True ($LASTEXITCODE -eq 0) 'repository harness validation must pass'

    & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\pass')
    Assert-True ($LASTEXITCODE -eq 0) 'pass fixture must pass'

    & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\fail-authority')
    Assert-True ($LASTEXITCODE -eq 1) 'authority fixture must fail'
}
```

- [ ] **Step 2: Run tests and verify the validator is missing**

Expected: `validator script must exist`.

- [ ] **Step 3: Create semantic fixtures**

The passing fixture must state:

```text
Sales stores PaymentAuthorizationId and StockReservationId as external evidence.
Payment authorizes payment in the Payment module.
Warehouse reserves stock in the Warehouse module.
```

Failure fixtures must contain these explicit violations:

```text
SalesOrder.Confirm authorizes payment and reserves inventory.
```

```text
Payment authorization is implemented under src/Sales/BrewUp.Sales.Domain/Payment.
```

```text
On provider timeout, retry exactly three times and then mark payment declined.
```

- [ ] **Step 4: Implement the structural validator**

Implement functions:

```powershell
function Add-Finding {
    param(
        [ValidateSet('PASS', 'CONCERNS', 'FAIL')][string]$Decision,
        [string]$CheckId,
        [string]$Message
    )
}

function Resolve-HarnessPath {
    param([string]$RepositoryRoot, [string]$RelativePath)
}

function Get-RuleIds {
    param([string[]]$GovernancePaths, [string]$Prefix)
}

function Test-ManifestReferences {
    param([pscustomobject]$Manifest, [string]$RepositoryRoot)
}

function Test-OverrideShape {
    param([pscustomobject]$Manifest, [string]$RepositoryRoot)
}

function Test-AgentContracts {
    param([pscustomobject]$Manifest, [string]$RepositoryRoot)
}
```

Checks must cover:

- JSON parsing;
- referenced files;
- required rule IDs;
- agent/workflow TOML table type;
- `{project-root}` references;
- required phase coverage;
- Copilot frontmatter and common decisions;
- no `_bmad` installed-core paths such as `_bmad/bmm/` or `_bmad/scripts/`.

- [ ] **Step 5: Implement optional artifact linting**

When `-ArtifactRoot` is present, scan Markdown text for the three fixture
violations using named patterns:

```powershell
$artifactRules = @(
    @{
        Id = 'BC-003/BC-007'
        Pattern = 'SalesOrder\.Confirm.*(authoriz|reserv)'
        Message = 'Sales must not authorize payment or reserve stock.'
    },
    @{
        Id = 'AR-001/AR-016'
        Pattern = 'src[/\\]Sales[/\\].*Payment'
        Message = 'Payment implementation must not be collapsed into Sales.'
    },
    @{
        Id = 'BC-000'
        Pattern = 'retry exactly three times.*mark payment declined'
        Message = 'The story invents provider timeout policy.'
    }
)
```

Matching any pattern produces `FAIL` and exit `1`.

- [ ] **Step 6: Run all tests and commit**

Run:

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1
```

Expected: both exit `0`; the nested negative-fixture checks are reported as
expected failures by the test runner.

Commit:

```powershell
git add -- .bmad-harness/scripts .bmad-harness/tests
git commit -m "test: validate BMAD harness without installation"
```

### Task 7: Document and expose the Copilot BMAD harness

**Files:**
- Create: `.bmad-harness/README.md`
- Create: `BrewUpDocs/brewup-bmad-harness.md`
- Modify: `.github/copilot-instructions.md`
- Modify: `README.md`
- Modify: `.bmad-harness/tests/validate-harness.tests.ps1`

**Interfaces:**
- Consumes: final manifest, validator, overrides, agents, and governance
- Produces: discoverable user workflow and exact validation command

- [ ] **Step 1: Add documentation-link tests**

Assert:

```powershell
$documentationPaths = @(
    '.bmad-harness/README.md',
    'BrewUpDocs/brewup-bmad-harness.md'
)

foreach ($relativePath in $documentationPaths) {
    $path = Join-Path $RepositoryRoot $relativePath
    Assert-True (Test-Path -LiteralPath $path) "$relativePath must exist"
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True ($text -match 'validate-harness\.ps1') "$relativePath must document validation"
        Assert-True ($text -match 'without.*BMAD|BMAD.*not.*installed') "$relativePath must state the installation boundary"
    }
}
```

- [ ] **Step 2: Run tests and verify documentation is missing**

Expected: failures for both documentation files.

- [ ] **Step 3: Write the operator README**

`.bmad-harness/README.md` must document:

1. no BMAD installation is included;
2. directory responsibilities;
3. official BMAD skill names used;
4. Copilot guard order;
5. validation command;
6. what becomes active after a BMAD installation;
7. how to update governance without changing rule meanings silently.

- [ ] **Step 4: Write the detailed harness guide**

`BrewUpDocs/brewup-bmad-harness.md` must parallel the existing Spec Kit guide
and cover:

- domain ownership versus physical architecture;
- BMAD phase mapping;
- every override and Copilot agent;
- readiness decision semantics;
- expected demo failures;
- static validation;
- why BMAD is deliberately absent;
- later Codex branch boundary.

- [ ] **Step 5: Add repository discovery links**

Add a concise `## BMAD Governance Overlay` section to
`.github/copilot-instructions.md` pointing to:

- `_bmad-output/project-context.md`;
- `.bmad-harness/governance/`;
- the current BMAD artifact being reviewed;
- the explicit guard agents.

Add a BMAD harness link beside the existing harness documentation in
`README.md`. Do not alter the managed `<!-- SPECKIT START -->` block.

- [ ] **Step 6: Run tests, inspect diff, and commit**

Run:

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1
git diff --check
git status --short
```

Expected:

- both validators exit `0`;
- no whitespace errors;
- only harness, documentation, Copilot instruction, and README files are staged;
- the unrelated `.csproj` changes remain unstaged.

Commit:

```powershell
git add -- .bmad-harness/README.md BrewUpDocs/brewup-bmad-harness.md .github/copilot-instructions.md README.md .bmad-harness/tests/validate-harness.tests.ps1
git commit -m "docs: explain BrewUp BMAD Copilot harness"
```

### Task 8: Final verification and handoff

**Files:**
- Verify only; no planned file changes

**Interfaces:**
- Consumes: complete harness
- Produces: evidence that the overlay is complete, BMAD-free, and isolated from application changes

- [ ] **Step 1: Run the full harness test suite**

```powershell
pwsh -NoProfile -File .bmad-harness/tests/validate-harness.tests.ps1
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1
```

Expected: both commands exit `0`.

- [ ] **Step 2: Verify representative negative fixtures**

```powershell
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1 -ArtifactRoot .bmad-harness/tests/fixtures/fail-authority
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1 -ArtifactRoot .bmad-harness/tests/fixtures/fail-module
pwsh -NoProfile -File .bmad-harness/scripts/validate-harness.ps1 -ArtifactRoot .bmad-harness/tests/fixtures/fail-policy
```

Expected: each command exits `1` and cites the matching `BC-###` or `AR-###`.

- [ ] **Step 3: Verify BMAD was not installed**

Run:

```powershell
Get-ChildItem _bmad -Force
```

Expected: only `custom/` is present; there is no `_bmad/bmm/`, `_bmad/core/`,
`_bmad/scripts/`, or installer-owned `_bmad/config.toml`.

- [ ] **Step 4: Verify change isolation**

Run:

```powershell
git status --short
git diff --name-only 4ffd096..HEAD
```

Expected: the original `.csproj` modifications remain uncommitted and no
application source file appears in harness commits.

- [ ] **Step 5: Review against the approved design**

Confirm every success criterion in
`docs/superpowers/specs/2026-07-25-brewup-bmad-copilot-harness-design.md`
maps to a manifest entry, validation check, or documentation section.

- [ ] **Step 6: Prepare the Codex-port handoff**

Report:

- shared files that should be reused unchanged;
- Copilot-only files under `.github/agents/`;
- recommended branch name `codex/bmad-codex-harness`;
- any BMAD schema assumptions that should be revalidated when the Codex branch
  begins.

Do not create the Codex branch in this task.
