using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using Lena.Core;

namespace BrewUp.Payment.ReadModel.Services;

public interface IPaymentAuthorizationService
{
    Task<Result<bool>> CreatePendingAsync(
        PaymentAuthorizationId authorizationId,
        SalesOrderReference salesOrder,
        CancellationToken cancellationToken);

    Task<Result<bool>> MarkAuthorizedAsync(
        PaymentAuthorizationId authorizationId,
        string providerReference,
        CancellationToken cancellationToken);
}
