using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.MasterData;

public sealed class BeerDeleted(BeerId aggregateId) : IntegrationEvent(aggregateId)
{
}
