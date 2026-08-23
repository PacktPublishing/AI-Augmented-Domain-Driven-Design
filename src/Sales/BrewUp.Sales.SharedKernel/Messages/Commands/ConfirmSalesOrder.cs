using BrewUp.Sales.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class ConfirmSalesOrder(SalesOrderId aggregateId,
    Guid correlationId,
    PaymentAuthorizationReference paymentAuthorizationReference,
    StockReservationReference stockReservationReference) : Command(aggregateId, correlationId)
{
    public PaymentAuthorizationReference PaymentAuthorizationReference { get; } = paymentAuthorizationReference;
    public StockReservationReference StockReservationReference { get; } = stockReservationReference;
}
