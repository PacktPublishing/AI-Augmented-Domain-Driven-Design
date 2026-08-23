using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Dashboards.ReadModel.Services;

public interface ISalesByCustomersService
{
    Task<Result<PagedResult<SalesByCustomerJson>>> GetSalesByCustomerAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}