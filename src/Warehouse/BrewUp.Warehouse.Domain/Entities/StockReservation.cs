using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Muflone.Core;

namespace BrewUp.Warehouse.Domain.Entities;

public class StockReservation : AggregateRoot
{
    private const string UnavailableReason = "All requested stock is not available.";
    private bool _isTerminal;

    protected StockReservation()
    {
    }

    internal static StockReservation Create() => new();

    internal void Reserve(
        StockReservationId reservationId,
        WarehouseId warehouseId,
        SalesOrderId salesOrderId,
        IEnumerable<ItemRequested> rows,
        Guid correlationId)
    {
        if (_isTerminal)
            return;

        var assessedRows = rows.ToArray();
        var hasUnitMismatch = assessedRows.Any(row =>
            !string.Equals(
                row.QuantityOrdered.UnitOfMeasure,
                row.QuantityAvailable.UnitOfMeasure,
                StringComparison.Ordinal));
        var hasUnavailableGroup = assessedRows
            .GroupBy(row => (
                warehouseId.Value,
                row.BeerId.Value,
                row.QuantityOrdered.UnitOfMeasure))
            .Any(group =>
                group.Sum(row => row.QuantityOrdered.Value) >
                group.First().QuantityAvailable.Value);

        if (hasUnitMismatch || hasUnavailableGroup)
        {
            RaiseEvent(new StockReservationFailed(
                reservationId,
                warehouseId,
                salesOrderId,
                UnavailableReason,
                correlationId));
            return;
        }

        RaiseEvent(new StockReserved(
            reservationId,
            warehouseId,
            salesOrderId,
            assessedRows,
            correlationId));
    }

    private void Apply(StockReserved @event)
    {
        Id = @event.AggregateId;
        _isTerminal = true;
    }

    private void Apply(StockReservationFailed @event)
    {
        Id = @event.AggregateId;
        _isTerminal = true;
    }
}
