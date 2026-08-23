using BrewUp.Payment.ReadModel.Services;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Payment.ReadModel.EventHandlers;

public sealed class PaymentAuthorizedEventHandler(
    IPaymentAuthorizationService service,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<PaymentAuthorized>(loggerFactory)
{
    public override async Task HandleAsync(
        PaymentAuthorized @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var persistenceResult = await service
            .MarkAuthorizedAsync(
                new PaymentAuthorizationId(@event.AggregateId.Value),
                @event.ProviderReference,
                cancellationToken)
            .ConfigureAwait(false);
        var persisted = persistenceResult.Match(
            success => success,
            error => throw new PersistenceException(
                $"Failed to persist authorized payment projection: {error}"));
        if (!persisted)
        {
            throw new PersistenceException(
                "Failed to persist authorized payment projection.");
        }
    }
}
