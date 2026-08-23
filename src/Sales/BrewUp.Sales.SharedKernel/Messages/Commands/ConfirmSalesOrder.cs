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
    public PaymentAuthorizationId PaymentAuthorizationId { get; } = paymentAuthorizationId;
    public StockReservationId StockReservationId { get; } = stockReservationId;
}
