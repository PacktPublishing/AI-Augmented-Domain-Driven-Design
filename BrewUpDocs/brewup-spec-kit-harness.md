# BrewUp Spec Kit Harness

> Detailed explanation of the custom Spec Kit harness built for the DWX26 demo  
> **Spec-Driven Development: Redefining the Software Architect in the AI Era**

This document explains the custom harness built around GitHub Spec Kit and GitHub Copilot for the BrewUp DWX26 demo.

The goal of this harness is not to make AI-generated output deterministic.

The goal is to make domain and architecture decisions explicit, durable, and verifiable across the Spec-Driven Development workflow.

---

## 1. Why this harness exists

The demo starts from a simple but dangerous feature request:

```text
Implement order confirmation for BrewUp.

An order can be confirmed when:
- the customer payment has been authorized;
- all requested beers are available in the warehouse.

When the order is confirmed, reserve the stock.
```

A plausible AI-generated model may produce:

```text
Order
 ├── OrderLines
 ├── Payment
 └── WarehouseReservation
```

and a method such as:

```csharp
public void Confirm()
{
    Payment.Authorize(Total);

    foreach (var line in Lines)
        WarehouseReservation.Reserve(
            line.BeerId,
            line.Quantity);

    Status = OrderStatus.Confirmed;
}
```

The code may compile.

The tests may be green.

The demo may even work.

But the model is still wrong.

It assigns Payment and Warehouse decisions to the Sales Order aggregate.

The issue is not syntax.

The issue is decision authority.

---

## 2. Core principle

The harness is based on this distinction:

```text
BC rules answer:
Who owns the decision?

AR rules answer:
Where must that ownership live in the solution?
```

The first version of the harness focused on bounded-context ownership.

That solved part of the problem:

```text
Payment owns payment authorization.
Warehouse owns stock reservation.
Sales owns the Sales Order lifecycle.
```

However, during the demo it became clear that this was not enough.

The model could preserve the conceptual domain ownership and still fail to create the physical module structure required by BrewUp.

The missing rule was:

```text
If Payment is a business authority in scope for implementation,
Payment must become a BrewUp module.
```

So the harness now protects both:

1. **Domain ownership** — through `BC-###` rules.
2. **Architecture structure** — through `AR-###` rules.

---

## 3. Repository files involved

The harness uses the following files.

```text
.github/
  copilot-instructions.md

.github/
  agents/
    load-domain-context.brewup.speckit.agent.md
    domain-guard.brewup.speckit.agent.md
    plan-readiness.brewup.speckit.agent.md
    plan-guard.brewup.speckit.agent.md
    task-guard.brewup.speckit.agent.md
    module-structure-guard.brewup.speckit.agent.md

.specify/
  extensions.yml

.specify/
  memory/
    constitution.md

.specify/
  memory/
    domain-carriers/
      brewup-sales-order-confirmation.md

.specify/
  memory/
    architecture/
      brewup-module-structure.md
```

Each file has a different role.

---

## 4. `copilot-instructions.md`

### Purpose

`copilot-instructions.md` provides general repository-level guidance for GitHub Copilot.

It is useful because it applies outside the explicit Spec Kit workflow too: chat, edits, refactorings, implementation work, and code review assistance.

It should not contain every detail of the domain model.

It should contain the rules Copilot must remember whenever it works in this repository.

### Main changes added

The updated file adds explicit guidance for:

- Spec-Driven Development context;
- BrewUp as a DDD modular monolith;
- bounded contexts as physical modules;
- standard module structure;
- architecture rules `AR-###`;
- Payment as a module when in scope;
- correct location for IDs, commands, and domain events;
- forbidden collapse of Payment behavior into Sales;
- role of Facade, Domain, ReadModel, Infrastructure, SharedKernel, and Tests.

### Key rule added

```text
We told the model who owns the decision.
Now we also tell it where that ownership must live in the solution.
```

This matters because the model may understand that Payment owns payment authorization but still implement Payment behavior inside Sales unless the physical structure is explicit.

### Example: Payment module rule

If Payment is in scope for implementation, Copilot must create a full module:

