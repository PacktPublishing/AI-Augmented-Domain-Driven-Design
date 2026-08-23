using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Facade.Acl;

public sealed class StockReservationRequestedIntegrationEventHandler(
    IServiceBus serviceBus,
    IAvailabilityService availabilityService,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<StockReservationRequestedIntegrationEvent>(
        loggerFactory)
{
    public override async Task HandleAsync(
        StockReservationRequestedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var assessedRows = new List<ItemRequested>(@event.Rows.Count);
        foreach (var row in @event.Rows)
        {
            var result = await availabilityService
                .GetAvailabilityByWarehouseIdAndBeerIdAsync(
                    @event.WarehouseId,
                    row.BeerId,
                    cancellationToken)
                .ConfigureAwait(false);

            var available = new Quantity(
                0,
                row.QuantityOrdered.UnitOfMeasure);
            if (result.IsSuccess)
            {
                result.TryGetValue(out AvailabilityJson projection);
                available = new Quantity(
                    projection.Quantity,
                    projection.UnitOfMeasure);
            }

            assessedRows.Add(row with
            {
                QuantityAvailable = available
            });
        }

        await serviceBus
            .SendAsync(
                new ReserveStock(
                    new StockReservationId(@event.AggregateId.Value),
                    @event.WarehouseId,
                    @event.SalesOrderId,
                    assessedRows,
                    MessageHelpers.GetCorrelationId(@event)),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
