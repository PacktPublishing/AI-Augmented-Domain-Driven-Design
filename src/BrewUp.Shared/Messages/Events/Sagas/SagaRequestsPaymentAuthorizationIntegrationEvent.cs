using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class SagaRequestsPaymentAuthorizationIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    Price amount) : IntegrationEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public Price Amount { get; private set; } = amount;
}
