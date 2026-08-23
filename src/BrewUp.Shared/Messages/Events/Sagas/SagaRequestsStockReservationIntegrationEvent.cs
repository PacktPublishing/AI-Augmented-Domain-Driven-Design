using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class SagaRequestsStockReservationIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string warehouseId,
    IEnumerable<ItemRequested> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string WarehouseId { get; private set; } = warehouseId;
    public List<ItemRequested> Rows { get; private set; } = rows.ToList();
}
