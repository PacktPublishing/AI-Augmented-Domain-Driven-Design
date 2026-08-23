using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class CustomerBudgetUnVerified(CustomerId aggregateId, Guid correlationId,
    string message) : IntegrationEvent(aggregateId, correlationId)
{
    public string Message { get; private set; } = message;
}