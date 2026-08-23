using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class SagaSalesOrderAvailabilityCheckedIntegrationEvent(
    IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    IEnumerable<ItemRequested> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}