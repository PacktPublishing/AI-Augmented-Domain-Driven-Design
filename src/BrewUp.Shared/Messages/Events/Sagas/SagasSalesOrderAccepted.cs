using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public class SagasSalesOrderAccepted(IntegrationId aggregateId, Guid correlationId) 
    : IntegrationEvent(aggregateId, correlationId) 
{
}