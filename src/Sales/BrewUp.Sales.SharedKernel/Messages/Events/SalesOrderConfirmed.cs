using BrewUp.Sales.SharedKernel.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class SalesOrderConfirmed(SalesOrderId aggregateId,
    Guid correlationId,
    PaymentAuthorizationReference paymentAuthorizationReference,
    StockReservationReference stockReservationReference) : DomainEvent(aggregateId, correlationId)
{
    public PaymentAuthorizationReference PaymentAuthorizationReference { get; private set; } = paymentAuthorizationReference;
    public StockReservationReference StockReservationReference { get; private set; } = stockReservationReference;
}
