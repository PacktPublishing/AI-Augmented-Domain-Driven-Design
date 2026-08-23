using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts;
using BrewUp.Shared.ExternalContracts.Sales;

namespace BrewUp.Sales.ReadModel.Dtos;

public class SalesOrderRow
{
    public string BeerId { get; private set; } = string.Empty;
    public string BeerName { get; private set; } = string.Empty;
    public Quantity Quantity { get; private set; } = default!;
    public Price Price { get; private set; } = default!;
    
    protected SalesOrderRow()
    {}
    
    internal static SalesOrderRow Create(BeerId beerId, BeerName beerName, Quantity quantity, Price price)
        => new SalesOrderRow(beerId.Value, beerName.Value, new Quantity(quantity.Value, quantity.UnitOfMeasure),
            new Price(price.Value, price.Currency));
    
    private SalesOrderRow(string beerId, string beerName, Quantity quantity, Price price)
    {
        BeerId = beerId;
        BeerName = beerName;
        Quantity = quantity;
        Price = price;
    }

    internal SalesOrderRowJson ToJson => new()
    {
        BeerId = BeerId,
        BeerName = BeerName,
        Quantity = Quantity,
        Price = Price
    };
}