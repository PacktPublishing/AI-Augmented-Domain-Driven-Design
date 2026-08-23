using BrewUp.MasterData.Domain.Services;
using BrewUp.MasterData.ReadModel.Services;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

internal class MasterDataWarehouseFacade(IWarehouseDomainService warehouseDomainService,
    IWarehouseQueryService queries) : IMasterDataWarehouseFacade
{
    public Task<Result<string>> CreateWarehouseAsync(CreateWarehouseJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return warehouseDomainService.CreateWarehouseAsync(body, cancellationToken);
    }

    public Task<Result<PagedResult<WarehouseJson>>> GetWarehousesAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return queries.GetWarehousesAsync(page, pageSize, cancellationToken);
    }
}