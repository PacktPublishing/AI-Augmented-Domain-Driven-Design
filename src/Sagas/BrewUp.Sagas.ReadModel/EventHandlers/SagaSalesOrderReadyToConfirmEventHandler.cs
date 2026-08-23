using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SagaSalesOrderReadyToConfirmEventHandler(
    IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<SagaSalesOrderReadyToConfirm>(loggerFactory)
{
    public override async Task HandleAsync(SagaSalesOrderReadyToConfirm @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new SagaSalesOrderReadyToConfirmIntegrationEvent(
            new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event),
            @event.SalesOrderId,
            @event.PaymentAuthorizationId,
            @event.StockReservationId);

        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
