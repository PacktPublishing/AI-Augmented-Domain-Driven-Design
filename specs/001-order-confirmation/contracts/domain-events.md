# Contracts — Domain Events

Domain events are Sales-owned facts raised by the `SalesOrder` aggregate and applied to mutate its state (Muflone event sourcing).

## `SalesOrderConfirmed` *(Sales-owned domain event)*

- **Location**: `BrewUp.Sales.SharedKernel.Messages.Events.SalesOrderConfirmed`
- **Raised by**: `SalesOrder.Confirm(...)` when both external decision references are present.
- **Applied by**: `SalesOrder.Apply(SalesOrderConfirmed)` → records references, sets status to `Confirmed`.
- **Purpose**: Announce that the Sales Order has been confirmed so other contexts/read models may react (FR-008).

| Field | Type | Notes |
|-------|------|-------|
| `AggregateId` | `SalesOrderId` | Confirmed Sales Order. |
| `PaymentAuthorizationId` | `PaymentAuthorizationId` | Recorded evidence. |
| `StockReservationId` | `StockReservationId` | Recorded evidence. |
| `CorrelationId` | `Guid` | Correlation. |

**Rules**:
- MUST only be raised when both references are present (BC-010 / INV-1).
- At most one per aggregate (idempotency / INV-2).
- Carries only id references — never Payment/Warehouse models (BC-002/BC-009).

> Note: `BrewUp.Shared.Messages.Events.Sagas.SalesOrderConfirmed` already exists as a saga-level integration event with a different shape (order header + rows). The **new** event above is the **Sales-owned domain event** inside the Sales context and is distinct from that saga integration event. Keep the names disambiguated by namespace; if a cross-context confirmation announcement is needed, reuse/extend the existing shared integration event rather than duplicating it.
