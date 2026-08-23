using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Enums;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events;

public sealed class ShipmentPendingForPreparation(ShipmentId aggregateId,
    SalesOrderId  salesOrderId,
    CustomerId customerId,
    DeliveryDate deliveryDate,
    IEnumerable<OrderRowDto> rows,
    ShipmentState shipmentState,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public SalesOrderId SalesOrderId { get; private set; } = salesOrderId;
    public CustomerId CustomerId { get; private set; } = customerId;
    public DeliveryDate DeliveryDate { get; private set; } = deliveryDate;
    public IEnumerable<OrderRowDto> Rows { get; private set; } = rows;
    public ShipmentState ShipmentState { get; private set; } = shipmentState;
}