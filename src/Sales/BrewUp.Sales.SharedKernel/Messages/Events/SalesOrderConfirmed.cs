using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class SalesOrderConfirmed(
    SalesOrderId aggregateId,
    PaymentAuthorizationId paymentAuthorizationId,
    StockReservationId stockReservationId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public PaymentAuthorizationId PaymentAuthorizationId { get; } = paymentAuthorizationId;
    public StockReservationId StockReservationId { get; } = stockReservationId;
}
