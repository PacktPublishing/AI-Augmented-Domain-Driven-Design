using BrewUp.Payment.Domain.Entities;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Payment.Domain.CommandHandlers;

public sealed class RequestPaymentAuthorizationCommandHandler(
    IRepository repository,
    ILoggerFactory loggerFactory)
    : CommandHandlerAsync<RequestPaymentAuthorization>(repository, loggerFactory)
{
    public override async Task HandleAsync(
        RequestPaymentAuthorization command,
        CancellationToken cancellationToken = default)
    {
        var aggregate = await Repository
            .GetByIdAsync<PaymentAuthorization>(command.AggregateId, cancellationToken)
            .ConfigureAwait(false)
            ?? PaymentAuthorization.Create();

        aggregate.Request(
            new PaymentAuthorizationId(command.AggregateId.Value),
            command.SalesOrder,
            command.MessageId);

        await Repository
            .SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken)
            .ConfigureAwait(false);
    }
}
