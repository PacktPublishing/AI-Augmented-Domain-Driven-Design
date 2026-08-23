using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.Messages.Events.Sales;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderCreatedForIntegrationEventHandler(
    IEventBus  eventBus,
    ILoggerFactory loggerFactory) 
    : DomainEventHandlerAsync<SalesOrderCreated>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        SalesOrderCreatedIntegrationEvent integrationEvent = new (
            new IntegrationId(@event.AggregateId.Value), @event.SalesOrderNumber.Value,
            @event.SalesOrderDate.Value, @event.Customer.CustomerId.Value, @event.Customer.CustomerName.Value,
            @event.SalesOrderDeliveryDate.Value,
            @event.Rows.Select(r => new OrderRowDto
            {
                BeerId = r.BeerId,
                BeerName = r.BeerName,
                Quantity = r.Quantity
            }).ToList());

        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}