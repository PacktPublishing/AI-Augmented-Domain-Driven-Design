using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class StockReserved(IntegrationId aggregateId, Guid correlationId,
    string stockReservationId, IEnumerable<ItemRequested> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public string StockReservationId { get; private set; } = stockReservationId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}
