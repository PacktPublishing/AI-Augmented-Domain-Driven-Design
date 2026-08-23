# Data Models

## Event-Sourced Aggregates

| Module | Aggregate | Current role |
| --- | --- | --- |
| Sales | `SalesOrder` | Order number/date, customer, delivery date, rows, and status |
| Warehouse | `Availability` | Warehouse/beer quantity and stock additions |
| Warehouse | `Shipment` | Pending shipment for a sales order |
| Sagas | `SalesOrderSaga` | Correlation and process state for an order |

Other modules use domain services and/or projections following the same module boundaries.

## Read Models

- Sales: orders, order summaries, customers, beers, sales by customer.
- Warehouse: availability, shipments, warehouses, beers.
- Sagas: saga status projections.
- MasterData: customers, beers, suppliers, warehouses.
- Purchases: suppliers and beers.
- Dashboards: sales by customer/product and received-message tracking.

## Persistence

- EventStoreDB stores event streams for event-sourced aggregates.
- MongoDB stores most read models, separated by module-specific persisters.
- The repository contains no schema-migration system; documents are shaped by DTO classes and event handlers.

## Missing Models for Order Confirmation

- A durable payment-authorization outcome.
- A Warehouse-owned stock reservation with an identifier and reserved rows.
- Sales evidence/state representing successful payment authorization and reservation.

