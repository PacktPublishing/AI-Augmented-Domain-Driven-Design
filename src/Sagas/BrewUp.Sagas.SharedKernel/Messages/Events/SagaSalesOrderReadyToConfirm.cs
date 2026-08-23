using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

/// <summary>
/// Raised by the saga when both payment authorization and stock reservation have been received,
/// signaling that the ConfirmSalesOrder command should be dispatched.
/// </summary>
public sealed class SagaSalesOrderReadyToConfirm(IntegrationId aggregateId, Guid correlationId,
    string salesOrderId, string paymentAuthorizationId, string stockReservationId)
    : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
    public string StockReservationId { get; private set; } = stockReservationId;
}
