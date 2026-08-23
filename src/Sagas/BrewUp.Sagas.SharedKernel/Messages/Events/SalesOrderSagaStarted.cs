using BrewUp.Sagas.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SalesOrderSagaStarted(SagaId aggregateId, 
    Guid correlationId,
    string salesOrderNumber,
    DateTime salesOrderDate,
    string customerId,
    string warehouseId,
    DateTime salesOrderDeliveryDate,
    IEnumerable<SalesOrderRowJson> rows) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderNumber { get; private set; } = salesOrderNumber;
    public DateTime SalesOrderDate { get; private set; } = salesOrderDate;
    public string CustomerId { get; private set; } = customerId;
    public string WarehouseId { get; private set; } = warehouseId;
    public DateTime SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
    public List<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
}