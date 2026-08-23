using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class SagaSalesOrderReadyToConfirmIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string paymentAuthorizationId,
    string stockReservationId) : IntegrationEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
    public string StockReservationId { get; private set; } = stockReservationId;
}
