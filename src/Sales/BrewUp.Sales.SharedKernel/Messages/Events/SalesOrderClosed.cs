using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class SalesOrderClosed(
    SalesOrderId aggregateId,
    SalesOrderDeliveryDate salesOrderDeliveryDate,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public SalesOrderDeliveryDate SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
}