using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.SharedKernel.Messages.Commands;
using BrewUp.Dashboards.SharedKernel.Persistence;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Domain.CommandHandlers;

public sealed class IncreaseSummaryByProductsCommandHandler(IDashboardsRepository<SalesByProducts> repository, ILoggerFactory loggerFactory) 
    : DashboardsCommandHandlerBaseAsync<IncreaseSummaryByProducts>(repository, loggerFactory)
{
    public override async Task HandleAsync(IncreaseSummaryByProducts command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var aggregateResult = await repository.GetByIdAsync(command.AggregateId.Value, cancellationToken);
        if (aggregateResult.IsError)
            return;
        
        aggregateResult.TryGetValue(out var salesByProducts);
        if (salesByProducts is null)
            return;
        
        salesByProducts.UpdateTotalSales(command.SalesOrderValue, command.SalesOrderQuantity);
        await repository.UpdateAsync(salesByProducts, cancellationToken);
    }
}