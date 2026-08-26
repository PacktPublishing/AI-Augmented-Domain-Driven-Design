# Chapter 10 orchestration laboratory

This standalone .NET 10 laboratory implements the executable path used in
Chapter 10. It coordinates the four Chapter 9 specialists without moving their
transformations or human domain decisions into the orchestrator.

The laboratory keeps three authorities separate:

- deterministic code owns routing, eligibility, message validation, state, and trace;
- specialists produce candidate artifacts through `ISpecialistRunner`;
- a `human-reviewer` message is the only way a candidate becomes accepted input.

## Structure

```text
ch10-orchestrator/
  specialist-registry.json
  workflow-state.json
  gates/eventstormer-review.json
  gates-empty/
  src/BrewUp.Orchestration/
  tests/BrewUp.Orchestration.Tests/
```

Runtime candidates produced by the deterministic runner are persisted under
`artifacts/generated/`. That directory is ignored by Git because the files are
run evidence, not authored source.

Repository-backed workflow snapshots are persisted atomically under
`artifacts/runs/`. Running the same repository command again loads the completed
snapshot instead of invoking a recorded specialist a second time.

## Verify the declared test counts

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

dotnet test ch10-orchestrator/BrewUp.Orchestration.slnx --nologo
```

The expected counts are respectively 4, 9, 9, 3, and 25 passing tests.

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

This command proves the orchestration mechanics with deterministic candidate
generation. Its single `ES-17` identifier is an intentionally small fixture;
it does not claim to replay every unresolved record from Chapter 9.

## Verify and run against Chapter 9 authority records

Inspect the repository gates without invoking a specialist:

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- --verify-ch09
```

The verifier requires an exact `complete` status, every declared source and
decision hash, and the effective accepted artifact. It fails closed on missing,
stale, or mismatched provenance.

Replay the recorded Chapter 9 specialist outputs only if all four human gates
and their provenance records verify:

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- \
  --run-repository-loop
```

The repository-backed command replays the primary and corrected outputs that
actually produced the four accepted handoffs. EventStormer, Storyteller, and
Context Mapper each traverse their recorded referral before acceptance. The
completed snapshot contains four accepted artifacts, fourteen applied
transitions, and all 40 unresolved identifiers carried by the Chapter 9
handoffs, including `ES-17`, `CM-06`, `CM-08`, `CM-12`, and `CM-14`.

The command fails closed with exit code `3` while a gate is incomplete or its
provenance is invalid. A second execution resumes from the persisted snapshot
and prints the same trace without replaying specialist work.
