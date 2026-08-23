using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Payment.Facade.Acl;

public sealed class SagaRequestsPaymentAuthorizationIntegrationEventHandler(
    IServiceBus serviceBus,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SagaRequestsPaymentAuthorizationIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SagaRequestsPaymentAuthorizationIntegrationEvent @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = new AuthorizePayment(
            new PaymentAuthorizationId(Guid.CreateVersion7().ToString()),
            MessageHelpers.GetCorrelationId(@event),
            @event.SalesOrderId,
            @event.Amount);

        await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
    }
}
