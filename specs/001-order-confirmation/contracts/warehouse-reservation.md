# Contract: Stock reservation (Warehouse module)

Warehouse owns physical stock and the reservation decision (BC-005, BC-006, BC-007). Partial reservation is allowed (OQ-2): Warehouse decides the reservable subset.

## Command — `ReserveStock`

Location: `src/Warehouse/BrewUp.Warehouse.SharedKernel/Messages/Commands/ReserveStock.cs`

```csharp
public sealed class ReserveStock(WarehouseId aggregateId,
    Guid correlationId,
    string salesOrderId,
    IEnumerable<ItemRequested> rows) : Command(aggregateId, correlationId)
{
    public string SalesOrderId { get; } = salesOrderId;
    public IEnumerable<ItemRequested> Rows { get; } = rows;
}
```

Handler `ReserveStockCommandHandler` (Warehouse.Domain): load `Availability` → call `ReserveStock(...)` → save.

## Domain events (Warehouse.SharedKernel/Messages/Events)

```csharp
public sealed class StockReserved(WarehouseId aggregateId,
    Guid correlationId,
    StockReservationId stockReservationId,
    string salesOrderId,
    IEnumerable<ItemRequested> reservedRows) : DomainEvent(aggregateId, correlationId)
{ /* stockReservationId, salesOrderId, reservedRows (the reserved subset) */ }

public sealed class StockReservationRejected(WarehouseId aggregateId,
    Guid correlationId, string salesOrderId, string reason) : DomainEvent(aggregateId, correlationId)
{ /* salesOrderId, reason */ }
```

## Saga-facing integration events (BrewUp.Shared/Messages/Events/Sagas)

Published by `Warehouse.ReadModel`; consumed by the saga.

```csharp
public sealed class StockReservedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId, string stockReservationId, string salesOrderId,
    IEnumerable<ItemRequested> reservedRows) : IntegrationEvent(aggregateId, correlationId);

public sealed class StockReservationRejectedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId, string salesOrderId, string reason) : IntegrationEvent(aggregateId, correlationId);
```

**New ID** `StockReservationId : DomainId` in `Warehouse.SharedKernel/CustomTypes`.

**Ownership**: Sales never reserves, releases, or decrements stock. Reservation expiry (OQ-4) is **not** implemented here.
