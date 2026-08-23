using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.Domain.Entities;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Domain.CommandHandlers;

public sealed class ReserveStockCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<ReserveStock>(repository, loggerFactory)
{
    public override async Task HandleAsync(ReserveStock command, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Load the Availability aggregate keyed by WarehouseId value (per-beer availability)
        var aggregate = await Repository
            .GetByIdAsync<Availability>(new AvailabilityId(command.AggregateId.Value), cancellationToken)
            .ConfigureAwait(false);

        if (aggregate is null)
            throw new InvalidOperationException($"Availability not found for warehouse {command.AggregateId.Value}");

        aggregate.ReserveStock(command.Rows, command.SalesOrderId, command.MessageId);

        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
}
