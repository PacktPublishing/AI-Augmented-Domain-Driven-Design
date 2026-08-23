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
    IServiceBus serviceBus,
    IAvailabilityService availabilityService, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<RequestBeerAvailablityRaised>(loggerFactory)
{
    public override async Task HandleAsync(RequestBeerAvailablityRaised @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        List<ItemRequested> beerAvailability = [];
        List<ReserveItemStock> reserveCommands = [];
        StockReservationId stockReservationId = new(Guid.CreateVersion7().ToString());
        var canReserveAllRows = true;

        foreach (var row in @event.Rows)
        {
            var availablityResult = await availabilityService.GetAvailabilityByWarehouseIdAndBeerIdAsync(new WarehouseId(@event.WarehouseId),
                new BeerId(row.BeerId.Value), cancellationToken);
            if (!availablityResult.IsSuccess)
            {
                beerAvailability.Add(row);
                canReserveAllRows = false;
                continue;
            }
            
            availablityResult.TryGetValue(out var availability);
            var quantityAvailable = new Quantity(availability.Quantity, availability.UnitOfMeasure);
            beerAvailability.Add(row with
            {
                QuantityAvailable = quantityAvailable
            });

            if (row.QuantityOrdered.UnitOfMeasure != quantityAvailable.UnitOfMeasure ||
                row.QuantityOrdered.Value > quantityAvailable.Value)
            {
                canReserveAllRows = false;
                continue;
            }

            reserveCommands.Add(new ReserveItemStock(new AvailabilityId(availability.Id), stockReservationId,
                new SalesOrderId(@event.AggregateId.Value), row.QuantityOrdered, correlationId));
        }

        if (canReserveAllRows)
        {
            foreach (var command in reserveCommands)
            {
                await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }
        
        RequestBeersAvailabilityChecked integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            correlationId, canReserveAllRows ? stockReservationId : null, beerAvailability);
        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
