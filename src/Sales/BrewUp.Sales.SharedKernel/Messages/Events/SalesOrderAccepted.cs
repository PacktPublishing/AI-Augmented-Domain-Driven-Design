using BrewUp.Sales.SharedKernel.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class SalesOrderAccepted(SalesOrderId aggregateId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
}