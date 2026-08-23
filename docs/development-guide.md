# Development Guide

## Prerequisites

- .NET 10 SDK
- EventStoreDB reachable at the configured connection string
- RabbitMQ
- MongoDB

Default local values are in `src/BrewUp.Rest/appsettings.json`.

## Build and Test

```powershell
dotnet build src/BrewUp.slnx
dotnet test src/BrewUp.slnx
```

## Run

```powershell
dotnet run --project src/BrewUp.Rest/BrewUp.Rest.csproj
```

The REST host registers all modules from `Program.cs`. Scalar/OpenAPI support is configured by the host's OpenAPI module.

## Testing Style

- xUnit is the test framework.
- Muflone specification tests exercise domain commands and emitted events.
- NetArchTest rules protect module layering.
- `BrewUp.Rest.Tests` provides composition and HTTP integration coverage.

## Configuration

Runtime settings are grouped under `BrewUp` for MongoDB, EventStoreDB, RabbitMQ, and SQL Server. Secrets and environment-specific connection strings should override repository defaults outside source control.

