using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderCreatedForSalesOrderPlacedIntegrationEventHandler(
    IEventBus  eventBus, ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<SalesOrderCreated>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var correlationId = GetCorrelationId(@event);
        SalesOrderPlaced integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            correlationId, 
            @event.SalesOrderNumber.Value, 
            @event.SalesOrderDate.Value,
            @event.Customer.CustomerId.Value,
            @event.SalesOrderDeliveryDate.Value, 
            @event.Rows);

        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}