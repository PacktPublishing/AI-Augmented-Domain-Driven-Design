using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Sales.Facade.Acl;

public sealed class SagaSalesOrderReadyToConfirmIntegrationEventHandler(IServiceBus serviceBus,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SagaSalesOrderReadyToConfirmIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SagaSalesOrderReadyToConfirmIntegrationEvent @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        ConfirmSalesOrder command = new(
            new SalesOrderId(@event.SalesOrderId),
            new PaymentAuthorizationId(@event.PaymentAuthorizationId),
            new StockReservationId(@event.StockReservationId),
            MessageHelpers.GetCorrelationId(@event));

        await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
    }
}
