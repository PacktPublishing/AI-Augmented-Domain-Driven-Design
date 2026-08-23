using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands;

/// <summary>
/// Contract-only command. The handler will be implemented when Warehouse integrates with the saga.
/// Instructs the Warehouse to reserve stock for the items in a confirmed Sales Order.
/// </summary>
public sealed class ReserveStock(WarehouseId warehouseId, Guid correlationId,
    SalesOrderId salesOrderId, IEnumerable<ItemRequested> rows) : Command(warehouseId, correlationId)
{
    public SalesOrderId SalesOrderId { get; } = salesOrderId;
    public IList<ItemRequested> Rows { get; } = rows.ToList();
}
