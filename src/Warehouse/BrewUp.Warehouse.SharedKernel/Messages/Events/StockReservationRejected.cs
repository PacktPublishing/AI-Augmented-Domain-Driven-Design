using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events;

public sealed class StockReservationRejected(WarehouseId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string reason) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string Reason { get; private set; } = reason;
}
