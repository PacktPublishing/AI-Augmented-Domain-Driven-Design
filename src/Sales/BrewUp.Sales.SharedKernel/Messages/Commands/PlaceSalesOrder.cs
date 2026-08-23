using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class PlaceSalesOrder(IntegrationId aggregateId, 
    Guid correlationId,
    CreateSalesOrderJson salesOrder,
    CustomerJson customer) : Command(aggregateId, correlationId)
{
    public CreateSalesOrderJson SalesOrder { get; private set; } = salesOrder;
    public CustomerJson Customer { get; private set; } = customer;
}