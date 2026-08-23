using BrewUp.Shared.ExternalContracts.Sagas;
using Lena.Core;

namespace BrewUp.Sagas.Facade;

public interface ISagasFacade
{
    Task<Result<string>> PlaceSalesOrderAsync(PlaceSalesOrderJson body, CancellationToken cancellationToken);
}