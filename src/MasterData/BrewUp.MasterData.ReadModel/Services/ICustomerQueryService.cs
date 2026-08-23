using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.ReadModel.Services;

public interface ICustomerQueryService
{
    Task<Result<PagedResult<CustomerJson>>> GetCustomersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);
    Task<Result<CustomerJson>> GetCustomerByIdAsync(string customerId, CancellationToken cancellationToken);
}