using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using PaymentProjection = BrewUp.Payment.ReadModel.Dtos.PaymentAuthorization;

namespace BrewUp.Payment.ReadModel.Services;

public sealed class PaymentAuthorizationService(
    [FromKeyedServices("payment")] IPersister persister)
    : IPaymentAuthorizationService
{
    public Task<Result<bool>> CreatePendingAsync(
        PaymentAuthorizationId authorizationId,
        SalesOrderReference salesOrder,
        CancellationToken cancellationToken) =>
        persister.InsertAsync(
            PaymentProjection.CreatePending(authorizationId, salesOrder),
            cancellationToken);

    public async Task<Result<bool>> MarkAuthorizedAsync(
        PaymentAuthorizationId authorizationId,
        string providerReference,
        CancellationToken cancellationToken)
    {
        var getResult = await persister
            .GetByIdAsync<PaymentProjection>(
                authorizationId.Value,
                cancellationToken)
            .ConfigureAwait(false);
        if (!getResult.IsSuccess)
            return Result<bool>.Error("Payment authorization projection not found.");

        getResult.TryGetValue(out PaymentProjection projection);
        if (string.IsNullOrEmpty(projection.Id))
            return Result<bool>.Error("Payment authorization projection not found.");

        projection.MarkAuthorized(providerReference);
        return await persister
            .UpdateAsync(projection, cancellationToken)
            .ConfigureAwait(false);
    }
}