```text
src/Payment/
├── BrewUp.Payment.SharedKernel/
├── BrewUp.Payment.Domain/
├── BrewUp.Payment.ReadModel/
├── BrewUp.Payment.Infrastructure/
├── BrewUp.Payment.Facade/
└── BrewUp.Payment.Tests/
```

It must not implement Payment as:

```text
src/Sales/BrewUp.Sales.Domain/PaymentAuthorization.cs
src/Sales/BrewUp.Sales.SharedKernel/Messages/Commands/RequestPaymentAuthorization.cs
src/Sales/BrewUp.Sales.Facade/PaymentEndpoints.cs
```

---

## 5. `constitution.md`

### Purpose

The constitution defines the project-level non-negotiable principles.

It is not a feature specification.

It is not a detailed architecture manual.

It defines the governance baseline.

### Existing principles

The constitution already contained strong principles for:

- Domain-Driven Design;
- modular architecture and bounded contexts;
- test-first discipline;
- architecture fitness functions;
- property-based and mutation testing;
- CQRS and Event Sourcing;
- layered module layout;
- quality gates and governance.

### Changes added

The updated constitution adds:

- Spec-Driven Development governance;
- explicit separation between domain rules and architecture rules;
- architecture rules `AR-###`;
- requirement that new bounded contexts or authorities become explicit modules when in scope;
- requirement that plans and tasks preserve both `BC-###` and `AR-###`;
- relationship between:
  - constitution;
  - domain carriers;
  - architecture memory;
  - generated specs;
  - plans;
  - tasks.

### Why this belongs in the constitution

The constitution should contain global rules such as:

```text
BrewUp is a modular monolith.
Every bounded context must be explicit.
Generated artifacts must not invent business policy.
Generated artifacts must not collapse bounded-context authority.
Plans and tasks must respect explicit rules.
```

It should not contain every detail of the Payment module.

That level of detail belongs in architecture memory.

---

## 6. Domain carrier

### File

```text
.specify/memory/domain-carriers/brewup-sales-order-confirmation.md
```

### Purpose

The domain carrier exists before `/speckit.specify` creates the feature folder.

It solves the bootstrap problem:

```text
/specify creates the feature folder,
but the first specification already needs domain context.
```

The domain carrier provides the domain-specific rules for the feature.

### What it contains

The domain carrier contains:

- BrewUp authorities;
- ubiquitous language;
- Sales / Payment / Warehouse ownership;
- external decision references;
- forbidden responsibilities;
- confirmation invariant;
- policy invention rules;
- open questions;
- rules for generated Spec Kit artifacts.

### BC rules

The domain carrier defines `BC-###` rules.

Examples:

```text
BC-003 — Payment owns authorization outcomes.
BC-007 — Warehouse owns stock mutation.
BC-009 — External decision references, not embedded models.
```

These rules answer:

```text
Who owns the decision?
```

### Why the rules are numbered

Numbered rules let the harness produce precise findings.

Instead of saying:

```text
The design feels wrong.
```

the guard can say:

```text
The plan violates BC-003, BC-007, and BC-009.
```

That is the difference between opinion and inspectable governance.

---

## 7. Architecture memory

### File

```text
.specify/memory/architecture/brewup-module-structure.md
```

### Purpose

Architecture memory defines the physical module structure expected by BrewUp.

It complements the domain carrier.

The domain carrier says:

```text
Payment owns payment authorization.
```

Architecture memory says:

```text
If Payment is in scope, Payment must be represented as:
src/Payment/BrewUp.Payment.*
```

### AR rules

Architecture memory defines `AR-###` rules.

Examples:

```text
AR-001 — Bounded contexts must be explicit modules.
AR-002 — Standard module project structure.
AR-016 — Business authority must not be collapsed into another module.
```

These rules answer:

```text
Where must that ownership live in the solution?
```

### Standard module structure

Every BrewUp business module follows this structure:

```text
src/<ModuleName>/
├── BrewUp.<ModuleName>.SharedKernel/
├── BrewUp.<ModuleName>.Domain/
├── BrewUp.<ModuleName>.ReadModel/
├── BrewUp.<ModuleName>.Infrastructure/
├── BrewUp.<ModuleName>.Facade/
└── BrewUp.<ModuleName>.Tests/
```

Optional:

```text
BrewUp.<ModuleName>.Entities
```

only when the module needs value objects or internal shared structures that do not belong directly to the Domain project.

