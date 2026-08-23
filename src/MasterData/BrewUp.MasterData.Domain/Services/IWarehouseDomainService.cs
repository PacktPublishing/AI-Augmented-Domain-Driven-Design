using BrewUp.Shared.ExternalContracts.Warehouse;
using Lena.Core;

namespace BrewUp.MasterData.Domain.Services;

public interface IWarehouseDomainService
{
    Task<Result<string>> CreateWarehouseAsync(CreateWarehouseJson body, CancellationToken cancellationToken);
}