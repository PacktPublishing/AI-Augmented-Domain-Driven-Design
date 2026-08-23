# Contracts — Integration Events

Integration events cross bounded-context boundaries via the bus (`IntegrationEvent(aggregateId, correlationId)` base, declared in `BrewUp.Shared`). For this feature, Sales/Sagas **react** to Payment and Warehouse outcomes; producing them is owned by those authorities (producers out of scope — D1/D2).

## `PaymentAuthorized` *(produced by Payment authority — reacted to by Sagas)*

- **Location**: `BrewUp.Shared.Messages.Events.Sagas.PaymentAuthorized`
- **Meaning**: Payment has authorized the specified amount for the order.

| Field | Type | Notes |
|-------|------|-------|
| `AggregateId` | `IntegrationId` | Correlates to the saga/order. |
| `CorrelationId` | `Guid` | Saga correlation. |
| `PaymentAuthorizationId` | `string` | The decision reference Sales will store as evidence (BC-004). |

## `PaymentAuthorizationFailed` *(produced by Payment authority)*

| Field | Type | Notes |
|-------|------|-------|
| `AggregateId` | `IntegrationId` | Correlation. |
| `CorrelationId` | `Guid` | Correlation. |
| `Reason` | `string` | Diagnostic; Sales does not interpret provider semantics (BC-003). |

**Effect**: saga records failure; Sales Order stays pre-confirmation (Q1/Q3, FR-014). No void/refund by Sales.

## `StockReserved` *(produced by Warehouse authority — reacted to by Sagas)*

- **Location**: `BrewUp.Shared.Messages.Events.Sagas.StockReserved`
- **Meaning**: Warehouse has reserved the requested beers for the order (durable fact, BC-006).

| Field | Type | Notes |
|-------|------|-------|
| `AggregateId` | `IntegrationId` | Correlation. |
| `CorrelationId` | `Guid` | Saga correlation. |
| `StockReservationId` | `string` | The decision reference Sales will store as evidence (BC-006). |
| `Rows` | `IEnumerable<ItemRequested>` | Reserved beers/quantities (optional, for traceability). |

## `StockReservationFailed` *(produced by Warehouse authority)*

| Field | Type | Notes |
|-------|------|-------|
| `AggregateId` | `IntegrationId` | Correlation. |
| `CorrelationId` | `Guid` | Correlation. |
| `Rows` | `IEnumerable<ItemRequested>` | Which beers could not be reserved (optional). |
| `Reason` | `string` | Diagnostic. |

**Effect**: saga records failure; Sales Order stays pre-confirmation. Payment owns any release/void (out of Sales scope, FR-014).

## Reaction map (Sagas orchestrator)

| Integration event | Orchestrator handler | Saga effect | Then |
|-------------------|----------------------|-------------|------|
| `PaymentAuthorized` | `HandleAsync(PaymentAuthorized)` | `MarkPaymentAuthorized` | if stock also reserved → send `ConfirmSalesOrder` |
| `StockReserved` | `HandleAsync(StockReserved)` | `MarkStockReserved` | if payment also authorized → send `ConfirmSalesOrder` |
| `PaymentAuthorizationFailed` | `HandleAsync(PaymentAuthorizationFailed)` | `MarkConfirmationFailed` | no Sales Order transition |
| `StockReservationFailed` | `HandleAsync(StockReservationFailed)` | `MarkConfirmationFailed` | no Sales Order transition |

> The `ConfirmSalesOrder` send is guarded so it fires at most once per saga, ensuring idempotent confirmation (FR-009, SC-004).
