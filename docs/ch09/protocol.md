# Chapter 9 — Specialist Agent Experiment Protocol

This file freezes the experimental protocol before any specialist agent is
written or executed.

It is committed first, and it is not edited after the first run. If the
protocol turns out to be wrong, the correction is a new commit with a stated
reason, not a silent rewrite.

---

## 1. What the experiment claims, and what it does not

Four specialized agents are asked to transform raw domain evidence into
candidate modeling material for BrewUp Stock Management.

The experiment asks one question:

> When an agent is allowed to propose modeling material rather than only to
> inspect it, does its output stay distinguishable from an accepted domain
> decision?

The experiment does not claim that these agents discover the Stock Management
model. The model already exists. It claims only that the four rubric axes below
can be measured against a source the agent never saw.

The honest formulation carried into the chapter is:

```text
The run is inspectable and attributable. Re-execution may produce different
candidates; what remains reproducible is the input pack, the contract, the
evaluation rubric, and the human review question.
```

---

## 2. The spine

Specialists do not produce the domain carrier. They produce candidate material
that a domain authority may accept, reject, or use to update the carrier.

```text
raw evidence
  -> specialist proposals
  -> human gates
  -> accepted domain material
  -> eventual carrier update
```

This preserves the Chapter 8 contract: the specialist transforms, the authority
decides.

---

## 3. Separation of run pack and evaluation baseline

### Run pack — what the agent sees

The canonical inputs live under `docs/ch09/run-pack/`:

```text
docs/ch09/run-pack/
├── constitution-excerpt.md  modeling principles only, see below
├── stock-walkthrough.md     raw evidence, stable source identifiers
├── glossary.md              reduced business vocabulary
├── handoff-schema.md        the output contract shared by all specialists
└── contracts/               one contract file per specialist
```

**Amendment, 2026-08-16, before the first run.** This section originally placed a
full copy of `.specify/memory/constitution.md` in the run pack. That was wrong on
inspection: the constitution carries `AR-000` through `AR-011`, which govern
module layout, project structure, and composition-root registration. Those rules
address implementation structure, and a discovery specialist given them is being
invited to propose a solution shape instead of domain candidates.

The run pack therefore carries `constitution-excerpt.md`: the modeling-discipline
statements only — domain events for state change, each context speaking its own
language, ownership explicit, and unknown policy remaining an open question.
Excluded are the domain-layer purity rules, the value-object and identifier
rules, the whole of the modular-architecture principle, and every `AR-###` rule.

The excerpt names no bounded context and no policy, so it does not weaken the
Boundary or Status axes.

### Evaluation baseline — what the agent never sees

**Terminology amendment, 2026-08-18, before the first run.** Earlier versions of
the protocol called this material an *answer key*. That wording could imply that
the current carrier is the correct domain answer, when it is only a reference
against which coverage, contamination, and newly surfaced uncertainty can be
evaluated. The protocol therefore uses *evaluation baseline*. This amendment
changes no input, hypothesis, rubric, or checkpoint, and tag `9.1` remains on
the original pre-run freeze.

```text
.specify/memory/domain-carriers/brewup-stock-management.md
docs/ch09/evidence-inventory.md
docs/ch09/leak-check.md
docs/ch09/failure-hypotheses.md
docs/ch09/protocol.md
```

### Input staging, and the limit of what it proves

An instruction not to read a file is not a control. Each run therefore executes
in a scratch workspace that contains only the run pack files and the single
contract for the specialist under test — no `.specify/` directory, no `src/`, no
`.git` of this repository, no copy of the carrier.

The manifest records a complete recursive listing of that workspace. A run whose
manifest lists a file outside the run pack is void and must be re-executed.

**A listing proves staging, not isolation.** It establishes which files were
placed in front of the agent. It does not establish that the runtime was unable
to read an absolute path outside the workspace, reach the repository elsewhere on
the filesystem, or retrieve anything over the network. An earlier draft of this
protocol claimed the listing was "the evidence of isolation". It is not, and the
chapter must not say so.

The two claims are therefore kept apart, and each run states which it earned:

| Claim | What it requires |
|---|---|
| **Controlled input staging** | The workspace listing in the manifest, and nothing more. |
| **Technical isolation** | Filesystem access confined to the workspace; carrier and repository not mounted or reachable; network disabled; the sandbox configuration or mount list recorded verbatim in the manifest. |

Where the enforcement boundary cannot be recorded, the run is reported as
controlled input staging. That is a weaker claim, and it is still a useful one:
the agent was given this evidence and no other, and the output is scored against
a source that was not placed in front of it. What it does not support is the
sentence "the agent could not have read the carrier", which may appear in the
chapter only for runs whose manifest records the enforcement boundary above.

### Fixture provenance

`stock-walkthrough.md` is a synthetic fixture, not a record of an interview that
took place. Its contradictions, omissions, and vocabulary variation were designed
before any specialist was written or executed; the commit history of this
directory is the evidence of that ordering.

The fixture declares its own synthetic nature and the fictional status of its
speakers, so that no artifact in this experiment makes a claim about provenance
that is untrue. It does not state that its contradictions were *designed*, and it
does not mention the experiment: that sentence would tell the agent under test
that the material contains deliberate traps, which is a different and easier
task than the one being measured. The full statement lives here, in the
evaluation baseline, where it can be verified without reaching the agent.

### Glossary reduction rule

The carrier's Ubiquitous Language section names the four internal events. If the
run-pack glossary repeated them, the Coverage axis would be tautological.

`glossary.md` therefore contains only the business vocabulary required to read
the walkthrough — the words a brewery employee uses. It must not contain:

