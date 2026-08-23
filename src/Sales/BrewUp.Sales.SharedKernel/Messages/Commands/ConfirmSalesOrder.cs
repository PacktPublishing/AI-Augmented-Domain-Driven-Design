using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class ConfirmSalesOrder(
    SalesOrderId aggregateId,
    PaymentAuthorizationId paymentAuthorizationId,
    StockReservationId stockReservationId,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public PaymentAuthorizationId PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
    public StockReservationId StockReservationId { get; private set; } = stockReservationId;
}
