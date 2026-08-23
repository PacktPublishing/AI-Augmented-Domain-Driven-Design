using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.Domain.Entities;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Domain.CommandHandlers
{
    public sealed class CreateAvailabilityCommandHandler(IRepository repository,
        ILoggerFactory loggerFactory) : CommandHandlerAsync<CreateAvailability>(repository, loggerFactory)
    {
        public override async Task HandleAsync(CreateAvailability command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var aggregate = Availability.Create(
                new AvailabilityId(command.AggregateId.Value),
                command.WarehouseId,
                command.BeerId,
                new Quantity(command.Quantity.Value, command.Quantity.UnitOfMeasure));

            await Repository.SaveAsync(aggregate, Guid.NewGuid(), cancellationToken);
        }
    }
}
