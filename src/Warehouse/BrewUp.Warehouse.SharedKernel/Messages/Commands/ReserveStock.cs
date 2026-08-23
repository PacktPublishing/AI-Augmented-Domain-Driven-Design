using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands;

public sealed class ReserveStock(WarehouseId aggregateId,
    Guid correlationId,
    string salesOrderId,
    IEnumerable<ItemRequested> rows) : Command(aggregateId, correlationId)
{
    public string SalesOrderId { get; } = salesOrderId;
    public IEnumerable<ItemRequested> Rows { get; } = rows;
}
