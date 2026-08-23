using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class RequestBeersAvailabilityFailed(IntegrationId aggregateId, Guid correlationId,
    IEnumerable<ItemRequested> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}