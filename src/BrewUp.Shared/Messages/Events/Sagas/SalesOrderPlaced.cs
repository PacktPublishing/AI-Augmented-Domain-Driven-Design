using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Sagas;

public sealed class SalesOrderPlaced(IntegrationId aggregateId, 
    Guid correlationId,
    string salesOrderNumber,
    DateTime salesOrderDate,
    string customerId,
    DateTime salesOrderDeliveryDate,
    IEnumerable<SalesOrderRowJson> rows) : IntegrationEvent(aggregateId, correlationId)
{
    public string SalesOrderNumber { get; private set; } = salesOrderNumber;
    public DateTime SalesOrderDate { get; private set; } = salesOrderDate;
    public string CustomerId { get; private set; } = customerId;
    public DateTime SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
    public List<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
}