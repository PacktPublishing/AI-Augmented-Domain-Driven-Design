using BrewUp.Warehouse.Domain.Entities;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Domain.CommandHandlers;

public sealed class ReserveItemStockCommandHandler(
    IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<ReserveItemStock>(repository, loggerFactory)
{
    public override async Task HandleAsync(
        ReserveItemStock command,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var aggregate = await Repository
            .GetByIdAsync<Availability>(command.AggregateId, cancellationToken)
            .ConfigureAwait(false);

        aggregate!.ReserveItemStock(command.StockReservationId, command.SalesOrderId, command.Quantity,
            command.MessageId);

        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
}
