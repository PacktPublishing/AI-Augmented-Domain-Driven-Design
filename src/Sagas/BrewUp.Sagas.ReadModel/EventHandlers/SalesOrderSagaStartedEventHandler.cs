using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SalesOrderSagaStartedEventHandler(IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<SalesOrderSagaStarted>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderSagaStarted @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var amountToCheck = @event.Rows.Sum(r => r.Quantity.Value * r.Price.Value);
        
        SalesOrderSagaStartedIntegrationEvent integrationEvent = new(new CustomerId(@event.CustomerId), correlationId, amountToCheck);
        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}