using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using Lena.Core;
using Muflone.Persistence;

namespace BrewUp.Payment.Facade;

public sealed class PaymentFacade(IServiceBus serviceBus) : IPaymentFacade
{
    public async Task<Result<string>> RecordAuthorizedAsync(
        PaymentAuthorizationId authorizationId,
        string providerReference,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await serviceBus
            .SendAsync(
                new RecordPaymentAuthorized(
                    authorizationId,
                    providerReference,
                    Guid.CreateVersion7()),
                cancellationToken)
            .ConfigureAwait(false);

        return Result<string>.Success(authorizationId.Value);
    }
}
