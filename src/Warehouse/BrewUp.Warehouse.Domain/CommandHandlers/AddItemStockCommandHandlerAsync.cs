using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Domain.CommandHandlers
{
    public sealed class AddItemStockCommandHandlerAsync(IRepository repository,
        ILoggerFactory loggerFactory) : CommandHandlerAsync<AddItemStock>(repository, loggerFactory)
    {
        public override async Task HandleAsync(AddItemStock command, CancellationToken cancellationToken = default)
        {
            var aggregate = await Repository.GetByIdAsync<Entities.Availability>(command.AggregateId, cancellationToken);
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            aggregate.AddItemStock(new Entities.Quantity(command.Quantity.Value, command.Quantity.UnitOfMeasure));

            await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken);
        }
    }
}
