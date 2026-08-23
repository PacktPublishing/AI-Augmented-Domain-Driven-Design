using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Facade.Acl;

public sealed class RequestBeerAvailablityRaisedEventHandler(IEventBus eventBus,
    IAvailabilityService availabilityService, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<RequestBeerAvailablityRaised>(loggerFactory)
{
    public override async Task HandleAsync(RequestBeerAvailablityRaised @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<ItemRequested> beerAvailability = [];
        foreach (var row in @event.Rows)
        {
            var availablityResult = await availabilityService.GetAvailabilityByWarehouseIdAndBeerIdAsync(new WarehouseId(@event.WarehouseId),
                new BeerId(row.BeerId.Value), cancellationToken);
            if (!availablityResult.IsSuccess)
            {
                beerAvailability = beerAvailability.Append(row);
                continue;
            }
            
            availablityResult.TryGetValue(out var availability);
            beerAvailability = beerAvailability.Append(row with
            {
                QuantityAvailable = new Quantity(availability.Quantity, availability.UnitOfMeasure)
            });
            return;
        }
        
        RequestBeersAvailabilityChecked integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            MessageHelpers.GetCorrelationId(@event), beerAvailability);
        await eventBus.PublishAsync(integrationEvent, cancellationToken);
        
        // At least we need a command to reserve the quantity
        // ReserveBeer command = new(
    }
}