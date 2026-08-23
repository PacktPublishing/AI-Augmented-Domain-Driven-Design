using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

/// <summary>
/// Raised when Payment BC declines the payment authorization.
/// The saga records this outcome without compensation (out of scope per OQ-4).
/// </summary>
public sealed class SagaPaymentDeclined(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string reason) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string Reason { get; private set; } = reason;
}
