using System.Runtime.InteropServices;
using BrewUp.Sales.Domain.Entities;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Price = BrewUp.Sales.Domain.Entities.Price;
using Quantity = BrewUp.Sales.Domain.Entities.Quantity;

namespace BrewUp.Sales.Domain.Mappers;

internal static class DomainConverter
{
    internal static SalesOrderRow ToEntity(this SalesOrderRowJson json)
    {
        return SalesOrderRow.Create(new BeerId(json.BeerId), new BeerName(json.BeerName), 
            new Quantity(json.Quantity.Value, json.Quantity.UnitOfMeasure), 
            new Price(json.Price.Value, json.Price.Currency));
    }

    internal static SalesOrderRowJson ToJson(this SalesOrderRow entity)
    {
        return new SalesOrderRowJson
        {
            BeerId = entity._beerId.Value,
            BeerName = entity._beerName.Value,
            Quantity = new BrewUp.Shared.CustomTypes.Quantity(entity._quantity.Value, entity._quantity.UnitOfMeasure),
            Price = new BrewUp.Shared.CustomTypes.Price(entity._price.Value, entity._price.Currency)
        };
    }
}