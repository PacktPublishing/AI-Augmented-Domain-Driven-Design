namespace BrewUp.Shared.ExternalContracts.Dashboards;

public class SalesByProductsJson
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    
    public decimal TotalSales { get; set; }
    public string Currency { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
}