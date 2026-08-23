# Source Tree Analysis

```text
src/
├── BrewUp.slnx                         # Solution and module grouping
├── BrewUp.Rest/                        # ASP.NET Core composition root
├── BrewUp.Rest.Tests/                  # HTTP and composition tests
├── BrewUp.Infrastructure/              # EventStoreDB, RabbitMQ, MongoDB setup
├── BrewUp.Shared/                      # Cross-module contracts, IDs, value objects
├── BrewUp.Shared.Tests/                # Shared architecture-test helpers
├── Sales/
│   ├── BrewUp.Sales.Domain/            # SalesOrder aggregate and command handlers
│   ├── BrewUp.Sales.SharedKernel/       # Sales commands, events, IDs, enums
│   ├── BrewUp.Sales.Facade/            # HTTP API and integration-event adapters
│   ├── BrewUp.Sales.ReadModel/         # MongoDB projections and queries
│   ├── BrewUp.Sales.Infrastructure/     # Sales projection persistence
│   └── BrewUp.Sales.Tests/              # Domain and architecture tests
├── Warehouse/                          # Same layered module pattern
├── Sagas/                              # Saga aggregate, orchestrator, projections
├── MasterData/                         # Master-data domain services and projections
├── Purchases/                          # Purchasing domain and projections
└── Dashboards/                         # Reporting projections and query endpoints
```

## Entry Points

- `src/BrewUp.Rest/Program.cs` registers and configures all modules.
- Each `BrewUp.*.Facade/Endpoints` folder maps minimal API endpoints.
- Each module's `*Helper.cs` files register command and event handlers.
- `SalesOrderSagaOrchestrator` is the central feature-relevant process coordinator.

## Layering Pattern

Business modules generally separate Domain, SharedKernel, Facade, Infrastructure, ReadModel, Tests, and—where needed—Entities. Domain projects use event-sourced aggregates. Facades translate HTTP or integration messages into module commands. ReadModel projects consume domain events into MongoDB-backed projections.

