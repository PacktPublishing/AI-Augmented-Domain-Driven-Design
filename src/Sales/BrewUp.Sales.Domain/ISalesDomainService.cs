using BrewUp.Shared.ExternalContracts;
using BrewUp.Shared.ExternalContracts.Sales;
using Lena.Core;

namespace BrewUp.Sales.Domain;

public interface ISalesDomainService
{
    Task<Result<string>> CreateSalesOrderAsync(CreateSalesOrderJson body, CancellationToken cancellationToken);
    Task<Result<bool>> CloseSalesOrderAsync(string orderId, CancellationToken cancellationToken);
    Task<Result<string>> AddBeersToSalesOrderAsync(AddBeersToCartJson body, CancellationToken cancellationToken);
}