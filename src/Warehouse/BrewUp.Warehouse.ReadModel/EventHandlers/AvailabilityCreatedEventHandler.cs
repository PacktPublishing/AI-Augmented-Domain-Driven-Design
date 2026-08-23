using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.ReadModel.EventHandlers
{
    public sealed class AvailabilityCreatedEventHandler(
        IAvailabilityService availabilityService,
        ILoggerFactory loggerFactory) : DomainEventHandlerAsync<AvailabilityCreated>(loggerFactory)
    {
        public override async Task HandleAsync(AvailabilityCreated @event, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();

            await availabilityService.AddAvailabilityAsync(new AvailabilityId(@event.AggregateId.Value),
                @event.WarehouseId,
                @event.BeerId,
                @event.Quantity,
                cancellationToken);
        }
    }
}
