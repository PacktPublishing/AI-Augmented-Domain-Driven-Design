# BrewUp Project Context

## Technology and architecture

BrewUp is a .NET 10 and C# modular monolith using Muflone, CQRS, Event
Sourcing, RabbitMQ, EventStore, and MongoDB. It follows Domain-Driven Design:
each business authority is an explicit bounded-context module and may not be
collapsed into another module for convenience.

Every module uses the standard project structure:

- `BrewUp.<ModuleName>.SharedKernel`
- `BrewUp.<ModuleName>.Domain`
- `BrewUp.<ModuleName>.ReadModel`
- `BrewUp.<ModuleName>.Infrastructure`
- `BrewUp.<ModuleName>.Facade`
- `BrewUp.<ModuleName>.Tests`

Allowed dependency directions are Facade to Domain, ReadModel, Infrastructure,
and SharedKernel; Domain to SharedKernel; ReadModel to SharedKernel;
Infrastructure to Domain and SharedKernel; and Tests to layers as needed.
Cross-module behavior uses explicit contracts and integration boundaries, never
another module's Domain or Infrastructure project.

## Implementation discipline

Implement behavior test-first with `CommandSpecification<T>`. Use
`Guid.CreateVersion7()` and `ConfigureAwait(false)` where applicable. Preserve
module boundaries and do not invent unresolved business policy.

## Governing documents

- [Sales Order Confirmation Governance](../.bmad-harness/governance/brewup-sales-order-confirmation.md)
- [Module Structure Governance](../.bmad-harness/governance/brewup-module-structure.md)
- [BMAD Gate Contract](../.bmad-harness/governance/gate-contract.md)

This `project-context.md` summarizes these documents but does not override
their numbered rules.
