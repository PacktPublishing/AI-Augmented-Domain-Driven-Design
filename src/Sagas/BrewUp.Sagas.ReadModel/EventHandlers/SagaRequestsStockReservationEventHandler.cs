using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SagaRequestsStockReservationEventHandler(
    IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<SagaRequestsStockReservation>(loggerFactory)
{
    public override async Task HandleAsync(SagaRequestsStockReservation @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new SagaRequestsStockReservationIntegrationEvent(
            new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event),
            @event.SalesOrderId,
            @event.WarehouseId,
            @event.Rows.Select(r => new ItemRequested(
                new BeerId(r.BeerId),
                new Quantity(r.Quantity.Value, r.Quantity.UnitOfMeasure),
                new Quantity(0, r.Quantity.UnitOfMeasure))));

        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
