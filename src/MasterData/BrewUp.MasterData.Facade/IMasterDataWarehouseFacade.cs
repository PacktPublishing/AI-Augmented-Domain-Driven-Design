using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

public interface IMasterDataWarehouseFacade
{
    Task<Result<string>> CreateWarehouseAsync(CreateWarehouseJson body, CancellationToken cancellationToken);
    Task<Result<PagedResult<WarehouseJson>>> GetWarehousesAsync(int page, int pageSize, CancellationToken cancellationToken);
}