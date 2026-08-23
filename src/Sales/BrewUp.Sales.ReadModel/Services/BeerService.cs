using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Sales.ReadModel.Services;

internal sealed class BeerService([FromKeyedServices("sales")] IPersister persister,
    IQueries<Beer> beersQuery,
    ILoggerFactory loggerFactory) 
    : ServiceBase(persister, loggerFactory),IBeerService
{
    public async Task<Result<bool>> AddBeerAsync(BeerId beerId, BeerName beerName, BeerStyle style, AlcoholByVolume abv, Packaging packaging,
        Price price, bool isActive, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var beer = Beer.Create(beerId, beerName, style, abv, packaging, price, isActive);
        var insertResult = await Persister.InsertAsync(beer, cancellationToken);

        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating beer: {Error}", error);
                return Result<bool>.Error($"Error creating beer: {error}");
            });
    }

    public async Task<Result<BeerJson>> GetBeerByIdAsync(BeerId beerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryResult = await beersQuery.GetByIdAsync(beerId.Value, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out Beer beer);
                return Result<BeerJson>.Success(beer.ToJson());
            },
            _ => Result<BeerJson>.Error("Error retrieving beer"));
    }
}