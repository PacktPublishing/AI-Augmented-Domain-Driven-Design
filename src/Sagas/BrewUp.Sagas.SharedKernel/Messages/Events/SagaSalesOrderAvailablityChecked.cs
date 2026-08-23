using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaSalesOrderAvailablityChecked(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    PaymentAuthorizationId paymentAuthorizationId,
    StockReservationId stockReservationId,
    IEnumerable<ItemRequested> rows) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public PaymentAuthorizationId PaymentAuthorizationId { get; private set; } = paymentAuthorizationId;
    public StockReservationId StockReservationId { get; private set; } = stockReservationId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}
