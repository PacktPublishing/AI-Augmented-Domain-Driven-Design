using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SagaSalesOrderPlacedEventHandler(IEventBus eventBus, ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<SagaSalesOrderPlaced>(loggerFactory)
{
    public override async Task HandleAsync(SagaSalesOrderPlaced @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        RequestBeerAvailablityRaised integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event), @event.WarehouseId,
            @event.Rows.Select(x =>
                new ItemRequested(new BeerId(x.BeerId), x.Quantity, new Quantity(0, x.Quantity.UnitOfMeasure))));
        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}