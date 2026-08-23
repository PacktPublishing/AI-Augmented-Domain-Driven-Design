using BrewUp.Sales.Domain.Entities;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain.CommandHandlers;

public sealed class AddBeersToCartCommandHandler (IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<AddBeersToCart>(repository, loggerFactory)
{
    public override async Task HandleAsync(AddBeersToCart command, CancellationToken cancellationToken = new ())
    {
        var aggregate = await Repository.GetByIdAsync<SalesOrder>(command.AggregateId,  cancellationToken);
        aggregate!.AddBeers(command.Rows);
        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken);
    }
}