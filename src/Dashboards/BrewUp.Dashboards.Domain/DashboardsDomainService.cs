using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Dashboards.SharedKernel.Messages.Commands;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.Domain;

internal sealed class DashboardsDomainService(
    ICommandHandlerAsync<CreateSummaryByCustomer> createSummarySummaryByCustomerCommandHandler,
    ICommandHandlerAsync<IncreaseSummaryByCustomer> increaseSummarySummaryByCustomerCommandHandler,
    ICommandHandlerAsync<CreateSummaryByProducts> createSummaryByProductsCommandHandler,
    ICommandHandlerAsync<IncreaseSummaryByProducts> increaseSummaryByProductsCommandHandler) : IDashboardsDomainService
{
    public async Task CreateSummaryByCustomersAsync(CustomerId customerId, CustomerName customerName, SalesOrderYear salesOrderYear,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        CreateSummaryByCustomer command = new(customerId, customerName, salesOrderYear);
        await createSummarySummaryByCustomerCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task IncreaseSummaryByCustomersAsync(CustomerId customerId, SalesOrderValue salesOrderValue,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        IncreaseSummaryByCustomer command = new(customerId, salesOrderValue);
        await increaseSummarySummaryByCustomerCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task CreateSummaryByProductsAsync(BeerId beerId, BeerName beerName, SalesOrderYear salesOrderYear,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        CreateSummaryByProducts command = new(beerId, beerName, salesOrderYear);
        await createSummaryByProductsCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task IncreaseSummaryByProductsAsync(BeerId beerId, SalesOrderValue salesOrderValue, SalesOrderQuantity salesOrderQuantity,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        IncreaseSummaryByProducts command = new(beerId, salesOrderValue, salesOrderQuantity);
        await increaseSummaryByProductsCommandHandler.HandleAsync(command, cancellationToken);
    }
}