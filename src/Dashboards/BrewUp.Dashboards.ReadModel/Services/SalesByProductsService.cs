using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Dashboards.ReadModel.Services;

internal sealed class SalesByProductsService(IQueries<SalesByProducts> salesByProductQueries) : ISalesByProductsService
{
    public async Task<Result<PagedResult<SalesByProductsJson>>> GetSalesByProductAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await salesByProductQueries.GetByFilterAsync(null, pageNumber, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<SalesByProducts> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<SalesByProductsJson>>.Success(new PagedResult<SalesByProductsJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<SalesByProductsJson>>.Success(new PagedResult<SalesByProductsJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<SalesByProductsJson>>.Error("Error retrieving salesByProducts"));
    }
}