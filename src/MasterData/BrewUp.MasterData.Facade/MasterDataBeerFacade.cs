using BrewUp.MasterData.Domain.Services;
using BrewUp.MasterData.ReadModel.Services;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

internal sealed class MasterDataBeerFacade(IBeerDomainService beerDomainService,
    IBeerQueryService beerQueryService) : IMasterDataBeerFacade
{
    public Task<Result<string>> RegisterBeerAsync(RegisterBeerJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return beerDomainService.RegisterBeerAsync(new BeerId(Guid.CreateVersion7().ToString()),
            new BeerName(body.BeerName),
            new BeerStyle(body.BeerStyle),
            new AlcoholByVolume(body.AlcoholByVolume),
            new Packaging(body.Packaging),
            new Price(body.Price.Value, body.Price.Currency),
            body.IsActive, cancellationToken);
    }

    public Task<Result<PagedResult<BeerJson>>> GetBeersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken) => beerQueryService.GetBeersAsync(pageNumber, pageSize, cancellationToken);
}