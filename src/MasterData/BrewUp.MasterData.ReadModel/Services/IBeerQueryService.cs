using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.ReadModel.Services;

public interface IBeerQueryService
{
    Task<Result<PagedResult<BeerJson>>> GetBeersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);
    Task<Result<BeerJson>> GetBeerByIdAsync(string beerId, CancellationToken cancellationToken);
}