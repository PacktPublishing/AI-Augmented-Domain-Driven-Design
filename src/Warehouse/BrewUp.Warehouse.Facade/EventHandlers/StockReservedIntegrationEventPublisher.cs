using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.Facade.EventHandlers;

public sealed class StockReservedIntegrationEventPublisher(
    IEventBus eventBus,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<StockReserved>(loggerFactory)
{
    public override async Task HandleAsync(
        StockReserved @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await eventBus
            .PublishAsync(
                new StockReservedIntegrationEvent(
                    new StockReservationId(@event.AggregateId.Value),
                    @event.WarehouseId,
                    @event.SalesOrderId,
                    @event.Rows,
                    MessageHelpers.GetCorrelationId(@event)),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
