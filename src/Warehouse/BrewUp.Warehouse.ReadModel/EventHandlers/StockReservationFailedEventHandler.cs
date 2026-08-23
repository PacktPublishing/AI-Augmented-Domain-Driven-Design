using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.ReadModel.EventHandlers;

public sealed class StockReservationFailedEventHandler(
    IStockReservationService reservationService,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<StockReservationFailed>(loggerFactory)
{
    public override async Task HandleAsync(
        StockReservationFailed @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var persistenceResult = await reservationService
            .CreateFailedAsync(
                new StockReservationId(@event.AggregateId.Value),
                @event.WarehouseId,
                @event.SalesOrderId,
                @event.Reason,
                cancellationToken)
            .ConfigureAwait(false);
        var persisted = persistenceResult.Match(
            success => success,
            error => throw new PersistenceException(
                $"Failed to persist failed stock reservation projection: {error}"));
        if (!persisted)
        {
            throw new PersistenceException(
                "Failed to persist failed stock reservation projection.");
        }
    }
}
