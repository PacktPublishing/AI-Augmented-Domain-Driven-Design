<!--
SYNC IMPACT REPORT
==================
Version change: 1.0.0 -> 1.1.0
Rationale: Added explicit Spec-Driven Development governance and architecture rules for
           AI-assisted planning and implementation. Expanded modular architecture rules so
           bounded-context authority must be reflected in the physical solution structure.
           MINOR bump because governance was materially expanded without removing or
           redefining existing principles.

Modified principles:
  - II. Modular Architecture & Bounded Contexts -> II. Modular Architecture, Bounded Contexts & Module Structure
  - IV. Architecture Fitness Functions -> expanded to include agent-generated plans/tasks and module-structure drift

Added principles:
  - VI. Spec-Driven Development & Agent Governance

Added sections:
  - Architecture Rules (AR-###)
  - Agent-Facing Memory & Durable Context

Removed sections: none

Templates requiring updates:
  - .specify/templates/plan-template.md ⚠ review recommended: the Constitution Check should verify AR-001 through AR-011 when a plan introduces or implements a bounded context/module.
  - .specify/templates/spec-template.md ⚠ review recommended: generated specifications should preserve domain rules and explicitly state whether introduced authorities are in scope for implementation.
  - .specify/templates/tasks-template.md ⚠ review recommended: generated tasks should include module-structure tasks when a new module is in scope.

Agent-facing guidance requiring updates:
  - .github/copilot-instructions.md ✅ updated to include module creation and architecture rules.
  - .github/agents/*.brewup.speckit.agent.md ⚠ update recommended: plan/task guards should check both BC-### and AR-### rules.
  - .specify/memory/architecture/brewup-module-structure.md ⚠ create recommended: detailed BrewUp module layout and project responsibilities.

Follow-up TODOs:
  - TODO(ARCHITECTURE_MEMORY): Create .specify/memory/architecture/brewup-module-structure.md.
  - TODO(AGENTS): Add a dedicated architecture/module-structure guard agent or extend existing plan/task guards.
-->

# DWX26 Constitution

## Core Principles

### I. Domain-Driven Design (NON-NEGOTIABLE)

The domain is the heart of the system and MUST be modeled explicitly using DDD building
blocks. Every bounded context MUST express its business rules through aggregates, entities,
value objects, domain events, and domain services that speak the ubiquitous language of
that context.

Rules:

- The domain layer MUST be pure: no dependencies on persistence, messaging, UI, or any
  infrastructure framework. Infrastructure concerns are pushed outward to Facade,
  ReadModel, and Infrastructure layers.
- Invariants MUST be enforced inside aggregates; no aggregate may be constructed in an
  invalid state. Validation that protects an invariant belongs in the domain, not in the UI
  or controllers.
- Value objects MUST be immutable and compared by value; identifiers MUST be strongly typed
  and MUST NOT be represented as naked primitives across boundaries.
- State changes MUST be expressed as domain events; cross-context communication uses
  explicit commands/events, never shared database tables or implicit coupling.
- Each bounded context MUST speak its own ubiquitous language. Generic terminology is not
  acceptable when a domain-specific term exists.
- Domain ownership MUST be explicit. A bounded context may depend on a decision produced by
  another bounded context, but it MUST NOT own or reproduce that decision unless explicitly
  assigned by the specification.
- Unknown business policy MUST remain visible as an open question or `[NEEDS CLARIFICATION]`.
  Agents MUST NOT invent domain policy to make a model look complete.

Rationale: A pure, invariant-protecting domain keeps business rules testable, portable, and
resistant to infrastructure churn, which is the core value the BrewUp DDD approach delivers.

### II. Modular Architecture, Bounded Contexts & Module Structure

The system is a modular monolith: independently reasoned modules grouped by bounded context,
composed through an explicit composition root.

A bounded context is not only a conceptual boundary. When it is in scope for implementation,
it MUST be represented as an explicit module in the solution.

Rules:

- Each bounded context MUST live in its own folder tree and assemblies. A module MUST NOT
  reference another module's internal `Domain` or `Infrastructure` types.
- Every implemented business module MUST follow the standard BrewUp module structure:
  `Domain`, `SharedKernel`, `Infrastructure`, `ReadModel`, `Facade`, and `Tests`.
- A specification that introduces a new bounded context or domain authority MUST explicitly
  state whether that authority is in scope for implementation. If it is in scope, the plan
  and tasks MUST create the corresponding module structure.
- A module MUST NOT be implemented as loose folders, helper classes, or ad-hoc services inside
  another module.
- Modules MUST register themselves explicitly at the composition root using the existing
  `IModule`-style registration. Implicit or convention-magic wiring that hides the feature
  surface is prohibited.
- Cross-context interaction MUST occur only through published contracts, shared messages,
  integration events, facades, or explicit read-model contracts — never direct method calls
  into another context's internals.
- Shared code MUST be classified deliberately: truly cross-cutting concerns go to the shared
  project; types shared inside one context go to that context's `SharedKernel`. Avoid
  growing a generic common dumping ground.
- A module MUST be removable or replaceable without forcing edits across unrelated modules.

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
- A change is not done until its tests pass in CI and no existing test regresses.
- Bug fixes MUST start with a test that reproduces the defect.
- When a generated plan or task list introduces behavior, the task list MUST include the
  corresponding failing tests before implementation tasks.

Rationale: Test-first work produces an executable specification, prevents regressions, and is
the only reliable guard for invariants in an event-driven, message-coupled system.

### IV. Architecture Fitness Functions

Architectural rules MUST be enforced automatically, not by convention or review alone.

Rules:

- Each solution MUST include automated architecture fitness tests that fail the build when
  structural rules are violated.
- Fitness functions MUST verify at minimum: the domain layer has no infrastructure
  dependencies; modules do not reach into other modules' internals; cross-context coupling
  flows only through approved contracts; naming and layering conventions per bounded context.
- Fitness functions MUST verify that implemented bounded contexts follow the standard module
  structure.
- Fitness functions MUST verify that a module's `Domain` project is not referenced directly
  by another module.
- Fitness functions MUST verify that generated plans and tasks do not introduce files,
  projects, or dependencies that violate this constitution.
- Fitness functions for measurable qualities such as dependency direction, layering, and
  allowed references MUST be deterministic and run in CI on every change.
- A violation reported by a fitness function is a build failure, not a warning. Suppressions
  MUST be explicit, justified in code, and reviewed.

Rationale: Fitness functions turn architectural intent into continuously verified guarantees,
preventing slow erosion of the modular DDD structure.

### V. Property-Based & Mutation Testing

Beyond example-based tests, correctness and test-suite quality MUST be validated with
property-based testing and mutation testing.

Rules:

- Core domain behaviors and value-object invariants MUST include property-based tests that
  assert properties over generated inputs, not just hand-picked examples.
- Mutation testing MUST be run against domain and application logic; the suite MUST detect
  introduced mutants. A minimum mutation score threshold MUST be defined per context and
  enforced in CI; surviving mutants MUST be triaged.
- Property-based and mutation tooling MUST be wired into the CI pipeline and reported, so
  weak or tautological tests are surfaced rather than hidden.
- New domain invariants SHOULD be expressed as properties first when a general rule, not a
  single example, is being specified.

Rationale: Property-based tests explore the input space machines find but humans miss, and
mutation testing measures whether the tests actually constrain behavior. Together they keep
the test suite honest where invariants matter most.

### VI. Spec-Driven Development & Agent Governance

Specifications are governed artifacts. They are not disposable prompts.

Spec-driven artifacts MUST preserve durable domain and architecture decisions across the
entire AI-assisted workflow: specification, clarification, plan, tasks, analysis, and
implementation.

Rules:

- A prompt may describe the requested workflow, but the specification defines domain language,
  decision authority, invariants, external references, and accepted outcomes.
- Generated plans and tasks MUST NOT introduce new domain ownership, business policy, module
  boundaries, or architecture decisions that are absent from the specification or memory.
- Agent-facing memory files MAY provide pre-specification context, but the generated
  `spec.md` MUST preserve the relevant non-negotiable rules needed to govern subsequent
  artifacts.
- If a memory file defines named rules such as `BC-###` or `AR-###`, generated artifacts MUST
  preserve those identifiers when they depend on the rule.
- Open questions MUST remain explicit until resolved by a human domain or architecture
  authority. Agents may identify ambiguity; they MUST NOT resolve it silently.
- Analysis and guard agents MUST report violations using named rules, not vague judgement.
- A generated artifact that violates this constitution is architecturally misaligned even if
  it compiles and tests pass.

Rationale: AI-generated output can be fluent and still wrong. Spec-Driven Development makes
non-negotiable decisions durable enough that architectural drift can be detected before it
becomes code.

## Architecture Rules (AR-###)

These rules are global and MUST be respected by specifications, plans, tasks, source code,
and tests.

### AR-000 — Architecture rules are authoritative

The architecture rules in this constitution constrain all generated artifacts.

A generated plan or task list that violates these rules MUST be treated as failing the
architecture gate.

### AR-001 — Bounded contexts must be explicit modules

Every bounded context or domain authority that is in scope for implementation MUST be
represented as an explicit module under `src/<ModuleName>`.

A module MUST NOT be implemented as loose folders or classes inside another module.

### AR-002 — Standard module project structure

Every implemented BrewUp business module MUST use the standard project structure:

```text
src/<ModuleName>/
  BrewUp.<ModuleName>.Domain
  BrewUp.<ModuleName>.Facade
  BrewUp.<ModuleName>.Infrastructure
  BrewUp.<ModuleName>.ReadModel
  BrewUp.<ModuleName>.SharedKernel
  BrewUp.<ModuleName>.Tests
```

An optional `BrewUp.<ModuleName>.Entities` project MAY exist only when the module needs value
objects shared internally across multiple projects of the same bounded context.

### AR-003 — New authorities must become modules when implementation is in scope

If a specification introduces a new authority and implementation is in scope, the plan MUST
create the corresponding module and solution folder before adding behavior.

If implementation is not in scope, the specification and plan MUST explicitly state that the
authority is external or pre-existing.

### AR-004 — SharedKernel owns module contracts

Module-specific identifiers, commands, domain events, enums, and custom types belong in the
module `SharedKernel`.

For a module named `<ModuleName>`:

```text
BrewUp.<ModuleName>.SharedKernel/DomainIds
BrewUp.<ModuleName>.SharedKernel/Messages/Commands
BrewUp.<ModuleName>.SharedKernel/Messages/Events
BrewUp.<ModuleName>.SharedKernel/Enums
BrewUp.<ModuleName>.SharedKernel/CustomTypes
```

Truly cross-context contracts MAY live in the global shared project, but this MUST be a
deliberate decision, not a default dumping ground.

### AR-005 — Facade is the module entry point

The `Facade` project is the public entry point of a module.

It owns endpoints, ACL handlers, integration entry points, and module registration helpers.

Other modules MUST NOT call another module's `Domain` project directly.

### AR-006 — Domain owns business behavior

The `Domain` project owns aggregates, command handlers, domain services, and domain behavior.

Command handlers MAY load aggregates, invoke aggregate methods, and save aggregates.

Business invariants MUST live in aggregate methods or domain services, not in facades,
endpoints, read models, or infrastructure.

### AR-007 — ReadModel owns queries

The `ReadModel` project owns query models, query services, read-side projections, and event
handlers that update denormalized read models.

Queries MUST NOT be implemented in the `Domain` project.

### AR-008 — Infrastructure owns technical persistence and messaging wiring

The `Infrastructure` project owns technical persistence and transport wiring for the module.

Domain code MUST NOT reference EventStore, MongoDB, RabbitMQ, or other infrastructure SDKs.

### AR-009 — Host registration is explicit

Every implemented module MUST be registered in the REST host through an explicit module
registration class following the existing `IModule` pattern.

### AR-010 — Plans must include structural work before behavior

When a plan introduces a new module, it MUST plan the solution folder and project structure
before planning domain behavior, handlers, read models, endpoints, or tests.

### AR-011 — Tasks must include structural and test work

When tasks introduce a new module, they MUST include explicit tasks for:

- creating the module folder and solution folder;
- creating all standard projects;
- wiring project references according to the allowed dependency direction;
- adding module registration in the host;
- adding architecture tests for the module;
- adding failing behavior tests before implementation.

## Technology & Architecture Constraints

- Architecture style: DDD modular monolith organized by bounded context, with CQRS and
  message/event-driven flows. Read models are denormalized and rebuilt from events.
- Layering per context is fixed: `Domain`, `SharedKernel`, `Infrastructure`, `ReadModel`,
  `Facade`, and `Tests`. Dependencies point inward toward the domain; the domain depends on
  nothing external.
- Cross-cutting infrastructure such as persistence helpers, messaging settings, and
  read-model bases lives in dedicated shared infrastructure projects, kept out of the domain.
- Observability is a first-class concern: structured logging and distributed tracing/metrics
  MUST be available for services so production behavior is debuggable.
- External backing services such as event store, document/read-model store, and message bus
  MUST be reachable only through infrastructure-layer abstractions, never referenced from the
  domain.
- New external dependencies MUST be justified against simplicity and isolated behind the
  appropriate layer.
- Agent-generated changes MUST preserve existing project naming, folder structure, layering,
  test style, and Muflone conventions unless the specification explicitly records an approved
  architecture change.

## Agent-Facing Memory & Durable Context

Agent-facing memory exists to prevent prompts from becoming the only source of architectural
and domain truth.

Rules:

- Global non-negotiables live in this constitution.
- Feature-specific domain rules SHOULD live in `.specify/memory/domain-carriers/` before a
  feature folder exists.
- Architecture-specific detailed rules SHOULD live in `.specify/memory/architecture/`.
- Generated specifications MUST carry relevant domain and architecture rules into the feature
  `spec.md` when those rules are required to govern plan, tasks, or analysis.
- Guard agents MUST check generated artifacts against both domain rules (`BC-###`) and
  architecture rules (`AR-###`) when applicable.

Rationale: The first specification already needs context before the feature folder exists.
Agent-facing memory solves the bootstrap problem while preserving the generated specification
as the official feature contract.

## Development Workflow & Quality Gates

- Every pull request MUST pass: unit tests, integration tests, architecture fitness
  functions, property-based tests, and the mutation-score gate. A red gate blocks merge.
- Code review MUST verify compliance with all Core Principles. Reviewers reject changes that
  introduce cross-context coupling, leak infrastructure into the domain, add behavior without
  tests, or violate module structure.
- Architectural decisions that deviate from these principles MUST be documented with
  rationale and an explicit, reviewed exception; undocumented deviations are not permitted.
- Complexity MUST be justified: prefer the simplest design that satisfies invariants and
  passes the gates. Speculative generality is rejected.
- CI MUST run the full quality-gate suite on every change to the default and feature branches.
- Spec Kit generated artifacts MUST pass the relevant guard checks before implementation
  starts.

## Governance

This constitution supersedes other practices and conventions where they conflict. When a
guideline contradicts a principle here, the principle wins.

- Amendments MUST be proposed via pull request, documented with rationale, and approved
  before merge. Each amendment MUST update the version and the Sync Impact Report, and
  propagate any required changes to dependent templates, agent instructions, memory files,
  and guard agents.
- Versioning policy follows semantic versioning:
  - MAJOR: backward-incompatible governance changes, principle removals, or redefinitions.
  - MINOR: a new principle/section is added or guidance is materially expanded.
  - PATCH: clarifications, wording, and non-semantic refinements.
- Compliance is enforced continuously through automated quality gates: tests, fitness
  functions, property-based tests, mutation thresholds, and Spec Kit guard checks.
- Reviews MUST confirm gate results before approval.
- Runtime and agent-facing development guidance MUST stay consistent with this constitution;
  any conflict is resolved in favor of the constitution and the guidance must be updated.

**Version**: 1.1.0 | **Ratified**: 2026-06-25 | **Last Amended**: 2026-06-26
