using BrewUp.Payment.ReadModel.Services;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Payment.ReadModel.EventHandlers;

public sealed class PaymentAuthorizationRequestedEventHandler(
    IPaymentAuthorizationService service,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<PaymentAuthorizationRequested>(loggerFactory)
{
    public override async Task HandleAsync(
        PaymentAuthorizationRequested @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var persistenceResult = await service
            .CreatePendingAsync(
                new PaymentAuthorizationId(@event.AggregateId.Value),
                @event.SalesOrder,
                cancellationToken)
            .ConfigureAwait(false);
        var persisted = persistenceResult.Match(
            success => success,
            error => throw new PersistenceException(
                $"Failed to persist pending payment authorization projection: {error}"));
        if (!persisted)
        {
            throw new PersistenceException(
                "Failed to persist pending payment authorization projection.");
        }
    }
}
