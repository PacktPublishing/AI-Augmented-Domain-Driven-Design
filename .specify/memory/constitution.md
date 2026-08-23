<!--
SYNC IMPACT REPORT
==================
Version change: TEMPLATE (unversioned) → 1.0.0
Rationale: Initial ratification of the project constitution from the template.
           MAJOR bump (0 → 1.0.0) establishes the first governed baseline.

Modified principles:
  - [PRINCIPLE_1_NAME] → I. Domain-Driven Design (NON-NEGOTIABLE)
  - [PRINCIPLE_2_NAME] → II. Modular Architecture & Bounded Contexts
  - [PRINCIPLE_3_NAME] → III. Test-First Discipline (NON-NEGOTIABLE)
  - [PRINCIPLE_4_NAME] → IV. Architecture Fitness Functions
  - [PRINCIPLE_5_NAME] → V. Property-Based & Mutation Testing

Added sections:
  - Technology & Architecture Constraints (was [SECTION_2_NAME])
  - Development Workflow & Quality Gates (was [SECTION_3_NAME])
  - Governance (populated)

Removed sections: none

Templates requiring updates:
  - .specify/templates/plan-template.md ✅ compatible (generic "Constitution Check"
    gate references the constitution file; no hardcoded principle names to update)
  - .specify/templates/spec-template.md ✅ compatible (no constitution coupling)
  - .specify/templates/tasks-template.md ✅ compatible (task categories already cover
    tests, architecture, and quality gates expressed here)

Follow-up TODOs:
  - TODO(RATIFICATION_DATE): Original adoption date assumed to equal first authoring
    date (2026-06-25). Replace with the true ratification date if it differs.
-->

# Constitution

## Core Principles

### I. Domain-Driven Design (NON-NEGOTIABLE)

The domain is the heart of the system and MUST be modeled explicitly using DDD building
blocks. Every bounded context MUST express its business rules through aggregates, entities,
value objects, domain events, and domain services that speak the ubiquitous language of
that context.

Rules:

- The domain layer MUST be pure: no dependencies on persistence, messaging, UI, or any
  infrastructure framework. Infrastructure concerns are pushed outward (Facade/Application
  and Infrastructure layers).
- Invariants MUST be enforced inside aggregates; no aggregate may be constructed in an
  invalid state. Validation that protects an invariant belongs in the domain, not in the UI
  or controllers.
- Value objects MUST be immutable and compared by value; identifiers MUST be strongly typed
  (no naked primitives crossing boundaries).
- State changes MUST be expressed as domain events; cross-context communication uses
  explicit commands/events, never shared database tables or implicit coupling.
- Each bounded context follows the layered layout: `Domain`, `SharedKernel`,
  `Infrastructure`, `ReadModel`, `Facade`, and `Tests`.

Rationale: A pure, invariant-protecting domain keeps business rules testable, portable, and
resistant to infrastructure churn, which is the core value the BrewUp DDD approach delivers.

### II. Modular Architecture & Bounded Contexts

The system is a modular monolith: independently reasoned modules grouped by bounded context,
composed through an explicit composition root.

Rules:

- Each bounded context MUST live in its own folder tree and assemblies. A module MUST NOT
  reference another module's internal `Domain` or `Infrastructure` types.
- Modules MUST register themselves explicitly at the composition root (an `IModule`-style
  registration). Implicit or convention-magic wiring that hides the feature surface is
  prohibited.
- Cross-context interaction MUST occur only through published contracts (commands/events in
  the shared messages location) or read-model contracts — never direct method calls into
  another context's internals.
- Shared code MUST be classified deliberately: truly cross-cutting concerns go to the shared
  project; types shared inside one context go to that context's `SharedKernel`. Avoid
  growing a generic "common dumping ground."
- A module MUST be removable/replaceable without forcing edits across unrelated modules.

Rationale: Explicit module boundaries and contracts keep the monolith decomposable, make the
feature surface visible, and prevent the tangle that erodes maintainability over time.

### III. Test-First Discipline (NON-NEGOTIABLE)

Tests define behavior before implementation. The Red-Green-Refactor cycle is mandatory.

Rules:

- For every behavioral change, a failing test MUST exist before the implementation that
  makes it pass. Tests are written → observed failing → implementation → observed passing.
- Domain logic MUST be covered by fast, infrastructure-free unit tests located in each
  context's `Tests` project.
- Integration tests MUST cover contract boundaries: inter-context messages, read-model
  projections, persistence behavior, and API endpoints.
- A change is not "done" until its tests pass in CI and no existing test regresses.
- Bug fixes MUST start with a test that reproduces the defect.

