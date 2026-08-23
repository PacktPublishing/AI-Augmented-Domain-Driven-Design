using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands;

public sealed class ReserveItemStock(
    AvailabilityId aggregateId,
    StockReservationId stockReservationId,
    SalesOrderId salesOrderId,
    Quantity quantity,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public StockReservationId StockReservationId { get; private set; } = stockReservationId;
    public SalesOrderId SalesOrderId { get; private set; } = salesOrderId;
    public Quantity Quantity { get; private set; } = quantity;
}
