using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public class SagaCustomerBudgetVerifiedIntegrationEvent(IntegrationId aggregateId, 
    Guid correlationId,
    CreateSalesOrderJson salesOrder,
    CustomerJson customer) : IntegrationEvent(aggregateId, correlationId)
{
    public CreateSalesOrderJson SalesOrder { get; private set; } = salesOrder;
    public CustomerJson Customer { get; private set; } = customer;
}