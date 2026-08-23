using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

internal interface IMasterDataBeerFacade
{
    Task<Result<string>> RegisterBeerAsync(RegisterBeerJson body, CancellationToken cancellationToken);
    Task<Result<PagedResult<BeerJson>>> GetBeersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}