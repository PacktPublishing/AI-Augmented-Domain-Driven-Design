using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaSalesOrderSuccessfullyCompleted(IntegrationId aggregateId, Guid correlationId) 
    : DomainEvent(aggregateId, correlationId)
{
}