using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.ReadModel.Services;

public interface IWarehouseQueryService
{
    Task<Result<PagedResult<WarehouseJson>>> GetWarehousesAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}