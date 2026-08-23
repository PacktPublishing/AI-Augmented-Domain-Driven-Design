using BrewUp.Purchases.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Purchases.ReadModel.Dtos;

public class Beer : DtoBase
{
    public string BeerName { get; private set; } = string.Empty;
    public string BeerStyle { get; private set; } = string.Empty;
    public decimal AlcoholByVolume { get; private set; }
    public string Packaging { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    
    protected  Beer()
    {}

    public static Beer Create(BeerId beerId, BeerName beerName, BeerStyle beerStyle, AlcoholByVolume alcoholByVolume,
        Packaging packaging, Price price, bool isActive) => new(beerId.Value, beerName.Value, beerStyle.Value,
        alcoholByVolume.Value, packaging.Value, price.Value, isActive);

    private Beer(string beerId, string beerName, string beerStyle, decimal alcoholByVolume, string packaging,
        decimal price, bool isActive)
    {
        Id = beerId;
        BeerName = beerName;
        BeerStyle = beerStyle;
        AlcoholByVolume = alcoholByVolume;
        Packaging = packaging;
        Price = price;
        IsActive = isActive;
    }
    
    public BeerJson ToJson() => new()
    {
        BeerId = Id,
        BeerName = BeerName,
        BeerStyle = BeerStyle,
        AlcoholByVolume = AlcoholByVolume,
        Packaging = Packaging,
        Price = new Price(Price, string.Empty),
        IsActive = IsActive
    };
    
    public BeerType ToBeerType(Quantity quantity, Price price) => new(
        new BeerId(Id),
        new BeerName(BeerName),
        quantity, price);
}