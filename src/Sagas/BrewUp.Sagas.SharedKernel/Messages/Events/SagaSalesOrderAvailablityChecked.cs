using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaSalesOrderAvailablityChecked(IntegrationId aggregateId,
    Guid correlationId, string salesOrderId, IEnumerable<ItemRequested> rows) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public IList<ItemRequested> Rows { get; private set; } = rows.ToList();
}