Rationale: Test-first work produces an executable specification, prevents regressions, and is
the only reliable guard for invariants in an event-driven, message-coupled system.

### IV. Architecture Fitness Functions

Architectural rules MUST be enforced automatically, not by convention or review alone.

Rules:

- Each solution MUST include automated architecture (fitness) tests that fail the build when
  structural rules are violated.
- Fitness functions MUST verify at minimum: the domain layer has no infrastructure
  dependencies; modules do not reach into other modules' internals; cross-context coupling
  flows only through approved contracts; naming/layering conventions per bounded context.
- Fitness functions for measurable qualities (e.g., dependency direction, layering, allowed
  references) MUST be deterministic and run in CI on every change.
- A violation reported by a fitness function is a build failure, not a warning. Suppressions
  MUST be explicit, justified in code, and reviewed.

Rationale: Fitness functions turn architectural intent into continuously verified guarantees,
preventing slow erosion of the modular DDD structure.

### V. Property-Based & Mutation Testing

Beyond example-based tests, correctness and test-suite quality MUST be validated with
property-based testing and mutation testing.

Rules:

- Core domain behaviors and value-object invariants MUST include property-based tests that
  assert properties over generated inputs (e.g., round-trips, idempotence, commutativity,
  invariant preservation), not just hand-picked examples.
- Mutation testing MUST be run against domain and application logic; the suite MUST detect
  (kill) introduced mutants. A minimum mutation score threshold MUST be defined per context
  and enforced in CI; surviving mutants MUST be triaged.
- Property-based and mutation tooling MUST be wired into the CI pipeline and reported, so
  weak or tautological tests are surfaced rather than hidden.
- New domain invariants SHOULD be expressed as properties first when a general rule (not a
  single example) is being specified.

Rationale: Property-based tests explore the input space machines find but humans miss, and
mutation testing measures whether the tests actually constrain behavior — together they keep
the test suite honest where invariants matter most.

## Technology & Architecture Constraints

- Architecture style: DDD modular monolith organized by bounded context, with CQRS and
  message/event-driven flows. Read models are denormalized and rebuilt from events.
- Layering per context is fixed: `Domain` → `SharedKernel` → `Infrastructure` /
  `ReadModel` → `Facade`. Dependencies point inward toward the domain; the domain depends on
  nothing external.
- Cross-cutting infrastructure (persistence helpers, messaging settings, read-model bases)
  lives in dedicated shared infrastructure projects, kept out of the domain.
- Observability is a first-class concern: structured logging and distributed tracing/metrics
  MUST be available for services so production behavior is debuggable.
- External backing services (event store, document/read-model store, message bus) MUST be
  reachable only through infrastructure-layer abstractions, never referenced from the domain.
- New external dependencies MUST be justified against simplicity (YAGNI) and isolated behind
  the appropriate layer.

## Development Workflow & Quality Gates

- Every pull request MUST pass: unit tests, integration tests, architecture fitness
  functions, property-based tests, and the mutation-score gate. A red gate blocks merge.
- Code review MUST verify compliance with all five Core Principles. Reviewers reject changes
  that introduce cross-context coupling, leak infrastructure into the domain, or add behavior
  without tests.
- Architectural decisions that deviate from these principles MUST be documented with
  rationale and an explicit, reviewed exception; undocumented deviations are not permitted.
- Complexity MUST be justified: prefer the simplest design that satisfies invariants and
  passes the gates. Speculative generality is rejected.
- CI MUST run the full quality-gate suite on every change to the default and feature branches.

## Governance

This constitution supersedes other practices and conventions where they conflict. When a
guideline contradicts a principle here, the principle wins.

- Amendments MUST be proposed via pull request, documented with rationale, and approved
  before merge. Each amendment MUST update the version and the Sync Impact Report, and
  propagate any required changes to dependent templates (`plan-template.md`,
  `spec-template.md`, `tasks-template.md`).
- Versioning policy (semantic):
  - MAJOR: backward-incompatible governance changes, principle removals, or redefinitions.
  - MINOR: a new principle/section is added or guidance is materially expanded.
  - PATCH: clarifications, wording, and non-semantic refinements.
- Compliance is enforced continuously through the automated quality gates (tests, fitness
  functions, property-based tests, mutation thresholds). Reviews MUST confirm gate results
  before approval.
- Runtime, agent-facing development guidance MUST stay consistent with this constitution; any
  conflict is resolved in favor of the constitution and the guidance updated.

**Version**: 1.0.0 | **Ratified**: 2026-06-25 | **Last Amended**: 2026-06-25
