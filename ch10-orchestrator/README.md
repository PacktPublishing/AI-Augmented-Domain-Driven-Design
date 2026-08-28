# Chapter 10 orchestration laboratory

This standalone .NET 10 laboratory implements the executable path used in
Chapter 10. It coordinates the four Chapter 9 specialists without moving their
transformations or human domain decisions into the orchestrator.

The laboratory keeps four responsibilities separate:

- deterministic code owns routing, eligibility, protocol state, mechanical
  alignment, retry/time/tool boundaries, and trace;
- specialists produce candidate artifacts through `ISpecialistRunner`;
- semantic alignment review may surface divergence but cannot rewrite or accept
  a candidate;
- a `human-reviewer` message is the only way candidate domain material becomes
  accepted input.

## Structure

```text
ch10-orchestrator/
  specialist-registry.json
  workflow-state.json
  gates/eventstormer-review.json
  gates-empty/
  src/BrewUp.Orchestration/
  tests/BrewUp.Orchestration.Tests/

docs/ch10/
  orchestrator-contract.md
  run-pack/
  runs/10.1-alignment/
```

Runtime candidates produced by the deterministic runner are persisted under
`artifacts/generated/`. That directory is ignored by Git because those files are
runtime output.

The authored and reviewed evidence for the controlled alignment experiment is
committed under `docs/ch10/`, following the same inspectable-evidence principle
used by `docs/ch09/`.

Repository-backed workflow snapshots are persisted atomically under
`artifacts/runs/`. Alignment observations are now persisted in the same snapshot
without becoming protocol messages or domain decisions.

## Verify the original orchestration laboratory

Run from the repository root:

```bash
dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~EligibilityEvaluatorTests --nologo

dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~ConversationProtocolTests --nologo

dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~BrewUpOrchestrationLoopTests --nologo

dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~Chapter9RepositoryCatalogTests --nologo
```

The original groups remain respectively 4, 9, 9, and 3 tests.

Run the complete suite after adding the autonomy/alignment tests:

```bash
dotnet test ch10-orchestrator/BrewUp.Orchestration.slnx --nologo
```

## Exercise the eligibility boundary

Before the EventStormer human gate exists, Storyteller is blocked:

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- \
  storyteller --gate-dir gates-empty
```

With the accepted gate, it becomes ready while preserving `ES-17`:

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- \
  storyteller --gate-dir gates
```

## Run the complete deterministic demonstration

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- --demo-loop
```

The command runs all four stages, refers Storyteller once, reruns only that
specialist, and finishes with:

```text
FINAL Completed; accepted=4; unresolved=ES-17
```

## Verify and run against Chapter 9 authority records

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- --verify-ch09

dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- \
  --run-repository-loop
```

The repository-backed command replays the primary and corrected outputs that
actually produced the four accepted handoffs. EventStormer, Storyteller, and
Context Mapper each traverse their recorded referral before acceptance. The
completed snapshot contains four accepted artifacts, fourteen applied
transitions, and all 40 unresolved identifiers carried by the Chapter 9 handoffs.

## Verify the controlled semantic drift

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- \
  --verify-alignment-run
```

The command reads the committed Markdown candidates under
`docs/ch10/runs/10.1-alignment/`.

Attempt 1 must surface:

- a candidate term that is not accepted language;
- a possible policy inference around `ES-17`.

The reviewer must leave the candidate bytes unchanged.

Attempt 2 must preserve `ES-17` while no longer triggering those configured drift
checks.

## Operational autonomy

`AutonomyPolicy` bounds:

- output kind;
- accessible evidence kinds;
- whether new candidate terms may be proposed;
- correction attempts;
- timeout;
- tool allow-list.

Failure does not grant additional authority. Retry exhaustion returns an explicit
stop result, and a retry receives the same tool boundary as the first attempt.

## What the complete laboratory demonstrates

```text
accepted evidence
  -> bounded specialist transformation
  -> deterministic alignment guard
  -> semantic divergence observation
  -> human gate
  -> accepted material or local correction
```

The orchestrator makes coordination more automatic without making authority less
explicit.
