using BrewUp.Sales.Domain;
using BrewUp.Sales.ReadModel.Services;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Sales.Facade;

internal class SalesFacade(ISalesDomainService salesDomainService,
    ISalesOrderService salesOrderService,
    IBeerService beerService,
    ICustomerService customerService) : ISalesFacade
{
    public async Task<Result<string>> CreateSalesOrderAsync(CreateSalesOrderJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        foreach (var row in body.Rows)
        {
            var beerResult = await beerService.GetBeerByIdAsync(new BeerId(row.BeerId), cancellationToken);
            if (beerResult.IsError)
                return Result<string>.Error($"Beer with ID {row.BeerId} not found");
        }
        
        return await salesDomainService.CreateSalesOrderAsync(body, cancellationToken);
    }

    public Task CloseSalesOrderAsync(string orderId, CancellationToken cancellationToken) =>
        salesDomainService.CloseSalesOrderAsync(orderId, cancellationToken);

    public Task<Result<PagedResult<SalesOrderJson>>> GetSalesOrdersAsync(int page, int pageSize,
        CancellationToken cancellationToken) =>
        salesOrderService.GetSalesOrdersAsync(page, pageSize, cancellationToken);

    public Task<Result<SalesOrderJson>> GetSalesOrderByIdAsync(string orderId, CancellationToken cancellationToken) =>
        salesOrderService.GetSalesOrderByIdAsync(orderId, cancellationToken);

    public async Task<Result<string>> AddBeersToSalesOrderAsync(AddBeersToCartJson body,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var row in body.Rows)
        {
            var beerResult = await beerService.GetBeerByIdAsync(new BeerId(row.BeerId), cancellationToken);
            if (beerResult.IsError)
                return Result<string>.Error($"Beer with ID {row.BeerId} not found");
        }
        
        return await salesDomainService.AddBeersToSalesOrderAsync(body, cancellationToken);
    }
}