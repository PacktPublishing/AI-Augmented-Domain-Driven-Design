using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Dashboards.Facade;

public interface IDashboardsFacade
{
    Task<Result<PagedResult<SalesByCustomerJson>>> GetSalesByCustomerAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    
    Task<Result<PagedResult<SalesByProductsJson>>> GetSalesByProductAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}