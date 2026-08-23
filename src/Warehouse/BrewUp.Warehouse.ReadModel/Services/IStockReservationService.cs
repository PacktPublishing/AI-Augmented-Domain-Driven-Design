using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using Lena.Core;

namespace BrewUp.Warehouse.ReadModel.Services;

public interface IStockReservationService
{
    Task<Result<bool>> CreateReservedAsync(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        IReadOnlyList<ItemRequested> rows,
        CancellationToken cancellationToken);

    Task<Result<bool>> CreateFailedAsync(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        string reason,
        CancellationToken cancellationToken);

    Task<Result<decimal>> GetReservedQuantityAsync(
        WarehouseId warehouseId,
        BeerId beerId,
        string unitOfMeasure,
        CancellationToken cancellationToken);
}
