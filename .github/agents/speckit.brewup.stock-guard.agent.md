---
description: Validate that the generated BrewUp Stock Management specification respects the Stock Management domain carrier.
handoffs:
  - label: Clarify Specification
    agent: speckit.clarify
    prompt: Clarify the Stock Management specification and preserve unresolved domain decisions as explicit questions.
  - label: Build Technical Plan
    agent: speckit.plan
    prompt: Build the technical plan only if the Stock Management specification passed the domain guard.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

# BrewUp Stock Management Domain Guard

You are validating a generated feature specification against the BrewUp Stock Management domain carrier.

This command runs after `/speckit.specify`.

Its purpose is not to improve the specification.

Its purpose is to report, by name, every place where the specification failed to preserve the carrier.

Do not rewrite the specification.

Do not add missing behaviour.

Do not resolve open questions.

A specification that looks complete but violates the carrier is a failed translation, not a draft.

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
BrewUp Stock Guard: BLOCKED
Stock Management domain carrier not found.
Validation cannot proceed without domain context.
```

## Specification under validation

Validate the Stock Management specification.

Select it in this order:

1. The specification path given in `$ARGUMENTS`, if present.
2. The most recently created `specs/*/spec.md` whose purpose concerns stock reservation, availability, or release.

Do not fall back to a Sales Order specification.

If no Stock Management specification is found, report `BLOCKED` and name the folders you inspected.

State the selected path explicitly under `Checked files`.

## Checks

Run every check below against the carrier.

### Language check

Every domain term used by the specification must appear in the carrier's Ubiquitous Language or Owned Concepts.

Flag generic substitutions, such as `Order` where `Sales Order` is meant, or `Warehouse` used as a bounded context name rather than as the implementing module.

### Ownership check

The specification must assign to Stock Management only the aggregates, commands, and internal events listed under Owned Concepts.

Flag any responsibility that belongs to Sales, Payment, or Shipment. Cite `BC-001`, `BC-002`, `BC-003`, or `BC-005`.

### External input check

`OrderSubmitted`, `OrderCancelled`, and `OrderExpired` must be treated as external inputs, never as internal Stock Management events.

An order-lifecycle fact not listed in the carrier must remain an open question. Cite `BC-004`.

### Policy invention check

Flag any policy the carrier does not confirm, in particular a reservation window, a retry policy, a partial-reservation rule, a failure-classification rule, or a compensation behaviour.

A plausible default is still an invention.

### Proposed-policy promotion check

Proposed policies must stay marked as proposed.

Flag any proposed policy the specification presents as confirmed.

### Open questions check

Every open question in the carrier must survive in the specification, as an open question or `[NEEDS CLARIFICATION]`.

Flag any that was silently answered. Quote the answer as evidence.

### Forbidden assumptions check

Flag any of the four forbidden assumptions the specification relies on, explicitly or implicitly.

## Output format

Return the result using this exact structure:

```text
BrewUp Stock Guard: PASS|FAIL|BLOCKED

Checked files:
- ...

Missing optional files:
- ...

Summary:
- ...

Language check:
- PASS|FAIL
- Details: ...

Ownership check:
- PASS|FAIL
- Details: ...

External input check:
- PASS|FAIL
- Details: ...

Policy invention check:
- PASS|FAIL
- Details: ...

Proposed-policy promotion check:
- PASS|FAIL
- Details: ...

Open questions check:
- PASS|FAIL
- Details: ...

Forbidden assumptions check:
- PASS|FAIL
- Details: ...

Violations:
- Severity:
  Rule:
  Location:
  Evidence:
  Why it matters:
  Suggested correction:

Open questions preserved:
- ...

Recommended next step:
- Continue to /speckit.clarify
- Continue to /speckit.plan
- Fix the specification before planning
- Ask a domain expert
```

If there are no violations, write:

```text
Violations:
- None
```

## Expected failure example

If the specification creates a shipment once stock has been reserved, report:

```text
Violations:
- Severity: CRITICAL
  Rule: BC-005 and Forbidden Assumptions
  Location: In Scope
  Evidence: "Create a shipment after stock has been reserved."
  Why it matters: Shipment is not owned by Stock Management. The carrier
    excludes shipment concerns by name.
  Suggested correction: Remove the rule. Stock Management emits StockReserved;
    the reaction belongs to the context that owns the order lifecycle.
```

If the specification answers an open question, report:

```text
Violations:
- Severity: HIGH
  Rule: Open Questions
  Location: Confirmed Rules
  Evidence: "A stock reservation expires after fifteen minutes."
  Why it matters: The reservation window is an open question in the carrier.
    A plausible default has been promoted to a confirmed rule.
  Suggested correction: Move the rule back to Proposed Rules and restore the
    open question.
```

## Reporting style

Report violations as named rule violations.

Correct:

```text
The specification violates BC-003 and BC-004.
```

Not:

```text
This specification feels wrong.
```

A named violation can be argued with.

An impression cannot.
