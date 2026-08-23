using BrewUp.Payment.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Payment.ReadModel.EventHandlers;

public sealed class PaymentAuthorizedEventHandler(IEventBus eventBus, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<PaymentAuthorized>(loggerFactory)
{
    public override async Task HandleAsync(PaymentAuthorized @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new PaymentAuthorizedIntegrationEvent(
            new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event),
            @event.AggregateId.Value,
            @event.SalesOrderId);

        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
