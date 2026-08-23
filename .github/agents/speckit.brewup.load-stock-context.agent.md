---
description: Load BrewUp Stock Management domain context before specification.
handoffs:
  - label: Create Specification
    agent: speckit.specify
    prompt: Create or update the feature specification using this BrewUp Stock Management domain context.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Stock Management Domain Context Loader

You are preparing the domain context for a Spec Kit specification.

This command runs before `/speckit.specify`.

Its purpose is not to generate the specification.

Its purpose is to make the domain language, ownership boundaries, external inputs, forbidden assumptions, and open questions explicit before the specification is created.

## Goal

Load the BrewUp Stock Management domain carrier and produce a concise, authoritative context summary that must guide the next `/speckit.specify` execution.

## Files to read

Read the following files if they exist:

* `.specify/memory/constitution.md`
* `.specify/memory/domain-carriers/brewup-stock-management.md`
* `.specify/memory/architecture/brewup-module-structure.md`

The domain carrier is the source of truth for this bounded context.

This agent does not restate the domain rules.

It loads them, and it fails loudly if it cannot.

If `.specify/memory/domain-carriers/brewup-stock-management.md` is missing, stop and report:

```text
Stock Management domain carrier not found.
Specification cannot proceed without domain context.
```

Do not reconstruct the missing carrier from memory, from the feature request, or from generic e-commerce knowledge.

Other listed files are optional. If one is missing, report it and continue.

## What to extract

From the carrier, extract and restate only:

1. Purpose and the four exclusions.
2. Owned concepts: aggregates, commands, internal events.
3. External inputs.
4. Ubiquitous language entries relevant to the feature request.
5. Bounded context rules `BC-000` through `BC-005`.
6. Confirmed policies, kept separate from proposed policies.
7. Open questions, verbatim.
8. Forbidden assumptions, verbatim.

Open questions and forbidden assumptions must be copied, not paraphrased.

Paraphrasing is how an open question quietly becomes an answer.

## Domain sensitivities

Stock Management sits next to Sales, Payment, and Shipment in most training data.

The model will therefore be tempted to:

* treat a stock reservation as an order confirmation;
* pull payment authorization into the reservation flow;
* create a shipment once stock has been reserved;
* model `Order` as an aggregate owned by this context;
* answer an open question with a plausible default, such as a fifteen-minute
  reservation window or a three-attempt retry policy.

Each of these is ruled out by the carrier.

Flag any of them in the feature request before `/speckit.specify` runs.

## Output shape

```markdown
# BrewUp Stock Management Context

## Purpose and exclusions

- ...

## Owned by Stock Management

- ...

## External inputs

- ...

## Confirmed policies

- ...

## Proposed policies

- ...

## Open questions

- ...

## Forbidden assumptions

- ...

## Rules for /speckit.specify

- ...
```

## Output constraints

The output must be short enough to be pasted into, or referenced by, the next `/speckit.specify` command.

Do not create or modify files.

Do not generate implementation tasks.

Do not generate code.

Do not resolve open questions.

Do not invent domain policy.

Do not promote a proposed policy to a confirmed one.

Your job is to prepare the reasoning environment, not to design the full system.
