using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.MasterData;

public sealed class BeerUpdated(BeerId aggregateId,
    BeerName beerName,
    BeerStyle beerStyle,
    AlcoholByVolume alcoholByVolume,
    Packaging packaging,
    Price price,
    bool isActive) : IntegrationEvent(aggregateId)
{
    public BeerName BeerName { get; private set; } = beerName;
    public BeerStyle BeerStyle { get; private set; } = beerStyle;
    public AlcoholByVolume AlcoholByVolume { get; private set; } = alcoholByVolume;
    public Packaging Packaging { get; private set; } = packaging;
    public Price Price { get; private set; } = price;
    public bool IsActive { get; private set; } = isActive;
}
