using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderCreatedForCustomerSalesEventHandler(
    ISalesByCustomerService salesByCustomerService,
    ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<SalesOrderCreated>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await salesByCustomerService.CreateSalesByCustermerAsync(@event.Customer.CustomerId, @event.Customer.CustomerName,
            cancellationToken);
        await salesByCustomerService.IncreaseSalesAsync(@event.Customer.CustomerId, 
            new Price(@event.Rows.Sum(r => r.Quantity.Value * r.Price.Value), "EUR"), cancellationToken);
    }
}