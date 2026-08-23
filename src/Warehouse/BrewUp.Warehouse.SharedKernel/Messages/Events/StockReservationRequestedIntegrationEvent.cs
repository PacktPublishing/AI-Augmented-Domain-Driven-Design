using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events;

public sealed class StockReservationRequestedIntegrationEvent(
    StockReservationId aggregateId,
    WarehouseId warehouseId,
    SalesOrderId salesOrderId,
    IEnumerable<ItemRequested> rows,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public WarehouseId WarehouseId { get; } = warehouseId;
    public SalesOrderId SalesOrderId { get; } = salesOrderId;
    public IReadOnlyList<ItemRequested> Rows { get; } = rows.ToArray();
}
