using System.Linq.Expressions;
using BrewUp.Dashboards.Domain;
using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.Infrastructure;
using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events;
using BrewUp.Shared.Messages.Events.Sales;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Dashboards.Facade.Acl;

public sealed class SalesOrderCreatedIntegrationForCustomerSummaryEventHandler(
    IQueries<SalesByCustomers> salesByCustomersQueries,
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
        
        var salesOrderValue = @event.Rows.Sum(row => (row.Price.Value * row.Quantity.Value));
        
        Expression<Func<SalesByCustomers, bool>> query = customers => customers.Id == @event.CustomerId && customers.Year == @event.SalesOrderDate.Year.ToString(); 
        var queryResult = await salesByCustomersQueries.GetByFilterAsync(query, 1, 1, cancellationToken);
        
        if (queryResult.IsError)
            return;
        
        queryResult.TryGetValue(out var pagedResult);
            
        if (!pagedResult.Results.Any())
        {
            await dashboardsDomainService.CreateSummaryByCustomersAsync(new CustomerId(@event.CustomerId), new CustomerName(@event.CustomerName),
                new SalesOrderYear(@event.SalesOrderDate.Year.ToString()), cancellationToken);
        }
            
        salesOrderValue += pagedResult.Results.FirstOrDefault()?.TotalSales ?? 0;
         
        await dashboardsDomainService.IncreaseSummaryByCustomersAsync(new CustomerId(@event.CustomerId), new SalesOrderValue(salesOrderValue, "EUR"), cancellationToken);
        
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