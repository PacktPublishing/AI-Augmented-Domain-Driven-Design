using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events;

public sealed class ItemStockReserved(
    AvailabilityId aggregateId,
    StockReservationId stockReservationId,
    SalesOrderId salesOrderId,
    Quantity quantity,
    Quantity remainingQuantity,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public StockReservationId StockReservationId { get; private set; } = stockReservationId;
    public SalesOrderId SalesOrderId { get; private set; } = salesOrderId;
    public Quantity Quantity { get; private set; } = quantity;
    public Quantity RemainingQuantity { get; private set; } = remainingQuantity;
}
