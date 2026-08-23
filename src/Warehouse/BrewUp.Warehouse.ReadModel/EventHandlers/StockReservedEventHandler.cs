using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.ReadModel.EventHandlers;

public sealed class StockReservedEventHandler(
    IStockReservationService reservationService,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<StockReserved>(loggerFactory)
{
    public override async Task HandleAsync(
        StockReserved @event,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var persistenceResult = await reservationService
            .CreateReservedAsync(
                new StockReservationId(@event.AggregateId.Value),
                @event.WarehouseId,
                @event.SalesOrderId,
                @event.Rows,
                cancellationToken)
            .ConfigureAwait(false);
        var persisted = persistenceResult.Match(
            success => success,
            error => throw new PersistenceException(
                $"Failed to persist reserved stock projection: {error}"));
        if (!persisted)
            throw new PersistenceException("Failed to persist reserved stock projection.");
    }
}
