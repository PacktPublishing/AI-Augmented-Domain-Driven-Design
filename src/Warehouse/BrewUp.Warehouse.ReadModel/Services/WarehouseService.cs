using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Warehouse.ReadModel.Services;

internal sealed class WarehouseService([FromKeyedServices("warehouse")] IPersister persister,
    IQueries<Dtos.Warehouse> query,
    ILoggerFactory loggerFactory) 
    : ServiceBase(persister, loggerFactory), IWarehouseService
{
    public async Task<Result<bool>> AddWarehouseAsync(WarehouseId warehouseId, WarehouseName warehouseName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var warehouse = Dtos.Warehouse.Create(warehouseId, warehouseName);
        var insertResult = await Persister.InsertAsync(warehouse, cancellationToken);
        
        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating warehouse: {Error}", error);
                return Result<bool>.Error($"Error creating warehouse: {error}");
            });
    }

    public async Task<Result<WarehouseJson>> GetWarehouseByIdAsync(WarehouseId warehouseId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await query.GetByIdAsync(warehouseId.Value, cancellationToken);

        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out Dtos.Warehouse warehouseDto);
                return Result<WarehouseJson>.Success(warehouseDto.ToJson());
            },
            _ => Result<WarehouseJson>.Error("Error retrieving warehouse"));
    }
}