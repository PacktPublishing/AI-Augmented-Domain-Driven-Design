using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Core;

namespace BrewUp.Sales.Domain.Entities;

public class SalesOrderRow : Entity
{
    internal BeerId _beerId;
    internal BeerName _beerName;
    internal Quantity _quantity;
    internal Price _price;
    
    protected SalesOrderRow()
    {}

    internal static SalesOrderRow Create(BeerId beerId, BeerName beerName, Quantity quantity, Price price)
    {
        // Controlli
        return new SalesOrderRow(beerId, beerName, quantity, price);
    }
    
    private SalesOrderRow(BeerId beerId, BeerName beerName, Quantity quantity, Price price)
    {
        _beerId = beerId;
        _beerName = beerName;
        _quantity = quantity;
        _price = price;
    }
    
    
}