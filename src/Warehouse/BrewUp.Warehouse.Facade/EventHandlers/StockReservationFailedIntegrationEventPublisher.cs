using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.Facade.EventHandlers;

public sealed class StockReservationFailedIntegrationEventPublisher(
    IEventBus eventBus,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<StockReservationFailed>(loggerFactory)
{
    public override async Task HandleAsync(
        StockReservationFailed @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await eventBus
            .PublishAsync(
                new StockReservationFailedIntegrationEvent(
                    new StockReservationId(@event.AggregateId.Value),
                    @event.WarehouseId,
                    @event.SalesOrderId,
                    @event.Reason,
                    MessageHelpers.GetCorrelationId(@event)),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
