using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;

namespace BrewUp.MasterData.ReadModel.Services;

internal class BeerQueryService(ILoggerFactory loggerFactory, 
    IQueries<Beer> beerQueries)
    : ServiceBase(loggerFactory), IBeerQueryService
{
    public async Task<Result<PagedResult<BeerJson>>> GetBeersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await beerQueries.GetByFilterAsync(null, pageNumber, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<Beer> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<BeerJson>>.Success(new PagedResult<BeerJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<BeerJson>>.Success(new PagedResult<BeerJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<BeerJson>>.Error("Error retrieving beers"));
    }

    public async Task<Result<BeerJson>> GetBeerByIdAsync(string beerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await beerQueries.GetByIdAsync(beerId, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out Beer result);
                
                return Result<BeerJson>.Success(result.ToJson());
            },
            _ => Result<BeerJson>.Error($"Error retrieving beer with ID {beerId}"));
    }
}