using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class CloseSalesOrder(
    SalesOrderId aggregateId,
    SalesOrderDeliveryDate salesOrderDeliveryDate,
    Account systemUser,
    Guid correlationId) : Command(aggregateId, correlationId, systemUser)
{
    public SalesOrderDeliveryDate SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
}