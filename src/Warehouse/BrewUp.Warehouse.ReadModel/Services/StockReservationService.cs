using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using ReservationProjection = BrewUp.Warehouse.ReadModel.Dtos.StockReservation;

namespace BrewUp.Warehouse.ReadModel.Services;

public sealed class StockReservationService(
    [FromKeyedServices("warehouse")] IPersister persister,
    IQueries<ReservationProjection> queries) : IStockReservationService
{
    public Task<Result<bool>> CreateReservedAsync(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        IReadOnlyList<ItemRequested> rows,
        CancellationToken cancellationToken) =>
        persister.InsertAsync(
            ReservationProjection.CreateReserved(
                reservationId,
                warehouseId,
                salesOrderId,
                rows),
            cancellationToken);

    public Task<Result<bool>> CreateFailedAsync(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        string reason,
        CancellationToken cancellationToken) =>
        persister.InsertAsync(
            ReservationProjection.CreateFailed(
                reservationId,
                warehouseId,
                salesOrderId,
                reason),
            cancellationToken);

    public async Task<Result<decimal>> GetReservedQuantityAsync(
        WarehouseId warehouseId,
        BeerId beerId,
        string unitOfMeasure,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryResult = await queries
            .GetByFilterAsync(
                reservation =>
                    reservation.WarehouseId == warehouseId.Value &&
                    reservation.Status == ReservationProjection.ReservedStatus,
                1,
                int.MaxValue,
                cancellationToken)
            .ConfigureAwait(false);
        if (!queryResult.IsSuccess)
            return Result<decimal>.Error("Error retrieving stock reservations.");

        queryResult.TryGetValue(out PagedResult<ReservationProjection> reservations);
        var reservedQuantity = reservations.Results
            .SelectMany(reservation => reservation.Rows)
            .Where(row =>
                row.BeerId.Value == beerId.Value &&
                row.QuantityOrdered.UnitOfMeasure == unitOfMeasure)
            .Sum(row => row.QuantityOrdered.Value);

        return Result<decimal>.Success(reservedQuantity);
    }
}
