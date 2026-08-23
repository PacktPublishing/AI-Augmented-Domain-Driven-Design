using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Payment.SharedKernel.Messages.Events;

public sealed class PaymentAuthorizedIntegrationEvent(
    PaymentAuthorizationId aggregateId,
    string providerReference,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public string ProviderReference { get; } = providerReference;
}
