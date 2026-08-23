# BrewUp Project Overview

## Executive Summary

BrewUp is a modular .NET backend used as a Domain-Driven Design and event-driven architecture demonstration. A single ASP.NET Core host composes six business modules plus shared infrastructure. Commands and domain/integration events flow through Muflone, RabbitMQ, and EventStoreDB; MongoDB supplies module-specific read models.

## Repository Classification

- Type: backend monorepo
- Runtime: .NET 10 / C#
- Host: ASP.NET Core minimal APIs
- Architectural style: modular monolith at deployment time, event-driven bounded modules internally
- Persistence: EventStoreDB for aggregates and MongoDB for projections; Dashboards also references SQL-oriented infrastructure
- Messaging: Muflone with RabbitMQ transport

## Logical Parts

| Part | Responsibility |
| --- | --- |
| Platform | REST composition root, shared contracts/types, messaging and persistence configuration |
| Sales | Sales-order lifecycle and sales projections |
| Warehouse | Availability, inventory additions, and shipment preparation |
| Sagas | Long-running sales-order coordination |
| MasterData | Customers, beers, suppliers, warehouses, and current customer-budget verification |
| Purchases | Purchase-order submission and purchasing projections |
| Dashboards | Cross-module reporting projections |

## Feature-Relevant Brownfield Facts

- The existing order workflow starts a `SalesOrderSaga`.
- MasterData currently verifies a customer's budget; there is no Payment module or payment-authorization aggregate.
- Warehouse answers beer-availability requests from a MongoDB projection.
- Sales accepts an order after an availability result, but the current handler ignores an error result.
- Warehouse contains a comment identifying the missing stock-reservation command.
- Shipment preparation currently starts from a sales-order-created integration event, independently of confirmation.

## Documentation Map

- [Architecture and integrations](./integration-architecture.md)
- [Source tree](./source-tree-analysis.md)
- [API contracts](./api-contracts.md)
- [Data models](./data-models.md)
- [Component inventory](./component-inventory.md)
- [Development guide](./development-guide.md)

