# Chapter 10 — Orchestrator evidence

Chapter 9 preserved the work of each specialist.

Chapter 10 preserves the coordination around that work.

The executable orchestration code is part of the existing BrewUp solution under:

```text
src/Orchestration/
  BrewUp.Orchestration.Application/
  BrewUp.Orchestration.Infrastructure/
  BrewUp.Orchestration.Tests/
```

The module is intentionally not a second application and does not replace
`src/BrewUp.Rest/Program.cs`. Chapter 10 exercises the orchestration behaviour
through the test project, keeping the existing REST composition root unchanged.

```text
Application     -> protocol, workflow state, orchestration loop
Infrastructure  -> JSON persistence, repository records, runner adapters
Tests           -> executable Chapter 10 demonstrations and invariants
Markdown        -> inspectable modeling evidence
Git             -> history and reproducibility
```

The orchestrator does not duplicate the accepted Chapter 9 artifacts. It references
them from `docs/ch09`.

Run the complete Chapter 10 test suite from the repository root:

```bash
dotnet test src/Orchestration/BrewUp.Orchestration.Tests/BrewUp.Orchestration.Tests.csproj --nologo
```

The suite contains 25 tests: 4 eligibility tests, 9 protocol tests, 9 orchestration
loop tests, and 3 repository-catalog/replay tests.
