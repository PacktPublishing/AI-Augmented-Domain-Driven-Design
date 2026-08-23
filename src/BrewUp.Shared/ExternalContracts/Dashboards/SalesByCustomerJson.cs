namespace BrewUp.Shared.ExternalContracts.Dashboards;

public class SalesByCustomerJson
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public string Currency { get; set; } = string.Empty;
}