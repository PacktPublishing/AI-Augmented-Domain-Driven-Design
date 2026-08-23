using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Payment.SharedKernel.Messages.Events;

public sealed class PaymentAuthorized(
    PaymentAuthorizationId aggregateId,
    string providerReference,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public string ProviderReference { get; } = providerReference;
}
