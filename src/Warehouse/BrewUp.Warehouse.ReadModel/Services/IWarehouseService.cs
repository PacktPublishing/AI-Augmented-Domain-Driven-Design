using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using Lena.Core;

namespace BrewUp.Warehouse.ReadModel.Services;

public interface IWarehouseService
{
    Task<Result<bool>> AddWarehouseAsync(WarehouseId warehouseId, WarehouseName warehouseName,
        CancellationToken cancellationToken = default);

    Task<Result<WarehouseJson>> GetWarehouseByIdAsync(WarehouseId warehouseId, CancellationToken cancellationToken = default);
}