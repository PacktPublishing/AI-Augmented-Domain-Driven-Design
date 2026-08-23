using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Payment.SharedKernel.Messages.Events;

public sealed class PaymentAuthorized(PaymentAuthorizationId aggregateId,
    Guid correlationId,
    string salesOrderId) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
}
