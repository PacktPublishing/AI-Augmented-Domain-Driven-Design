using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;

namespace BrewUp.Dashboards.Domain;

public interface IDashboardsDomainService
{
    Task CreateSummaryByCustomersAsync(CustomerId customerId, CustomerName customerName, SalesOrderYear salesOrderYear, CancellationToken cancellationToken);
    Task IncreaseSummaryByCustomersAsync(CustomerId customerId, SalesOrderValue salesOrderValue, CancellationToken cancellationToken);
    
    Task CreateSummaryByProductsAsync(BeerId beerId, BeerName beerName, SalesOrderYear salesOrderYear, CancellationToken cancellationToken);
    Task IncreaseSummaryByProductsAsync(BeerId beerId, SalesOrderValue salesOrderValue, SalesOrderQuantity salesOrderQuantity, CancellationToken cancellationToken);
}