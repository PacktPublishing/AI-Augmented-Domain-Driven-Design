using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Sales.ReadModel.Dtos;

public class SalesOrderSummary : DtoBase
{
    public string OrderNumber { get; private set; } = string.Empty;

    public string CustomerId { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;

    public DateTime OrderDate { get; private set; } = DateTime.MinValue;
    
    public decimal TotalAmount { get; private set; } = decimal.Zero;
    public string Currency { get; private set; } = string.Empty;
    
    public string Status { get; private set; } = string.Empty;
    
    protected SalesOrderSummary()
    { }

    public static SalesOrderSummary Create(SalesOrderId salesOrderId, SalesOrderNumber orderNumber,
        CustomerId customerId, CustomerName customerName, SalesOrderDate orderDate, Price totalAmount,
        SalesOrderStatus status)
    {
        return new SalesOrderSummary(salesOrderId.Value, orderNumber.Value, customerId.Value, customerName.Value,
            orderDate.Value, totalAmount.Value, totalAmount.Currency, status.Name);
    }

    private SalesOrderSummary(string salesOrderId, string salesOrderNumber, string customerId, string customerName,
        DateTime orderDate, decimal totalAmount, string currency, string status)
    {
        Id = salesOrderId;
        OrderNumber = salesOrderNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        OrderDate = orderDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
    }
    
    public SalesOrderSummaryJson ToJson() => new ()
    {
        Id = Id,
        OrderNumber = OrderNumber,
        CustomerId = CustomerId,
        CustomerName = CustomerName,
        OrderDate = OrderDate,
        TotalAmount = TotalAmount,
        Currency = Currency,
        Status = Status
    };
}