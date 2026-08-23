using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderCreatedForSalesSummaryEventHandler(
    ISalesOrderSummaryService salesOrderSummaryService,
    ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<SalesOrderCreated>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        await salesOrderSummaryService.CreateSalesOrderAsync(new SalesOrderId(@event.AggregateId.Value),
            @event.SalesOrderNumber, @event.Customer.CustomerId, @event.Customer.CustomerName,
            @event.SalesOrderDate, new Price(@event.Rows.Sum(r => r.Quantity.Value * r.Price.Value), "EUR"),
            SalesOrderStatus.Accepted, cancellationToken);
    }
}