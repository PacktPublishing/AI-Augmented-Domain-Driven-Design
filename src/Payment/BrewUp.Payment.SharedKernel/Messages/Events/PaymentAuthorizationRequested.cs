using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Payment.SharedKernel.Messages.Events;

public sealed class PaymentAuthorizationRequested(
    PaymentAuthorizationId aggregateId,
    SalesOrderReference salesOrder,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public SalesOrderReference SalesOrder { get; } = salesOrder;
}
