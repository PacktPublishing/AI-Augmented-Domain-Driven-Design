using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Sales.Facade.Acl;

public sealed class SagaSalesOrderReadyToConfirmIntegrationEventHandler(
    IServiceBus serviceBus,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SagaSalesOrderReadyToConfirmIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SagaSalesOrderReadyToConfirmIntegrationEvent @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = new ConfirmSalesOrder(
            new SalesOrderId(@event.SalesOrderId),
            MessageHelpers.GetCorrelationId(@event),
            new PaymentAuthorizationReference(@event.PaymentAuthorizationId),
            new StockReservationReference(@event.StockReservationId));

        await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
    }
}
