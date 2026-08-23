using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events;

public sealed class StockReservationFailed(
    StockReservationId aggregateId,
    WarehouseId warehouseId,
    SalesOrderId salesOrderId,
    string reason,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public WarehouseId WarehouseId { get; } = warehouseId;
    public SalesOrderId SalesOrderId { get; } = salesOrderId;
    public string Reason { get; } = reason;
}
