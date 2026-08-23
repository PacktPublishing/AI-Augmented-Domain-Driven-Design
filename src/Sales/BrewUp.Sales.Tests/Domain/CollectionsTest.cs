using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;

namespace BrewUp.Sales.Tests.Domain;

public class CollectionsTest
{
    [Fact]
    public void Compare_List()
    {
        BeerId beerId = new (Guid.CreateVersion7().ToString());
        IEnumerable<SalesOrderRowJson> first = new List<SalesOrderRowJson>
        {
            new ()
            {
                BeerId = beerId.ToString(),
                BeerName = "Beer 1",
                Quantity = new Quantity(2, "Bottles"),
                Price = new Price(0.1m, "USD")
            }
        };
        IEnumerable<SalesOrderRowJson> second = new List<SalesOrderRowJson>();
        
        second = second.Concat(new List<SalesOrderRowJson>
        {
            new ()
            {
                BeerId = beerId.ToString(),
                BeerName = "Beer 1",
                Quantity = new Quantity(2, "Bottles"),
                Price = new Price(0.1m, "USD")
            }
        });
        
        Assert.Equal(first, second);
    }
}