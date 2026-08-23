using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using Lena.Core;

namespace BrewUp.Warehouse.ReadModel.Services;

public interface IBeerService
{
    Task<Result<bool>> AddBeerAsync(BeerId beerId, BeerName beerName, BeerStyle style, AlcoholByVolume abv,
        Packaging packaging, Price price, bool isActive, CancellationToken cancellationToken = default);
    
    Task<Result<BeerJson>> GetBeerByIdAsync(BeerId beerId, CancellationToken cancellationToken = default);
}