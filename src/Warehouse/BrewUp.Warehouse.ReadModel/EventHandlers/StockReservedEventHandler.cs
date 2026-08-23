using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.ReadModel.EventHandlers;

public sealed class StockReservedEventHandler(
    IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<StockReserved>(loggerFactory)
{
    public override async Task HandleAsync(StockReserved @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new StockReservedIntegrationEvent(
            new IntegrationId(@event.AggregateId.Value),
            @event.MessageId,
            @event.StockReservationId.Value,
            @event.SalesOrderId,
            @event.ReservedRows);

        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
