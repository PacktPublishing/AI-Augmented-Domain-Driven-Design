using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.MasterData;

namespace BrewUp.MasterData.Domain.Helpers;

internal static class BeerHelper
{
    extension(Beer beer)
    {
        internal BeerCreated ToBeerCreated() => 
            new (new BeerId(beer.Id), new BeerName(beer.BeerName),
                new BeerStyle(beer.BeerStyle), new AlcoholByVolume(beer.AlcoholByVolume),
                new Packaging(beer.Packaging), new Price(beer.Price, "EUR"), beer.IsActive);

        internal BeerUpdated ToBeerUpdated() => 
            new (new BeerId(beer.Id), new BeerName(beer.BeerName),
                new BeerStyle(beer.BeerStyle), new AlcoholByVolume(beer.AlcoholByVolume),
                new Packaging(beer.Packaging), new Price(beer.Price, "EUR"), beer.IsActive);

        internal BeerDeleted ToBeerDeleted() => 
            new (new BeerId(beer.Id));
    }
}