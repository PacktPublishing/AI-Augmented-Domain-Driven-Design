using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class PaymentAuthorizationFailed(IntegrationId aggregateId, Guid correlationId,
    string reason) : IntegrationEvent(aggregateId, correlationId)
{
    public string Reason { get; private set; } = reason;
}
