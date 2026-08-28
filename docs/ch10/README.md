# Chapter 10 — Orchestrator evidence

Chapter 9 preserved the work of each specialist.

Chapter 10 preserves the coordination around that work.

The executable control plane lives under `ch10-orchestrator/`. This directory
contains the human-readable evidence used to inspect the orchestration experiment.

```text
C#       -> executable mechanism
JSON     -> persisted machine state
Markdown -> inspectable modeling evidence
Git      -> history and reproducibility
```

The orchestrator does not duplicate the accepted Chapter 9 artifacts. It references
them from `docs/ch09`.
