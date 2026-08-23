using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

/// <summary>
/// Gate event — raised by SalesOrderSaga when both PaymentAuthorized and StockReserved evidence is present.
/// Triggers final ConfirmSalesOrder in the Sales module.
/// </summary>
public sealed class SagaSalesOrderReadyToConfirm(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string paymentAuthorizationId,
    string stockReservationId) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
    public string StockReservationId { get; private set; } = stockReservationId;
}
