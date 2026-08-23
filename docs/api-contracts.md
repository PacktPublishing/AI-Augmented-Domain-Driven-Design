# API Contracts

## Public Minimal APIs

| Area | Method and route | Purpose |
| --- | --- | --- |
| Sagas | `POST /v1/sagas/` | Start the sales-order saga |
| Sales | `POST /v1/sales/` | Create a sales order |
| Sales | `PATCH /v1/sales/rows/{orderId}` | Add beers to an order |
| Sales | `PUT /v1/sales/{orderId}` | Close/place an order |
| Sales | `GET /v1/sales/` | Query sales orders |
| Sales | `GET /v1/sales/{orderId}` | Query order details |
| Warehouse | `GET /v1/warehouse/` | Query shipment orders |
| Warehouse | `POST /v1/warehouse/` | Add warehouse stock |
| Purchases | `POST /v1/purchases/` | Submit a purchase order |
| MasterData | `/v1/masterdata/customers` | Customer CRUD/query endpoints |
| MasterData | `/v1/masterdata/beers` | Beer create/query endpoints |
| MasterData | `/v1/masterdata/suppliers` | Supplier create/query endpoints |
| MasterData | `/v1/masterdata/warehouse` | Warehouse create/query endpoints |
| Dashboards | `GET /v1/dashboards/customers` | Sales-by-customer projection |
| Dashboards | `GET /v1/dashboards/products` | Sales-by-product projection |

## Internal Contracts

Internal commands and events are C# message types under each module's `SharedKernel` and under `BrewUp.Shared/Messages`. Correlation IDs link the order saga across modules.

No public confirmation endpoint currently exists. The existing flow confirms/accepts asynchronously through saga messages.

