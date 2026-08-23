using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaSalesOrderPlaced(IntegrationId aggregateId,
    Guid correlationId,
    string warehouseId,
    IEnumerable<SalesOrderRowJson> rows) : DomainEvent(aggregateId, correlationId)
{
    public string WarehouseId { get; private set; } = warehouseId;
    public IList<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
}