using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events;

public sealed class StockReserved(WarehouseId aggregateId,
    Guid correlationId,
    StockReservationId stockReservationId,
    string salesOrderId,
    IEnumerable<ItemRequested> reservedRows) : DomainEvent(aggregateId, correlationId)
{
    public StockReservationId StockReservationId { get; private set; } = stockReservationId;
    public string SalesOrderId { get; private set; } = salesOrderId;
    public List<ItemRequested> ReservedRows { get; private set; } = reservedRows.ToList();
}
