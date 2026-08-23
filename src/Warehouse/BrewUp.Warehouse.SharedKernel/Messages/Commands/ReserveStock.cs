using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands;

public sealed class ReserveStock(
    StockReservationId aggregateId,
    WarehouseId warehouseId,
    SalesOrderId salesOrderId,
    IEnumerable<ItemRequested> rows,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public WarehouseId WarehouseId { get; } = warehouseId;
    public SalesOrderId SalesOrderId { get; } = salesOrderId;
    public IReadOnlyList<ItemRequested> Rows { get; } = rows.ToArray();
}
