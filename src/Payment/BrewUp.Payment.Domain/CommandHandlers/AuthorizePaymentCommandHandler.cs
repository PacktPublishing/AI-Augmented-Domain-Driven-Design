using BrewUp.Payment.Domain.Entities;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Payment.Domain.CommandHandlers;

public sealed class AuthorizePaymentCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<AuthorizePayment>(repository, loggerFactory)
{
    public override async Task HandleAsync(AuthorizePayment command, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var aggregate = PaymentAuthorization.Create(
            new PaymentAuthorizationId(command.AggregateId.Value),
            command.SalesOrderId,
            command.Amount,
            command.MessageId);

        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
}
