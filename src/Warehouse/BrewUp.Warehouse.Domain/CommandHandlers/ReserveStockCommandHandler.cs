using BrewUp.Warehouse.Domain.Entities;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Domain.CommandHandlers;

public sealed class ReserveStockCommandHandler(
    IRepository repository,
    ILoggerFactory loggerFactory)
    : CommandHandlerAsync<ReserveStock>(repository, loggerFactory)
{
    public override async Task HandleAsync(
        ReserveStock command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var aggregate = await Repository
            .GetByIdAsync<StockReservation>(
                command.AggregateId,
                cancellationToken)
            .ConfigureAwait(false)
            ?? StockReservation.Create();

        aggregate.Reserve(
            new StockReservationId(command.AggregateId.Value),
            command.WarehouseId,
            command.SalesOrderId,
            command.Rows,
            command.MessageId);

        await Repository
            .SaveAsync(
                aggregate,
                Guid.CreateVersion7(),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
