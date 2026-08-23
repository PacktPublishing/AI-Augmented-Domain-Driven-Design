using BrewUp.Shared.ExternalContracts.Purchases;
using Lena.Core;

namespace BrewUp.Purchases.Facade;

public interface IPurchasesFacade
{
    Task<Result<string>> CreatePurchaseOrderAsync(CreatePurchaseOrderJson body, CancellationToken cancellationToken);
}