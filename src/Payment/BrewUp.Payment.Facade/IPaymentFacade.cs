using BrewUp.Payment.SharedKernel.DomainIds;
using Lena.Core;

namespace BrewUp.Payment.Facade;

public interface IPaymentFacade
{
    Task<Result<string>> RecordAuthorizedAsync(
        PaymentAuthorizationId authorizationId,
        string providerReference,
        CancellationToken cancellationToken);
}
