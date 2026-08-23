using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public class SalesOrderSagaStartedIntegrationEvent(CustomerId aggregateId, Guid correlationId, 
    decimal amountToCheck) : IntegrationEvent(aggregateId, correlationId)
{
    public decimal AmountToCheck { get; } = amountToCheck;
}