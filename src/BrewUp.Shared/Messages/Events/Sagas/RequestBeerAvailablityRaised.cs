using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class RequestBeerAvailablityRaised(IntegrationId aggregateId,
    Guid correlationId, string warehouseId,
    IEnumerable<ItemRequested> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public string WarehouseId { get; private set; } = warehouseId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}