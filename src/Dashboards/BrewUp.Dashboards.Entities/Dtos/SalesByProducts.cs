using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Dashboards.Entities.Dtos;

public class SalesByProducts : DtoBase
{
    public string ProductName { get; private set; } = string.Empty;
    public string Year { get; private set; } = string.Empty;
    public decimal TotalSales { get; private set; } = 0;
    public string Currency { get; private set; } = string.Empty;
    public decimal TotalQuantity { get; private set; } = 0;
    public string UnitOfMeasure { get; private set; } = string.Empty;
    
    protected SalesByProducts()
    {
    }
    
    public static SalesByProducts Create(BeerId beerId, BeerName beerName, SalesOrderYear year)
    {
        return new SalesByProducts(beerId.Value, beerName.Value, year.Value);
    }
    
    private SalesByProducts(string beerId, string beerName, string year)
    {
        Id = beerId;
        ProductName = beerName;
        Year = year;
        
        TotalSales = 0;
        Currency = string.Empty;
        
        TotalQuantity = 0;
        UnitOfMeasure = string.Empty;
    }
    
    public void UpdateTotalSales(SalesOrderValue totalSales, SalesOrderQuantity totalQuantity)
    {
        TotalSales += totalSales.Value;
        Currency = totalSales.Currency;
        
        TotalQuantity += totalQuantity.Value;
        UnitOfMeasure = totalQuantity.UnitOfMeasure;
    }
    
    public SalesByProductsJson ToJson()
    {
        return new SalesByProductsJson
        {
            ProductId = Id,
            ProductName = ProductName,
            Year = Year,
            TotalSales = (decimal)TotalSales,
            Currency = Currency,
            Quantity = (decimal)TotalQuantity,
            UnitOfMeasure = UnitOfMeasure
        };
    }
}