using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Dashboards.Entities.Dtos;

public class SalesByCustomers : DtoBase
{
    public string CustomerName { get; private set; } = string.Empty;
    public string Year { get; private set; } = string.Empty;
    public decimal TotalSales { get; private set; } = 0;
    public string Currency { get; private set; } = string.Empty;
    
    protected SalesByCustomers()
    {
    }
    
    public static SalesByCustomers Create(CustomerId customerId, CustomerName customerName, SalesOrderYear year)
    {
        return new SalesByCustomers(customerId.Value, customerName.Value, year.Value);
    }

    private SalesByCustomers(string customerId, string customerName, string year)
    {
        Id = customerId;
        CustomerName = customerName;
        Year = year;
        
        TotalSales = 0;
        Currency = string.Empty;
    }
    
    public void UpdateTotalSales(SalesOrderValue totalSales)
    {
        TotalSales += totalSales.Value;
        Currency = totalSales.Currency;
    }
    
    public SalesByCustomerJson ToJson()
    {
        return new SalesByCustomerJson
        {
            CustomerId = Id,
            CustomerName = CustomerName,
            Year = Year,
            TotalSales = (decimal)TotalSales,
            Currency = Currency
        };
    }
}