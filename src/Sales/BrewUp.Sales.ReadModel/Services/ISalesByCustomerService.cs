using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Lena.Core;

namespace BrewUp.Sales.ReadModel.Services;

public interface ISalesByCustomerService
{
    Task<Result<bool>> CreateSalesByCustermerAsync(CustomerId customerId, CustomerName customerName,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> IncreaseSalesAsync(CustomerId customerId, Price price, CancellationToken cancellationToken = default);
}