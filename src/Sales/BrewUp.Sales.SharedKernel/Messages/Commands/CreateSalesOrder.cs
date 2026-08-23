using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class CreateSalesOrder(
    SalesOrderId aggregateId,
    SalesOrderNumber salesOrderNumber,
    SalesOrderDate salesOrderDate,
    Customer customer,
    SalesOrderDeliveryDate salesOrderDeliveryDate,
    IEnumerable<SalesOrderRowJson> rows,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public SalesOrderNumber SalesOrderNumber { get; private set; } = salesOrderNumber;
    public SalesOrderDate SalesOrderDate { get; private set; } = salesOrderDate;
    
    public Customer Customer { get; private set; } = customer;
    
    public SalesOrderDeliveryDate SalesOrderDeliveryDate { get; private set; } = salesOrderDeliveryDate;
    public IEnumerable<SalesOrderRowJson> Rows { get; private set; } = rows;
}