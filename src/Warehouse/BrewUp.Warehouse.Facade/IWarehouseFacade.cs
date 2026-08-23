using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Warehouse.Facade;

public interface IWarehouseFacade
{
    Task<Result<PagedResult<ShipmentJson>>> GetShipmentOrdersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    Task<Result<string>> AddItemStockAsync(AddItemStockJson json, CancellationToken cancellationToken);
}