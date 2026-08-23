using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderAcceptedEventHandler(IEventBus eventBus,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<SalesOrderAccepted>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderAccepted @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        SagasSalesOrderAccepted integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event));
        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}