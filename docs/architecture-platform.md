# Platform Architecture

## Summary

`BrewUp.Rest` is the ASP.NET Core composition root. It registers shared infrastructure and all business modules explicitly. `BrewUp.Shared` contains cross-module IDs, value objects, external DTOs, and integration events. `BrewUp.Infrastructure` configures EventStoreDB, RabbitMQ, and MongoDB.

## Key Decisions

- Single deployable host with modular internal boundaries.
- Minimal APIs at module facades.
- Muflone command/event dispatch.
- EventStoreDB write model and MongoDB projections.
- Shared contracts are transport contracts, not ownership of module behavior.

## Testing

`BrewUp.Rest.Tests` checks HTTP/composition behavior; `BrewUp.Shared.Tests` provides reusable architecture-test utilities.

