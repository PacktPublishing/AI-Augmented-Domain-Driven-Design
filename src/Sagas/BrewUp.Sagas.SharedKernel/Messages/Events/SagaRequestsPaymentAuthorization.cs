using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

/// <summary>
/// Raised by SalesOrderSaga when it initiates the payment authorization request.
/// Read model publishes SagaRequestsPaymentAuthorizationIntegrationEvent for the Payment ACL.
/// </summary>
public sealed class SagaRequestsPaymentAuthorization(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    Price amount) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public Price Amount { get; private set; } = amount;
}
