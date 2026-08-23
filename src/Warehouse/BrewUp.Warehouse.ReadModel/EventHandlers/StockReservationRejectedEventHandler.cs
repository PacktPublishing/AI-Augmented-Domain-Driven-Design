using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.ReadModel.EventHandlers;

public sealed class StockReservationRejectedEventHandler(
    IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<StockReservationRejected>(loggerFactory)
{
    public override async Task HandleAsync(StockReservationRejected @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new StockReservationRejectedIntegrationEvent(
            new IntegrationId(@event.AggregateId.Value),
            @event.MessageId,
            @event.SalesOrderId,
            @event.Reason);

        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
