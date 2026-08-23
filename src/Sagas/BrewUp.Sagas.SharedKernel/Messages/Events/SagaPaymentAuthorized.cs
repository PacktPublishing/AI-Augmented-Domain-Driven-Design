using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaPaymentAuthorized(IntegrationId aggregateId, Guid correlationId,
    string paymentAuthorizationId) : DomainEvent(aggregateId, correlationId)
{
    public string PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
}
