# BrewUp Project Documentation Index

## Project Overview

- **Type:** backend monorepo with seven logical parts
- **Primary language:** C# on .NET 10
- **Architecture:** modular, event-driven backend with event-sourced aggregates and CQRS projections
- **Entry point:** `src/BrewUp.Rest/Program.cs`

## Quick Reference by Part

| Part | Root | Architecture |
| --- | --- | --- |
| Platform | `src/BrewUp.Rest`, `src/BrewUp.Infrastructure`, `src/BrewUp.Shared` | [Platform](./architecture-platform.md) |
| Sales | `src/Sales` | [Sales](./architecture-sales.md) |
| Warehouse | `src/Warehouse` | [Warehouse](./architecture-warehouse.md) |
| Sagas | `src/Sagas` | [Sagas](./architecture-sagas.md) |
| MasterData | `src/MasterData` | [MasterData](./architecture-master-data.md) |
| Purchases | `src/Purchases` | [Purchases](./architecture-purchases.md) |
| Dashboards | `src/Dashboards` | [Dashboards](./architecture-dashboards.md) |

## Generated Documentation

- [Project overview](./project-overview.md)
- [Integration architecture](./integration-architecture.md)
- [Source-tree analysis](./source-tree-analysis.md)
- [API contracts](./api-contracts.md)
- [Data models](./data-models.md)
- [Component inventory](./component-inventory.md)
- [Development guide](./development-guide.md)
- [Machine-readable project parts](./project-parts.json)

## Existing Documentation Used

- [Repository README](../README.md)
- [BrewUp commands](../BrewUpDocs/BrewUp.Commands.md)

## Experiment Material Excluded as Domain Authority

The following files are present in the working tree but were not used to import decisions from the earlier harness:

- [Spec Kit harness guide](../BrewUpDocs/brewup-spec-kit-harness.md)
- [Previous BMAD harness design](./superpowers/specs/2026-07-25-brewup-bmad-copilot-harness-design.md)
- [Previous BMAD harness plan](./superpowers/plans/2026-07-25-brewup-bmad-copilot-harness.md)

## Getting Started

1. Read the [project overview](./project-overview.md).
2. For the order-confirmation feature, read the Sales, Warehouse, and Sagas architecture documents plus the integration architecture.
3. Build with `dotnet build src/BrewUp.slnx`.
4. Test with `dotnet test src/BrewUp.slnx`.

This index is the brownfield context entry point for subsequent native BMAD planning workflows.

