using BrewUp.Payment.Domain.Entities;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Payment.Domain.CommandHandlers;

public sealed class RecordPaymentAuthorizedCommandHandler(
    IRepository repository,
    ILoggerFactory loggerFactory)
    : CommandHandlerAsync<RecordPaymentAuthorized>(repository, loggerFactory)
{
    public override async Task HandleAsync(
        RecordPaymentAuthorized command,
        CancellationToken cancellationToken = default)
    {
        var aggregate = await Repository
            .GetByIdAsync<PaymentAuthorization>(command.AggregateId, cancellationToken)
            .ConfigureAwait(false);

        aggregate!.RecordAuthorized(command.ProviderReference, command.MessageId);

        await Repository
            .SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken)
            .ConfigureAwait(false);
    }
}
