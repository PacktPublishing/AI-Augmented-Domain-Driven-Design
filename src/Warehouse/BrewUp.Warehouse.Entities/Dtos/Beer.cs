using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Warehouse.Entities.Dtos;

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
    
    private Beer(string beerId, string beerName, string beerStyle, decimal alcoholByVolume, string packaging, decimal price, bool isActive)
    {
        Id = beerId;
        BeerName = beerName;
        BeerStyle = beerStyle;
        AlcoholByVolume = alcoholByVolume;
        Packaging = packaging;
        Price = price;
        IsActive = isActive;
    }

    public void UpdateBeerName(BeerName beerName) => BeerName = beerName.Value;
    public void UpdateBeerStyle(BeerStyle beerStyle) => BeerStyle = beerStyle.Value;
    public void UpdateAlcoholByVolume(AlcoholByVolume alcoholByVolume) => AlcoholByVolume = alcoholByVolume.Value;
    public void UpdatePackaging(Packaging packaging) => Packaging = packaging.Value;
    public void UpdatePrice(Price price) => Price = price.Value;
    public void UpdateIsActive(bool isActive) => IsActive = isActive;
    
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
}