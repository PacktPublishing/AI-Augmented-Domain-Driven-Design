using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Lena.Core;

namespace BrewUp.MasterData.Domain.Services;

public interface IBeerDomainService
{
    Task<Result<string>> RegisterBeerAsync(BeerId beerId, BeerName beerName, BeerStyle style, AlcoholByVolume abv, Packaging packaging,
        Price price, bool isActive, CancellationToken cancellationToken = default);
        
    Task<Result<bool>> SaveBeerAsync(BeerId beerId, BeerName beerName, BeerStyle style, AlcoholByVolume abv, Packaging packaging,
        Price price, bool isActive, CancellationToken cancellationToken = default);
        
    Task<Result<bool>> DeleteBeerAsync(BeerId beerId, CancellationToken cancellationToken = default);
}