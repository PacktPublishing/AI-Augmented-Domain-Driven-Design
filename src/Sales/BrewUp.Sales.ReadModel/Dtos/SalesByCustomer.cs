using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Sales.ReadModel.Dtos;

public class SalesByCustomer : DtoBase
{
    public string CustomerName { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; }
    
    protected SalesByCustomer() { }
    
    public static SalesByCustomer Create(CustomerId customerId, CustomerName customerName)
    {
        return new SalesByCustomer(customerId.Value, customerName.Value);
    }
    
    private SalesByCustomer(string customerId, string customerName)
    {
        Id  = customerId;
        CustomerName = customerName;
        TotalAmount = 0;
        CurrencyCode = "EUR";
    }
    
    public void IncreaseSalesAmount(Price price)
    {
        if (price.Currency != CurrencyCode)
            throw new InvalidOperationException($"Currency mismatch: expected {CurrencyCode}, got {price.Currency}");
        
        TotalAmount += price.Value;
    }
}