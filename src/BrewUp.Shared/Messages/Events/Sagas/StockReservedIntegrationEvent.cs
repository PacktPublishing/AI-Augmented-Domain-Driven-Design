using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class StockReservedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId,
    string stockReservationId,
    string salesOrderId,
    IEnumerable<ItemRequested> reservedRows) : IntegrationEvent(aggregateId, correlationId)
{
    public string StockReservationId { get; private set; } = stockReservationId;
    public string SalesOrderId { get; private set; } = salesOrderId;
    public List<ItemRequested> ReservedRows { get; private set; } = reservedRows.ToList();
}
