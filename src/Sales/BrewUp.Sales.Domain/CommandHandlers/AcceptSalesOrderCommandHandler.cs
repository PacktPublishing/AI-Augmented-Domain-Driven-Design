using BrewUp.Sales.Domain.Entities;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain.CommandHandlers;

public sealed class AcceptSalesOrderCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<AcceptSalesOrder>(repository, loggerFactory)
{
    public override async Task HandleAsync(AcceptSalesOrder command, CancellationToken cancellationToken = new CancellationToken())
    {
        var aggregate = await Repository.GetByIdAsync<SalesOrder>(command.AggregateId, cancellationToken)
            .ConfigureAwait(false);
        aggregate!.AcceptOrder(command.MessageId);

        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
}