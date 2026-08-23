using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Sales.Facade;

public interface ISalesFacade
{
    Task<Result<string>> CreateSalesOrderAsync(CreateSalesOrderJson body, CancellationToken cancellationToken);
    Task CloseSalesOrderAsync(string orderId, CancellationToken cancellationToken);
    Task<Result<PagedResult<SalesOrderJson>>> GetSalesOrdersAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Result<SalesOrderJson>> GetSalesOrderByIdAsync(string orderId, CancellationToken cancellationToken);
    Task<Result<string>> AddBeersToSalesOrderAsync(AddBeersToCartJson body, CancellationToken cancellationToken);
}