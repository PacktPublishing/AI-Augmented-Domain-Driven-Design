# Contracts — Commands

Commands are imperative messages handled by a single owning context. Shapes mirror the existing Muflone conventions (`Command`/`CommandHandlerAsync<T>`, strongly-typed ids, `correlationId`).

## `ConfirmSalesOrder` *(Sales-owned command)*

- **Location**: `BrewUp.Sales.SharedKernel.Messages.Commands.ConfirmSalesOrder`
- **Sent by**: `SalesOrderSagaOrchestrator` (Sagas context) when both evidence references are present.
- **Handled by**: `BrewUp.Sales.Domain.CommandHandlers.ConfirmSalesOrderCommandHandler`.
- **Purpose**: Instruct Sales to transition the Sales Order to `Confirmed` using the supplied external decision references.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `AggregateId` | `SalesOrderId` | yes | Target Sales Order. |
| `PaymentAuthorizationId` | `PaymentAuthorizationId` | yes | Payment evidence (BC-004). |
| `StockReservationId` | `StockReservationId` | yes | Warehouse evidence (BC-006). |
| `CorrelationId` / `MessageId` | `Guid` | yes | Correlation across the saga flow. |

**Handler behavior**: load `SalesOrder` by `AggregateId`; call `aggregate.Confirm(paymentAuthorizationId, stockReservationId, correlationId)`; save. Handler performs no invariant logic itself (invariant lives in the aggregate, Principle I).

**Validation / rules**:
- Both ids MUST be non-empty (also enforced by the aggregate; the command carries them as evidence).
- Re-delivery MUST be safe (aggregate `Confirm` is idempotent).

## Reservation request *(Warehouse-owned command — contract reference)*

A reservation **request** is emitted by the saga toward Warehouse so Warehouse can produce the `StockReserved`/`StockReservationFailed` outcome. The producing handler is **out of scope** for this feature (D2); only the contract is defined here so the saga can request it.

- **Suggested name**: `ReserveStock` (Warehouse `SharedKernel`), carrying `WarehouseId`, the Sales Order reference, and the requested rows (`IEnumerable<SalesOrderRowJson>` or `IEnumerable<ItemRequested>`), plus `correlationId`.
- Sales/Sagas MUST NOT implement the reservation; Warehouse owns it (BC-005/BC-007).

> Payment authorization is likewise requested through a boundary toward the Payment authority; the request/producer is out of scope (D1). Only the outcome contracts in [integration-events.md](integration-events.md) are required for Sales/Sagas to react.
