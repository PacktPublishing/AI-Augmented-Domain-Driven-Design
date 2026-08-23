using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class RequestBeersAvailabilityChecked(IntegrationId aggregateId, Guid correlationId,
    StockReservationId? stockReservationId,
    IEnumerable<ItemRequested> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public StockReservationId? StockReservationId { get; private set; } = stockReservationId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}