### Payment module requirement

When Payment is in scope, the plan and tasks must create:

```text
src/Payment/
├── BrewUp.Payment.SharedKernel/
│   ├── DomainIds/
│   ├── Enums/
│   └── Messages/
│       ├── Commands/
│       └── Events/
│
├── BrewUp.Payment.Domain/
│   ├── Entities/
│   ├── CommandHandlers/
│   └── DomainHelper.cs
│
├── BrewUp.Payment.ReadModel/
│   ├── EventHandlers/
│   ├── Queries/
│   ├── Dtos/
│   └── PaymentReadModelHelper.cs
│
├── BrewUp.Payment.Infrastructure/
│   └── InfrastructureHelper.cs
│
├── BrewUp.Payment.Facade/
│   ├── Acl/
│   ├── Endpoints/
│   ├── IPaymentFacade.cs
│   ├── PaymentFacade.cs
│   └── PaymentFacadeHelper.cs
│
└── BrewUp.Payment.Tests/
    ├── Architecture/
    └── Domain/
```

---

## 8. Custom Spec Kit commands

The harness adds six custom commands.

They live under:

```text
.github/agents/
```

Each file follows the GitHub Copilot agent format:

```text
<command-name>.brewup.speckit.agent.md
```

---

### 8.1 `speckit.brewup.load-domain-context`

**File**

```text
.github/agents/load-domain-context.brewup.speckit.agent.md
```

**When it runs**

```text
before_specify
```

**Purpose**

Prepare the domain context before `/speckit.specify`.

This command loads the BrewUp Sales Order domain context and makes explicit:

- Sales Order language;
- Payment Authorization;
- Stock Reservation;
- external decision references;
- ownership rules;
- forbidden responsibilities;
- open questions.

It does not generate code.

It does not generate the specification.

It prepares the reasoning environment.

**Main value**

It prevents the first specification from starting with generic e-commerce assumptions.

Without this command, the model may interpret the feature as a simple order-confirmation workflow and embed Payment and Warehouse behavior inside Sales.

---

### 8.2 `speckit.brewup.domain-guard`

**File**

```text
.github/agents/domain-guard.brewup.speckit.agent.md
```

**When it runs**

```text
after_specify
```

**Purpose**

Validate the generated specification against BrewUp domain ownership rules.

It checks whether `spec.md` respects the `BC-###` rules.

**What it checks**

The guard checks that:

- Sales owns Sales Order lifecycle;
- Payment owns payment authorization outcomes;
- Warehouse owns stock reservation outcomes;
- Sales stores external decision references;
- Sales does not embed Payment or Warehouse models;
- Sales does not authorize payment;
- Sales does not reserve stock;
- unresolved business decisions remain visible.

**Main value**

It turns domain review into a repeatable quality gate.

**Example failure**

```text
CRITICAL — Bounded-context ownership violation

The specification assigns payment authorization and physical stock mutation to the Sales Order aggregate.

Conflicts:

BC-003
Payment owns authorization outcomes.

BC-007
Warehouse owns physical stock and stock reservations.

BC-009
Sales may store external decision references but must not embed Payment or Warehouse domain models.
```

---

### 8.3 `speckit.brewup.plan-readiness`

**File**

```text
.github/agents/plan-readiness.brewup.speckit.agent.md
```

**When it runs**

```text
before_plan
```

**Purpose**

Decide whether the specification is ready for technical planning.

This command checks whether the specification contains enough explicit domain information to generate a plan safely.

**What it checks**

It verifies that `spec.md` contains:

- ubiquitous language;
- domain ownership;
- bounded-context rules;
- external decision references;
- confirmation invariant;
- open questions;
- out-of-scope decisions.

It blocks planning when the plan would need to invent business decisions.

**Main value**

It prevents `/speckit.plan` from being generated from an underspecified domain model.

**Important nuance**

Open questions do not automatically block planning.

They block planning only when the plan would need to resolve them silently in order to proceed.

---

### 8.4 `speckit.brewup.plan-guard`

**File**

```text
.github/agents/plan-guard.brewup.speckit.agent.md
```

**When it runs**

```text
after_plan
```

**Purpose**

Detect bounded-context drift in the generated implementation plan.

