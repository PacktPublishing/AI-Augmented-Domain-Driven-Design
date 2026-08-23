using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SagaSalesOrderAvailablityCheckedEventHandler(IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<SagaSalesOrderAvailablityChecked>(loggerFactory)
{
    public override async Task HandleAsync(SagaSalesOrderAvailablityChecked @event,
        CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        SagaSalesOrderAvailabilityCheckedIntegrationEvent integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event), @event.SalesOrderId, @event.PaymentAuthorizationId,
            @event.StockReservationId, @event.Rows);
        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
