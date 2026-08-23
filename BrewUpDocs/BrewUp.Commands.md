# BrewUp Spec Kit Commands — Summary

## `speckit.brewup.load-domain-context`

**Purpose:** Prepare the domain context before creating the specification.

This command loads the BrewUp Sales Order domain knowledge before `/speckit.specify` runs. It makes the key domain concepts explicit: Sales Order, Payment Authorization, Stock Reservation, external decision references, ownership rules, forbidden responsibilities, and open questions.

It does not generate the specification. Its role is to prepare the reasoning environment so that the specification is created from the correct domain assumptions.

**Main value:** Prevents the AI from starting with a generic e-commerce interpretation of the problem.

---

## `speckit.brewup.domain-guard`

**Purpose:** Validate the generated specification against domain ownership rules.

This command runs after `/speckit.specify`. It checks whether the generated `spec.md` respects the bounded-context rules for Sales, Payment, and Warehouse.

It verifies that Sales owns the Sales Order lifecycle, Payment owns authorization outcomes, and Warehouse owns physical stock and reservation outcomes. It also checks that the Sales Order stores external decision references such as `PaymentAuthorizationId` and `StockReservationId`, instead of embedding Payment or Warehouse models.

**Main value:** Turns domain review into a repeatable quality gate.

---

## `speckit.brewup.plan-readiness`

**Purpose:** Decide whether the specification is ready for technical planning.

This command runs before `/speckit.plan`. It checks whether the specification contains enough explicit domain information to generate a plan safely.

It verifies that the specification includes ubiquitous language, ownership rules, external decision references, the confirmation invariant, and unresolved open questions. It blocks planning if the plan would need to invent business decisions.

**Main value:** Prevents the implementation plan from being generated from an underspecified domain model.

---

## `speckit.brewup.plan-guard`

**Purpose:** Detect architectural drift in the generated plan.

This command runs after `/speckit.plan`. It checks whether the plan has drifted away from the specification.

The main risk it detects is the plan reintroducing the original wrong design: a Sales Order aggregate that contains Payment or WarehouseReservation, or a `Confirm()` method that authorizes payment and reserves stock directly.

It allows coordination mechanisms such as sagas, process managers, application services, workflows, or agents, but only if they preserve decision ownership.

**Main value:** Shows that SDD does not make AI infallible; it makes architectural drift inspectable.

---

## `speckit.brewup.task-guard`

**Purpose:** Validate generated implementation tasks before coding starts.

This command runs after `/speckit.tasks`. It checks whether the generated task list respects the specification and plan.

It prevents tasks from implementing Payment or Warehouse responsibilities inside Sales. It also ensures that unresolved domain decisions remain blocked tasks instead of becoming hidden implementation defaults.

For example, a task like “add `PaymentAuthorizationId` to SalesOrder” is valid. A task like “authorize payment inside SalesOrder.Confirm()`” is invalid.

**Main value:** Protects the implementation backlog from turning architectural drift into code.

---

# Overall Workflow

```text
before_specify  -> speckit.brewup.load-domain-context
after_specify   -> speckit.brewup.domain-guard
before_plan     -> speckit.brewup.plan-readiness
after_plan      -> speckit.brewup.plan-guard
after_tasks     -> speckit.brewup.task-guard
```

Together, these commands create a controlled SDD workflow:

1. Load the domain context.
2. Generate the specification.
3. Check the specification against domain rules.
4. Verify that the specification is ready for planning.
5. Generate the plan.
6. Check the plan for architectural drift.
7. Generate tasks.
8. Check tasks before implementation.

The goal is not to make AI deterministic.

The goal is to make non-negotiable domain decisions explicit, durable, and verifiable.
