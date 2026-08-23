using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Commands;

namespace BrewUp.Sagas.SharedKernel.Messages.Commands;

public sealed class StartSalesOrderSaga(IntegrationId aggregateId, 
    Guid correlationId,
    string salesOrderNumber,
    DateTime salesOrderDate,
    string customerId,
    string warehouseId,
    DateTime salesOrderDeliveryDate,
    IEnumerable<SalesOrderRowJson> rows) : Command(aggregateId, correlationId)
{
    public Guid CorrelationId { get; private set; } = correlationId;
    public string SalesOrderNumber { get; private set; } = salesOrderNumber;
    public DateTime SalesOrderDate { get; private set; } = salesOrderDate;
    public string CustomerId { get; private set; } = customerId;
    public string WarehouseId { get; private set; } = warehouseId;
    public DateTime SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
    public List<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
}