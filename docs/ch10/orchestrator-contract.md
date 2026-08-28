# BrewUp Orchestrator contract

## Responsibility

Coordinate the bounded specialist transformations from Chapter 9 without
performing their modeling work and without replacing human domain authority.

## May

- determine whether a specialist is eligible;
- assemble accepted input packages;
- invoke the current specialist;
- validate deterministic protocol and alignment rules;
- record semantic divergence;
- route human gate decisions;
- persist workflow state and trace;
- retry only the specialist that owns the referred candidate;
- stop execution when a declared precondition, authority, retry, timeout, or tool
  boundary is violated.

## Must not

- accept its own candidate;
- convert candidate material into accepted material;
- resolve an unresolved business question;
- normalize terminology to make the chain look coherent;
- invent domain policy;
- infer decision ownership from convenience;
- repair semantic content on behalf of a specialist;
- silently increase retry, timeout, model, or tool authority after failure.

## Authority model

Deterministic code governs routing and mechanical invariants.

Specialists govern bounded transformations.

Semantic review surfaces possible divergence.

Human reviewers decide what becomes accepted domain material.
