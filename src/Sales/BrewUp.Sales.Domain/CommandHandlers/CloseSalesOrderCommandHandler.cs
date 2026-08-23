using BrewUp.Sales.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain.CommandHandlers;

public sealed class CloseSalesOrderCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<CloseSalesOrder>(repository, loggerFactory)
{
    public override async Task HandleAsync(CloseSalesOrder command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var aggregate = await Repository.GetByIdAsync<Entities.SalesOrder>(command.AggregateId, cancellationToken);
        aggregate!.CloseSalesOrder(command.SalesOrderDeliveryDate, command.Who, command.MessageId);
        
        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken); 
    }
}