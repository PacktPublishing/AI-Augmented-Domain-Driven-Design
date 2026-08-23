using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class StockReservationFailed(IntegrationId aggregateId, Guid correlationId,
    string reason, IEnumerable<ItemRequested>? rows = null) : IntegrationEvent(aggregateId, correlationId)
{
    public string Reason { get; private set; } = reason;
    public IList<ItemRequested> Rows { get; private set; } = rows?.ToList() ?? [];
}
