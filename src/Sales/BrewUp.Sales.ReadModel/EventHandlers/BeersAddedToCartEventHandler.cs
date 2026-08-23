using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public class BeersAddedToCartEventHandler(
    ISalesOrderService salesOrderService,
    ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<BeersAddedToCart>(loggerFactory)
{
    public override async Task HandleAsync(BeersAddedToCart @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        await salesOrderService.AddBeersToSalesOrderAsync((SalesOrderId) @event.AggregateId, @event.Rows,
            cancellationToken);
    }
}