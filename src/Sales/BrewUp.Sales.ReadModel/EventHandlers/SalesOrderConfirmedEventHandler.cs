using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;
using SalesOrderConfirmed = BrewUp.Sales.SharedKernel.Messages.Events.SalesOrderConfirmed;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderConfirmedEventHandler(
    ISalesOrderService salesOrderService,
    IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<SalesOrderConfirmed>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderConfirmed @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await salesOrderService.ConfirmSalesOrderAsync(new SalesOrderId(@event.AggregateId.Value),
            @event.PaymentAuthorizationId, @event.StockReservationId, cancellationToken);
        if (result.IsError)
            return;

        SagasSalesOrderAccepted integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event));
        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}
