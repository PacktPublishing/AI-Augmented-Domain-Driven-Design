using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Payment.Facade.EventHandlers;

public sealed class PaymentAuthorizedIntegrationEventPublisher(
    IEventBus eventBus,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<PaymentAuthorized>(loggerFactory)
{
    public override async Task HandleAsync(
        PaymentAuthorized @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent = new PaymentAuthorizedIntegrationEvent(
            new PaymentAuthorizationId(@event.AggregateId.Value),
            @event.ProviderReference,
            MessageHelpers.GetCorrelationId(@event));
        await eventBus
            .PublishAsync(integrationEvent, cancellationToken)
            .ConfigureAwait(false);
    }
}
