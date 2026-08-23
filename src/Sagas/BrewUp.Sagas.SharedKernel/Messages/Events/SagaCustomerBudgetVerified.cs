using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaCustomerBudgetVerified(CustomerId aggregateId, Guid correlationId,
    CustomerJson customer,
    CreateSalesOrderJson order) : DomainEvent(aggregateId, correlationId)
{
    public CustomerJson Customer { get; private set; } = customer;
    public CreateSalesOrderJson Order { get; private set; } = order;
}