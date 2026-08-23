using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events;
using BrewUp.Shared.Messages.Events.Sales;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Facade.Acl;

public sealed class SalesOrderCreatedIntegrationEventHandler(IServiceBus serviceBus, 
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SalesOrderCreatedIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        PrepareShipment command = new(new ShipmentId(Guid.NewGuid().ToString()),
            new SalesOrderId(@event.AggregateId.Value),
            new CustomerId(@event.CustomerId),
            new DeliveryDate(@event.SalesOrderDeliveryDate),
            @event.Rows,
            @event.MessageId);

        await serviceBus.SendAsync(command, cancellationToken);
    }
}