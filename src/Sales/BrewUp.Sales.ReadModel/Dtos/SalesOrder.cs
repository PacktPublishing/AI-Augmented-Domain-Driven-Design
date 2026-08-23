using BrewUp.Sales.ReadModel.Helpers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Sales.ReadModel.Dtos;

public class SalesOrder : DtoBase
{
    public string OrderNumber { get; private set; } = string.Empty;

    public string CustomerId { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;

    public DateTime OrderDate { get; private set; } = DateTime.MinValue;

    public IEnumerable<SalesOrderRow> Rows { get; private set; } = [];

    public string Status { get; private set; } = string.Empty;

    protected SalesOrder()
    { }

    public static SalesOrder CreateSalesOrder(SalesOrderId salesOrderId, SalesOrderNumber salesOrderNumber, CustomerId customerId,
        CustomerName customerName, SalesOrderDate orderDate, IEnumerable<SalesOrderRowJson> rows) => new(salesOrderId.Value,
        salesOrderNumber.Value, customerId.Value, customerName.Value, orderDate.Value, rows.ToReadModelEntities());

    private SalesOrder(string salesOrderId, string salesOrderNumber, string customerId, string customerName, DateTime orderDate, IEnumerable<SalesOrderRow> rows)
    {
        Id = salesOrderId;
        OrderNumber = salesOrderNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        OrderDate = orderDate;
        Rows = rows;

        Status = Shared.Helpers.OrderState.Created.Name;
    }
    
    public void AddBeers(IEnumerable<SalesOrderRowJson> rows)
    {
        Rows = rows.ToReadModelEntities();
    }

    public void CompleteOrder() => Status = Shared.Helpers.OrderState.Completed.Name;

    public SalesOrderJson ToJson() => new()
    {
        Id = Id,
        OrderNumber = OrderNumber,
        OrderDate = OrderDate,
        CustomerId = CustomerId,
        CustomerName = CustomerName,
        DeliveryDate = DateTime.MaxValue,
        Rows = Rows.Select(r => r.ToJson),
        Status = Status
    };
}