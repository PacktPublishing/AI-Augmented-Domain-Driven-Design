using BrewUp.Payment.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Payment.ReadModel.EventHandlers;

public sealed class PaymentDeclinedEventHandler(IEventBus eventBus, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<PaymentDeclined>(loggerFactory)
{
    public override async Task HandleAsync(PaymentDeclined @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new PaymentDeclinedIntegrationEvent(
            new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event),
            @event.SalesOrderId,
            @event.Reason);

        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
