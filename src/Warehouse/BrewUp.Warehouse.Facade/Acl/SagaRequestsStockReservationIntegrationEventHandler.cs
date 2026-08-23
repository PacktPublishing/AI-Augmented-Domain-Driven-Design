using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Facade.Acl;

public sealed class SagaRequestsStockReservationIntegrationEventHandler(
    IServiceBus serviceBus,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SagaRequestsStockReservationIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SagaRequestsStockReservationIntegrationEvent @event,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Warehouse is keyed by WarehouseId — Warehouse BC owns stock reservation (BC-005/BC-006)
        var command = new ReserveStock(
            new WarehouseId(@event.WarehouseId),
            @event.MessageId,
            @event.SalesOrderId,
            @event.Rows);

        await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
    }
}
