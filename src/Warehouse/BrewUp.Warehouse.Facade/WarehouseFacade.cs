using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Lena.Core;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Facade;

internal sealed class WarehouseFacade(IShipmentService shipmentService, 
    IWarehouseService warehouseService,
    IAvailabilityService availabilityService,
    IServiceBus serviceBus) : IWarehouseFacade
{
    public Task<Result<PagedResult<ShipmentJson>>> GetShipmentOrdersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken) =>
        shipmentService.GetShipmentsAsync(pageNumber, pageSize, cancellationToken);

    public async Task<Result<string>> AddItemStockAsync(AddItemStockJson json, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var warehouseResult = await warehouseService.GetWarehouseByIdAsync(new WarehouseId(json.WarehouseId), cancellationToken);
        if (warehouseResult.IsError)
            return Result<string>.Error("Warehouse not found");

        var chkAvailability = await availabilityService.GetAvailabilityByWarehouseIdAndBeerIdAsync(new WarehouseId(json.WarehouseId), new BeerId(json.BeerId),
            cancellationToken);

        if (chkAvailability.IsError)
        {
            CreateAvailability createCommand = new(new AvailabilityId(json.Id), new WarehouseId(json.WarehouseId), new BeerId(json.BeerId), 
                new Quantity(json.Quantity, json.UnitOfMeasure));
            await serviceBus.SendAsync(createCommand, cancellationToken);
            
            return Result<string>.Success("OK");
        }
        
        AddItemStock command = new(new AvailabilityId(json.Id), 
            new Quantity(json.Quantity, json.UnitOfMeasure), 
            Guid.NewGuid());
        await serviceBus.SendAsync(command, cancellationToken);

        return Result.Success("OK");
    }
}