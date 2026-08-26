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

## Verify the declared test counts

Run from the repository root:

```bash
dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~EligibilityEvaluatorTests --nologo

dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~ConversationProtocolTests --nologo

dotnet test ch10-orchestrator/tests/BrewUp.Orchestration.Tests \
  --filter FullyQualifiedName~BrewUpOrchestrationLoopTests --nologo

dotnet test ch10-orchestrator/BrewUp.Orchestration.slnx --nologo
```

The expected counts are respectively 4, 7, 8, and 19 passing tests.

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
generation. It reports explicitly when a repository gate is being simulated.
It does not claim that a pending human decision has become accepted.

## Verify and run against Chapter 9 authority records

Inspect the repository gates without invoking a specialist:

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- --verify-ch09
```

Run the loop only if all four human gates are complete:

```bash
dotnet run --project ch10-orchestrator/src/BrewUp.Orchestration -- \
  --run-repository-loop
```

The repository-backed command fails closed with exit code `3` while any human
gate is missing or pending. In the current Packt checkpoint, the Context Mapper
decision in `docs/ch09/runs/9.5-context-mapper/human-decision.md` is still
pending, so this command must not reach `Completed` until a human authority
records the decision.