The plan may still look technically reasonable while reintroducing the original wrong design.

**What it checks**

It checks whether the plan reintroduces patterns such as:

```text
SalesOrder contains Payment.
SalesOrder contains WarehouseReservation.
SalesOrder.Confirm() authorizes payment.
SalesOrder.Confirm() reserves stock.
```

It allows coordination mechanisms such as:

- application service;
- saga;
- process manager;
- message handler;
- workflow;
- agent-assisted coordinator.

But only if decision ownership remains unchanged.

**Main value**

It shows the key SDD lesson:

```text
SDD does not guarantee obedience.
It makes architectural drift inspectable.
```

---

### 8.5 `speckit.brewup.task-guard`

**File**

```text
.github/agents/task-guard.brewup.speckit.agent.md
```

**When it runs**

```text
after_tasks
```

**Purpose**

Validate generated tasks before implementation begins.

This command checks whether `tasks.md` turns external decisions or unresolved policies into implementation work inside Sales.

**Valid task example**

```markdown
- [ ] Add `PaymentAuthorizationId` to SalesOrder as an external decision reference.
```

**Invalid task example**

```markdown
- [ ] Authorize payment from SalesOrder.Confirm().
```

**Main value**

It protects the implementation backlog from turning architectural drift into code.

---

### 8.6 `speckit.brewup.module-structure-guard`

**File**

```text
.github/agents/module-structure-guard.brewup.speckit.agent.md
```

**When it runs**

```text
after_plan
after_tasks
```

**Purpose**

Validate BrewUp module structure against architecture memory.

This is the guard introduced after the first custom workflow showed a gap:

```text
Payment was recognized as an authority,
but no Payment module was created.
```

**What it checks**

It checks:

- whether Payment is in scope;
- whether `src/Payment` exists or is planned;
- whether Payment uses the standard BrewUp project structure;
- whether Payment contracts are placed in `BrewUp.Payment.SharedKernel`;
- whether commands and events are not placed in Sales;
- whether the module is added to the solution;
- whether the module is registered in the REST host;
- whether architecture tests are included.

**Main value**

It protects the solution structure from architectural drift.

**Expected failure**

```text
CRITICAL — Missing module structure

The specification introduces Payment as a separate authority, but the implementation plan does not create a Payment module.

Violates:
AR-001 — Bounded contexts must be explicit modules.
AR-002 — Standard module project structure.
AR-016 — Business authority must not be collapsed into another module.
```

---

## 9. `extensions.yml`

### Purpose

`extensions.yml` wires the custom commands into the Spec Kit workflow.

It makes the harness visible as a sequence of quality gates.

### Final workflow

```text
before_specify
  -> speckit.brewup.load-domain-context

after_specify
  -> speckit.brewup.domain-guard
  -> speckit.agent-context.update

before_plan
  -> speckit.brewup.plan-readiness

after_plan
  -> speckit.brewup.plan-guard
  -> speckit.brewup.module-structure-guard
  -> speckit.agent-context.update

after_tasks
  -> speckit.brewup.task-guard
  -> speckit.brewup.module-structure-guard
```

### Updated `extensions.yml`

