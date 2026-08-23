using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class PaymentDeclinedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string reason) : IntegrationEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string Reason { get; private set; } = reason;
}
