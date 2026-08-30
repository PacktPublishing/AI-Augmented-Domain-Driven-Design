# Chapter 11 — Governed autonomy evidence

Chapter 10 made coordination executable.

Chapter 11 adds explicit boundaries around the freedom each specialist has
inside its transformation.

The executable mechanism remains in the existing modular orchestration code
under `src/Orchestration/`. This directory contains the inspectable evidence for
the controlled Storyteller drift-and-correction run.

```text
C#       -> governance mechanism
Markdown -> candidate and review evidence
Git      -> history and reproducibility
```

The run deliberately keeps a structurally valid but semantically overconfident
candidate. The semantic reviewer reports divergence without rewriting the
candidate. A human referral then sends correction back to Storyteller, and the
second attempt keeps `ES-17` unresolved.
