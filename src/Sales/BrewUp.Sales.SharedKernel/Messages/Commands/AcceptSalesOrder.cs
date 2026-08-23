using BrewUp.Sales.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class AcceptSalesOrder(SalesOrderId aggregateId,
    Guid correlationId) : Command(aggregateId, correlationId)
{
}