using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class SalesOrderCreated(
    SalesOrderId aggregateId,
    SalesOrderNumber salesOrderNumber,
    SalesOrderDate salesOrderDate,
    Customer customer,
    SalesOrderDeliveryDate salesOrderDeliveryDate,
    IEnumerable<SalesOrderRowJson> rows,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public SalesOrderNumber SalesOrderNumber { get; private set; } = salesOrderNumber;
    public SalesOrderDate SalesOrderDate { get; private set; } = salesOrderDate;
    
    public Customer Customer { get; private set; } = customer;
    
    public SalesOrderDeliveryDate SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
    public IEnumerable<SalesOrderRowJson> Rows { get; private set; } = rows;
}