# Integration Architecture

## Runtime Topology

The application is deployed as one ASP.NET Core process, but module interaction is message-driven. The REST host is the composition root; commands and events are routed through Muflone. EventStoreDB persists event-sourced aggregates, RabbitMQ transports messages, and MongoDB stores read models.

## Current Sales-Order Flow

1. The Sagas API sends `StartSalesOrderSaga`.
2. `SalesOrderSagaStarted` triggers MasterData customer-budget verification.
3. `CustomerBudgetVerified` advances the saga and causes Sales to create/place the order.
4. `SalesOrderPlaced` causes the saga to request Warehouse availability.
5. Warehouse queries availability projections and publishes `RequestBeersAvailabilityChecked`.
6. The saga republishes an availability-checked integration event.
7. Sales currently sends `AcceptSalesOrder`; its handler does not enforce the failed availability result.
8. `SalesOrderAccepted` completes the saga.

## Feature-Relevant Gaps

- Payment authorization has no owning module, command, aggregate, or outcome event.
- Availability checking is a query, not an atomic stock reservation.
- Warehouse has no reservation aggregate or command.
- Confirmation is represented by the existing `Accepted` terminology, which is ambiguous for the requested feature.
- The availability handler returns after the first successful row and therefore may omit later requested beers.
- Shipment creation is coupled to `SalesOrderCreatedIntegrationEvent`, so it can occur before confirmation.

## Integration Boundaries

| From | To | Mechanism | Purpose |
| --- | --- | --- | --- |
| REST | Sagas | Command | Start order process |
| Sagas | MasterData | Integration event / command chain | Verify current customer budget |
| Sagas | Sales | Integration event | Create/place and later accept order |
| Sagas | Warehouse | Integration event | Check requested quantities |
| Domain modules | MongoDB projections | Domain-event handlers | Maintain query models |
| All modules | EventStoreDB/RabbitMQ | Muflone infrastructure | Persist aggregates and transport messages |

