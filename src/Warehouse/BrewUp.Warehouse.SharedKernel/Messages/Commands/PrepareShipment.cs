using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands;

public sealed class PrepareShipment(ShipmentId aggregateId,
    SalesOrderId  salesOrderId,
    CustomerId customerId,
    DeliveryDate deliveryDate,
    IEnumerable<OrderRowDto> rows,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public SalesOrderId SalesOrderId { get; private set; } = salesOrderId;
    public CustomerId CustomerId { get; private set; } = customerId;
    public DeliveryDate DeliveryDate { get; private set; } = deliveryDate;
    public IEnumerable<OrderRowDto> Rows { get; private set; } = rows;
}