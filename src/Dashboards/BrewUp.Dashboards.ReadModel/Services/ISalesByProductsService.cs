using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Dashboards.ReadModel.Services;

public interface ISalesByProductsService
{
    Task<Result<PagedResult<SalesByProductsJson>>> GetSalesByProductAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}