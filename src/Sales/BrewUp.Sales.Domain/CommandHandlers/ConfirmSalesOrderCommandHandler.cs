using BrewUp.Sales.Domain.Entities;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain.CommandHandlers;

public sealed class ConfirmSalesOrderCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<ConfirmSalesOrder>(repository, loggerFactory)
{
    public override async Task HandleAsync(ConfirmSalesOrder command, CancellationToken cancellationToken = new())
    {
        var aggregate = await Repository.GetByIdAsync<SalesOrder>(command.AggregateId, cancellationToken)
            .ConfigureAwait(false);
        aggregate!.ConfirmOrder(command.PaymentAuthorizationReference, command.StockReservationReference,
            command.MessageId);
        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
}
