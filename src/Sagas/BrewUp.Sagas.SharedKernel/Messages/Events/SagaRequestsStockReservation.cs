using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

/// <summary>
/// Raised by SalesOrderSaga when it initiates the stock reservation request.
/// Read model publishes SagaRequestsStockReservationIntegrationEvent for the Warehouse ACL.
/// </summary>
public sealed class SagaRequestsStockReservation(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string warehouseId,
    IEnumerable<SalesOrderRowJson> rows) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string WarehouseId { get; private set; } = warehouseId;
    public IList<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
}
