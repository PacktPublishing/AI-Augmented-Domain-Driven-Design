using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Payment.SharedKernel.Messages.Events;

public sealed class PaymentDeclined(PaymentAuthorizationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string reason) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string Reason { get; private set; } = reason;
}
