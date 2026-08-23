using BrewUp.Dashboards.Domain;
using BrewUp.Dashboards.ReadModel.Services;
using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Facade;

internal sealed class DashboardsFacade(IDashboardsDomainService dashboardsDomainService,
    ISalesByCustomersService salesByCustomersService,
    ISalesByProductsService salesByProductsService,
    ILoggerFactory loggerFactory) : IDashboardsFacade
{
    private readonly ILogger<DashboardsFacade> _logger = loggerFactory.CreateLogger<DashboardsFacade>();

    public async Task<Result<PagedResult<SalesByCustomerJson>>> GetSalesByCustomerAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return await salesByCustomersService.GetSalesByCustomerAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<PagedResult<SalesByProductsJson>>> GetSalesByProductAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return await salesByProductsService.GetSalesByProductAsync(pageNumber, pageSize, cancellationToken);
    }
}