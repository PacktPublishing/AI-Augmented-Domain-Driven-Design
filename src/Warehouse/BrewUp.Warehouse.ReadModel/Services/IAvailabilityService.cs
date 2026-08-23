using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Lena.Core;

namespace BrewUp.Warehouse.ReadModel.Services
{
    public interface IAvailabilityService
    {
        Task<Result<bool>> AddAvailabilityAsync(AvailabilityId availabilityId, WarehouseId warehouseId, BeerId beerId,
            Quantity quantity, CancellationToken cancellationToken);
        Task<Result<AvailabilityJson>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Result<string>> AddItemStockAsync(AvailabilityId availabilityId, Quantity quantity, CancellationToken cancellationToken);

        Task<Result<AvailabilityJson>> GetAvailabilityByWarehouseIdAndBeerIdAsync(WarehouseId warehouseId, BeerId beerId,
            CancellationToken cancellationToken);
        Task<Result<AvailabilityJson>> GetAvailabilityByBeerIdAsync(BeerId beerId, CancellationToken cancellationToken);
        Task<Result<ReorderThreshold>> GetReorderThresholdByBeerIdAsync(BeerId beerId, CancellationToken cancellationToken);
    }
}