```yaml
installed:
- agent-context
- brewup-sdd

settings:
  auto_execute_hooks: true

hooks:
  before_specify:
  - extension: brewup-sdd
    command: speckit.brewup.load-domain-context
    enabled: true
    optional: true
    priority: 5
    prompt: Execute speckit.brewup.load-domain-context?
    description: Load BrewUp Sales Order domain context before specification
    condition: null

  after_specify:
  - extension: brewup-sdd
    command: speckit.brewup.domain-guard
    enabled: true
    optional: true
    priority: 5
    prompt: Execute speckit.brewup.domain-guard?
    description: Validate Sales, Payment, and Warehouse ownership after specification
    condition: null

  - extension: agent-context
    command: speckit.agent-context.update
    enabled: true
    optional: true
    priority: 10
    prompt: Execute speckit.agent-context.update?
    description: Refresh agent context after specification
    condition: null

  before_plan:
  - extension: brewup-sdd
    command: speckit.brewup.plan-readiness
    enabled: true
    optional: true
    priority: 5
    prompt: Execute speckit.brewup.plan-readiness?
    description: Check whether the Sales Order specification is ready for planning
    condition: null

  after_plan:
  - extension: brewup-sdd
    command: speckit.brewup.plan-guard
    enabled: true
    optional: true
    priority: 5
    prompt: Execute speckit.brewup.plan-guard?
    description: Detect bounded-context drift in the generated plan
    condition: null

  - extension: brewup-sdd
    command: speckit.brewup.module-structure-guard
    enabled: true
    optional: true
    priority: 6
    prompt: Execute speckit.brewup.module-structure-guard?
    description: Validate BrewUp module structure and architecture rules after planning
    condition: null

  - extension: agent-context
    command: speckit.agent-context.update
    enabled: true
    optional: true
    priority: 10
    prompt: Execute speckit.agent-context.update?
    description: Refresh agent context after planning
    condition: null

  after_tasks:
  - extension: brewup-sdd
    command: speckit.brewup.task-guard
    enabled: true
    optional: true
    priority: 5
    prompt: Execute speckit.brewup.task-guard?
    description: Validate generated tasks against Sales Order ownership and unresolved policy rules
    condition: null

  - extension: brewup-sdd
    command: speckit.brewup.module-structure-guard
    enabled: true
    optional: true
    priority: 6
    prompt: Execute speckit.brewup.module-structure-guard?
    description: Validate BrewUp module structure and architecture rules after task generation
    condition: null
```

---

## 10. Why the module structure guard runs twice

`module-structure-guard` runs after both plan and tasks.

### After plan

It checks whether the implementation plan creates the right module structure.

If the plan is wrong, task generation should not start.

Example failure:

```text
Payment is in scope, but plan.md does not mention src/Payment.
```

### After tasks

It checks whether the task list preserves the architecture.

Even if the plan is correct, tasks may still drift.

Example failure:

```text
tasks.md creates Payment commands under Sales.SharedKernel.
```

Running the guard twice catches both kinds of drift.

---

## 11. Relationship between the guards

The guards are intentionally layered.

```text
load-domain-context
  prepares the reasoning environment.

domain-guard
  checks the specification.

plan-readiness
  checks whether planning can start.

plan-guard
  checks domain ownership in the generated plan.

module-structure-guard
  checks physical architecture structure.

task-guard
  checks implementation tasks.

module-structure-guard
  checks task-level architecture structure again.
```

The important distinction is:

```text
plan-guard
  catches incorrect decision ownership.

module-structure-guard
  catches incorrect solution structure.
```

Both are needed.

---

## 12. Example: one failure, two interpretations

Suppose the plan says:

```text
Implement payment authorization inside Sales.
```

This violates domain ownership:

```text
BC-003 — Payment owns authorization outcomes.
```

It also violates architecture structure:

```text
AR-016 — Business authority must not be collapsed into another module.
```

The first violation says:

```text
Sales must not own this decision.
```

The second violation says:

```text
Payment must live in the Payment module.
```

Together they turn a vague design concern into two precise corrections.

---

## 13. Suggested demo sequence

For the conference demo, the harness can be shown conceptually without explaining every file in depth.

A useful sequence is:

```text
1. Show the wrong generated model.
2. Show the domain carrier.
3. Run or show /speckit.specify.
4. Show the generated spec.
5. Show /speckit.clarify output.
6. Show the corrected external-reference model.
7. Show the inconsistent plan.
8. Show plan-guard finding.
9. Show module-structure-guard finding.
10. Explain that SDD made the drift inspectable.
```

The talk should not become a command tutorial.

The repository can contain the full harness for participants who want to inspect it later.

---

## 14. Key takeaways

- A prompt is temporary context.
- A specification is durable context.
- A domain carrier bootstraps the first specification.
- A constitution defines non-negotiable project principles.
- Architecture memory defines physical structure.
- `BC-###` rules protect decision ownership.
- `AR-###` rules protect solution structure.
- A generated plan may violate either or both.
- A generated task list may reintroduce drift even after a correct plan.
- SDD does not make the model obedient.
- SDD makes violations nameable, inspectable, and correctable.

---

## 15. Final principle

The final principle of the harness is:

```text
Do not let the model stop at conceptual correctness.
```

If the model identifies a business authority, it must also place that authority correctly in the solution.

```text
Correct ownership without correct structure is still architectural drift.
```
