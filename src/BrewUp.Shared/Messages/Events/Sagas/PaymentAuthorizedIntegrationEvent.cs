using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class PaymentAuthorizedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId,
    string paymentAuthorizationId,
    string salesOrderId) : IntegrationEvent(aggregateId, correlationId)
{
    public string PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
    public string SalesOrderId { get; private set; } = salesOrderId;
}
