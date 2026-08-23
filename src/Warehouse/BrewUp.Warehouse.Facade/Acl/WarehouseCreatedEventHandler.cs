using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events;
using BrewUp.Shared.Messages.Events.MasterData;
using BrewUp.Warehouse.ReadModel.Services;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.Facade.Acl;

public sealed class WarehouseCreatedEventHandler(IWarehouseService warehouseService, 
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<WarehouseCreated>(loggerFactory)
{
    public override async Task HandleAsync(WarehouseCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        await warehouseService.AddWarehouseAsync(new WarehouseId(@event.AggregateId.Value), @event.WarehouseName,
            cancellationToken);
    }
}