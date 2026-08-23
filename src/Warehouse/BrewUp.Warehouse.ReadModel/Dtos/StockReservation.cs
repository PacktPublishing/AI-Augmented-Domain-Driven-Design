using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;

namespace BrewUp.Warehouse.ReadModel.Dtos;

public sealed class StockReservation : DtoBase
{
    public const string ReservedStatus = "reserved";
    public const string FailedStatus = "failed";

    public string WarehouseId { get; private set; } = string.Empty;
    public string SalesOrderId { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public IReadOnlyList<ItemRequested> Rows { get; private set; } = [];
    public string? FailureReason { get; private set; }

    private StockReservation()
    {
    }

    public static StockReservation CreateReserved(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        IEnumerable<ItemRequested> rows) =>
        new()
        {
            Id = reservationId.Value,
            WarehouseId = warehouseId.Value,
            SalesOrderId = salesOrderId.Value,
            Status = ReservedStatus,
            Rows = rows.ToArray()
        };

    public static StockReservation CreateFailed(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        string reason) =>
        new()
        {
            Id = reservationId.Value,
            WarehouseId = warehouseId.Value,
            SalesOrderId = salesOrderId.Value,
            Status = FailedStatus,
            Rows = [],
            FailureReason = reason
        };
}
