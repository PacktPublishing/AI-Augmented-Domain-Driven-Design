using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.Infrastructure.Hubs;
using BrewUp.Dashboards.SharedKernel.Messages.Commands;
using BrewUp.Dashboards.SharedKernel.Persistence;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Domain.CommandHandlers;

public sealed class IncreaseSummaryByCustomersCommandHandler(IDashboardsRepository<SalesByCustomers> repository,
    IDashboardsHubHelper dashboardsHubHelper,
    ILoggerFactory loggerFactory) 
    : DashboardsCommandHandlerBaseAsync<IncreaseSummaryByCustomer>(repository, loggerFactory)
{
    public override async Task HandleAsync(IncreaseSummaryByCustomer command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var aggregateResult = await repository.GetByIdAsync(command.AggregateId.Value, cancellationToken);
        if (aggregateResult.IsError)
            return;
        
        aggregateResult.TryGetValue(out var salesByCustomer);
        if (salesByCustomer is null)
            return;
        
        salesByCustomer.UpdateTotalSales(command.SalesOrderValue);
        await repository.UpdateAsync(salesByCustomer, cancellationToken);
        
        await dashboardsHubHelper.TellChildrenThatCustomersDashboardWasUpdated(salesByCustomer.CustomerName, cancellationToken);
    }
}