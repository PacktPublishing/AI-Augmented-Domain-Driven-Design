using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class PaymentAuthorized(IntegrationId aggregateId, Guid correlationId,
    string paymentAuthorizationId) : IntegrationEvent(aggregateId, correlationId)
{
    public string PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
}
