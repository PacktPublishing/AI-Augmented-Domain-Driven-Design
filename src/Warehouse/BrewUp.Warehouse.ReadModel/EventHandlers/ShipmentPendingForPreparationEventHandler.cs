using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.ReadModel.EventHandlers;

public sealed class ShipmentPendingForPreparationEventHandler(
    IShipmentService shipmentService,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<ShipmentPendingForPreparation>(loggerFactory)
{
    public override async Task HandleAsync(ShipmentPendingForPreparation @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await shipmentService.AddShipmentAsync((ShipmentId)@event.AggregateId, @event.SalesOrderId, @event.CustomerId,
            @event.DeliveryDate, @event.Rows, @event.ShipmentState, cancellationToken);
    }
}