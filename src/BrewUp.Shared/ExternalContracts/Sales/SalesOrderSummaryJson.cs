namespace BrewUp.Shared.ExternalContracts.Sales;

public class SalesOrderSummaryJson
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.MinValue;
    
    public decimal TotalAmount { get; set; } = decimal.Zero;
    public string Currency { get; set; } = string.Empty;
    
    public string Status { get; set; } = string.Empty;
}