- the names of any aggregate, command, or event in the carrier's Owned Concepts;
- the names of the carrier's external inputs;
- any confirmed, proposed, or forbidden policy.

The glossary is checked against the carrier before the first run, and the check
is recorded in `docs/ch09/leak-check.md`.

---

## 4. The handoff record

Every specialist emits candidates carrying these fields:

| Field | Meaning |
|---|---|
| `id` | Stable identifier for the candidate within the run |
| `source` | One or more stable source identifiers from the evidence |
| `status` | `observed`, `inferred`, `proposed`, or `unresolved` |
| `decision_owner` | The authority that must decide, or `unknown` |
| `uncertainty` | What would have to be true, and what is missing |
| `review_question` | The question a human must answer before this advances |

`decision_owner` names who owns the decision, never who produced the artifact.

`uncertainty` replaces self-reported confidence. A model's stated confidence is
not a measurement, and treating it as one would import into the evaluation the
same unearned authority the chapter is about.

### Statuses a specialist may not emit

`accepted` and `rejected` do not exist in the specialist vocabulary. They appear
only in `human-decision.md`. A specialist that emits either has failed the
contract regardless of the quality of its content.

---

## 5. Evaluation rubric

Each run is scored on four axes. Scoring is performed by a human with the
evaluation baseline open, and recorded in `evaluation.md`.

### Coverage

Of the behaviors and facts actually present in the walkthrough, which were
surfaced as candidates?

Measured against the `E-` items in `docs/ch09/evidence-inventory.md`, written
together with the walkthrough and held in the evaluation baseline. Recorded as:
surfaced, missed, and unsupported additions — candidates with no basis in the
evidence.

Coverage measures the evidence, not the carrier. An event in the carrier that
the walkthrough never mentions cannot be counted as missed.

Each contract declares the `E-` items its output is expected to account for, and
a run is scored only against its own declared subset. A specialist is not charged
for material that belongs to a different specialist's transformation. The mapping
from candidates onto carrier concepts is evaluated once, across all four outputs
together, at checkpoint `9.5`.

### Boundary

Did the output separate what Stock Management produces, what it merely observes,
and who owns each decision?

Checked against `BC-001` through `BC-005`. Failures include an order-lifecycle
fact modeled as an internal Stock event, or a decision attributed to Stock that
belongs to Sales, Payment, or Shipment.

### Status and restraint

Did any candidate silently resolve an open question or present a proposed policy
as settled?

Checked against the carrier's Open Questions, Proposed Policies, Policy
Invention Rules, and Forbidden Assumptions. Each of the four open questions is
recorded as `preserved`, `answered`, or `not addressed`. Only `answered` is a
failure; `not addressed` is a Coverage observation, not a restraint violation.

### Provenance

Does every candidate cite at least one stable source identifier that exists in
the walkthrough and supports the claim?

Failure modes: no citation, an identifier that does not exist, or an identifier
that exists but does not support the candidate. The third is the most important
and the easiest to miss.

Source identifiers are stable tokens such as `OBS-01` and `QUOTE-03`. Line
numbers are not used: they break on any edit and would make old runs
unverifiable.

---

## 6. Pre-registered failure hypotheses

Expected failures are hypotheses until a run produces them.

`docs/ch09/failure-hypotheses.md` is committed before the first execution. Its
commit timestamp is the pre-registration evidence. After each run, every
hypothesis is recorded as `OBSERVED` or `NOT OBSERVED`, with the raw output as
the citation.

A hypothesis that is not observed is reported as not observed. It is never
written into the chapter as though it had happened.

### Contaminated-input probes

When a hypothesis is not observed in the primary run, an optional second run may
present the specialist with a deliberately contaminated walkthrough to test
whether it recognizes the contamination.

A probe is a different question from the primary run and is labeled as such in
both the run record and the chapter prose. A probe result may never be presented
as a primary run result.

---

## 7. Run record layout

```text
docs/ch09/runs/9.2-eventstormer/
├── input-manifest.yaml
├── invocation.md
├── raw-output.md
├── evaluation.md
└── human-decision.md
```

`input-manifest.yaml` records:

- `run_id`, `date`, `repo_commit`;
- the contract file and its SHA-256;
- every input file and its SHA-256;
- the complete recursive listing of the scratch workspace;
- the model identifier and the runtime or harness used;
- any available parameters;
- the exact procedure or command used to execute the run.

`invocation.md` holds the verbatim instruction given to the agent.

`raw-output.md` holds the agent's output unedited. Formatting is not corrected,
and truncation is marked explicitly.

`evaluation.md` holds the four-axis scoring and the hypothesis outcomes.

`human-decision.md` holds what a domain authority accepted, rejected, or
referred back, and the reason. This is the only file in which `accepted` and
`rejected` may appear.

---

## 8. Evidence checkpoints

Tags mark checkpoints of evidence, not chapter headings.

| Tag | Checkpoint |
|---|---|
| `9.1` | Protocol, fixtures, handoff schema, and the four contracts |
| `9.2` | EventStormer run and evaluation |
| `9.3` | Storyteller run and evaluation |
| `9.4` | Command & Event Writer run and evaluation |
| `9.5` | Context Mapper run, plus the integrated cross-specialist evaluation |

---

## 9. Naming

The third specialist is **Command & Event Writer** in the outline, the chapter
headings, the contract file, and the prose. "Modeler" is not used.

Agent contracts are named `brewup.<specialist>.agent.md` without a `speckit.`
prefix. The Chapter 8 guards carry that prefix because they hook the Spec Kit
lifecycle. These specialists run before specification begins and depend on no
framework, and the file name should not imply otherwise.
