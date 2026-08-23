using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class CustomerBudgetVerified(CustomerId aggregateId, Guid correlationId,
    CustomerJson customer) : IntegrationEvent(aggregateId, correlationId)
{
    public CustomerJson Customer { get; private set; } = customer;
}