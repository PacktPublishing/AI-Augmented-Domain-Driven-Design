using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderCreatedEventHandler(
    ISalesOrderService salesOrderService,
    ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<SalesOrderCreated>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await salesOrderService.CreateSalesOrderAsync((SalesOrderId)@event.AggregateId, @event.SalesOrderNumber,
            @event.Customer.CustomerId, @event.Customer.CustomerName, @event.SalesOrderDate, @event.Rows, cancellationToken);
    }
}