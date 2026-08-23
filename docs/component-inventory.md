# Component Inventory

| Component category | Location | Pattern |
| --- | --- | --- |
| Composition root | `BrewUp.Rest` | Registers module objects and calls their configuration hooks |
| Minimal API endpoints | `*.Facade/Endpoints` | HTTP-to-facade adapters |
| Facades and ACL handlers | `*.Facade` | Translate external contracts to commands/events |
| Aggregates | `*.Domain/Entities` | Event-sourced business state |
| Command handlers | `*.Domain/CommandHandlers` | Load, invoke, and persist aggregates |
| Saga orchestrator | `Sagas.Domain/Orchestrators` | Long-running cross-module coordination |
| Shared kernels | `*.SharedKernel` | Module commands, events, IDs, and value objects |
| Cross-module contracts | `BrewUp.Shared` | Integration events, external JSON contracts, common IDs/types |
| Projections | `*.ReadModel/EventHandlers` | Update query models from domain events |
| Query services | `*.ReadModel/Services` | Query and persist projection DTOs |
| Infrastructure | `BrewUp.Infrastructure`, `*.Infrastructure` | EventStoreDB, RabbitMQ, MongoDB, module persisters |
| Tests | `*.Tests` | xUnit specification, HTTP integration, and NetArchTest tests |

