using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaStockReserved(IntegrationId aggregateId, Guid correlationId,
    string stockReservationId, IEnumerable<ItemRequested> rows) : DomainEvent(aggregateId, correlationId)
{
    public string StockReservationId { get; private set; } = stockReservationId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}
