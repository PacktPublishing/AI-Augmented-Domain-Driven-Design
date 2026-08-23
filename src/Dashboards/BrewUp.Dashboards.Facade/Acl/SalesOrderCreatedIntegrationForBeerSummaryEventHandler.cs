using System.Linq.Expressions;
using BrewUp.Dashboards.Domain;
using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.Infrastructure;
using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events;
using BrewUp.Shared.Messages.Events.Sales;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Dashboards.Facade.Acl;

public sealed class SalesOrderCreatedIntegrationForBeerSummaryEventHandler(
    IQueries<SalesByProducts> salesByProductsQueries,
    IDashboardsDomainService dashboardsDomainService,
    IMessagesReceivedService  messagesReceivedService,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SalesOrderCreatedWihPriceIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreatedWihPriceIntegrationEvent @event,
        CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (IsMessageAlreadyProcessed(@event.MessageId.ToString()))
            return;

        var beerGrouped = @event.Rows.GroupBy(row => row.BeerId);
        
        foreach (var row in @event.Rows.GroupBy(row => row.BeerId))
        {
            var salesOrderValue = @event.Rows.Sum(r => (r.Price.Value * r.Quantity.Value));
            var salesOrderQuantity = @event.Rows.Sum(row => row.Quantity.Value);
            
            Expression<Func<SalesByProducts, bool>> query = products => products.Id == @event.CustomerId && products.Year == @event.SalesOrderDate.Year.ToString(); 
            var queryResult = await salesByProductsQueries.GetByFilterAsync(query, 1, 1, cancellationToken);
        
            if (queryResult.IsError)
                return;
        
            queryResult.TryGetValue(out var pagedResult);
            
            if (!pagedResult.Results.Any())
            {
                using var orderRows = row.GetEnumerator();
                await dashboardsDomainService.CreateSummaryByProductsAsync(new BeerId(row.Key), new BeerName(orderRows.Current.BeerName),
                    new SalesOrderYear(@event.SalesOrderDate.Year.ToString()), cancellationToken);
            }
            
            salesOrderValue += pagedResult.Results.FirstOrDefault()?.TotalSales ?? 0;
         
            await dashboardsDomainService.IncreaseSummaryByProductsAsync(new BeerId(row.Key), 
                new SalesOrderValue(salesOrderValue, "EUR"),
                new SalesOrderQuantity(salesOrderQuantity, "Bottle"), 
                cancellationToken);
        }
        
        await AddMessageAsync(@event.MessageId.ToString(), cancellationToken);
    }
    
    private bool IsMessageAlreadyProcessed(string messageId)
    {
        var result = messagesReceivedService.GetByIdAsync(messageId, CancellationToken.None).GetAwaiter().GetResult();
        return result.IsSuccess;
    }
    
    private Task AddMessageAsync(string messageId, CancellationToken cancellationToken)
    {
        var message = MessagesReceived.Create(Guid.Parse(messageId), nameof(SalesOrderCreatedWihPriceIntegrationEvent));
        return messagesReceivedService.AddAsync(message, cancellationToken);
    }
}