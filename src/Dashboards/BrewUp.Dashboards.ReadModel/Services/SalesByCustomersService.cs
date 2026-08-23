using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Dashboards.ReadModel.Services;

internal sealed class SalesByCustomersService(IQueries<SalesByCustomers> salesByCustomersQueries) : ISalesByCustomersService
{
    public async Task<Result<PagedResult<SalesByCustomerJson>>> GetSalesByCustomerAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await salesByCustomersQueries.GetByFilterAsync(null, pageNumber, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<SalesByCustomers> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<SalesByCustomerJson>>.Success(new PagedResult<SalesByCustomerJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<SalesByCustomerJson>>.Success(new PagedResult<SalesByCustomerJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<SalesByCustomerJson>>.Error("Error retrieving salesByCustomers"));
    }
}