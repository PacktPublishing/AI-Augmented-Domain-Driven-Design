using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Payment.SharedKernel.Messages.Commands;

public sealed class RecordPaymentAuthorized(
    PaymentAuthorizationId aggregateId,
    string providerReference,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public string ProviderReference { get; } = providerReference;
}
