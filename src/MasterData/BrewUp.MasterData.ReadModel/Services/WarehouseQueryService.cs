using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;

namespace BrewUp.MasterData.ReadModel.Services;

internal sealed class WarehouseQueryService(ILoggerFactory loggerFactory, 
    IQueries<Warehouse> warehouseQueries)
    : ServiceBase(loggerFactory), IWarehouseQueryService
{
    public async Task<Result<PagedResult<WarehouseJson>>> GetWarehousesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await warehouseQueries.GetByFilterAsync(null, pageNumber, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<Warehouse> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<WarehouseJson>>.Success(new PagedResult<WarehouseJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<WarehouseJson>>.Success(new PagedResult<WarehouseJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<WarehouseJson>>.Error("Error retrieving warehouses"));
    }
}