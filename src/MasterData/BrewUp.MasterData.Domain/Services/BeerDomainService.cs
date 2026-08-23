using BrewUp.MasterData.Domain.Helpers;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ReadModel;
using Lena.Asyncs;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.MasterData.Domain.Services;

internal sealed class BeerDomainService([FromKeyedServices("masterdata")] IPersister persister,
    IIntegrationEventPublisher integrationEventPublisher) : IBeerDomainService
{
    public async Task<Result<string>> RegisterBeerAsync(BeerId beerId, BeerName beerName, BeerStyle style, AlcoholByVolume abv, Packaging packaging,
        Price price, bool isActive, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Beer beer = Beer.Create(beerId, beerName, style, abv, packaging, price, isActive);
        
        // Railway-Oriented Programming pattern
        return (await (await persister.InsertAsync(beer, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(beer.ToBeerCreated(), cancellationToken)))
            .Match(
                _ => Result<string>.Success(beerId.Value),
                Result<string>.Error);
    }

    public async Task<Result<bool>> SaveBeerAsync(BeerId beerId, BeerName beerName, BeerStyle style, AlcoholByVolume abv, Packaging packaging,
        Price price, bool isActive, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var beerResult = await persister.GetByIdAsync<Beer>(beerId.Value, cancellationToken);
        if (!beerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to save beer: {beerId.Value}");
        
        beerResult.TryGetValue(out Beer beer);
        beer.UpdateBeerName(beerName);
        beer.UpdateBeerStyle(style);
        beer.UpdateAlcoholByVolume(abv);
        beer.UpdatePackaging(packaging);
        beer.UpdatePrice(price);
        beer.UpdateIsActive(isActive);
        
        return (await (await persister.UpdateAsync(beer, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(beer.ToBeerUpdated(), cancellationToken)))
            .Match(
                _ => Result<bool>.Success(true),
                Result<bool>.Error);
    }

    public async Task<Result<bool>> DeleteBeerAsync(BeerId beerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var beerResult = await persister.GetByIdAsync<Beer>(beerId.Value, cancellationToken);
        if (!beerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to delete beer: {beerId.Value}");
        
        beerResult.TryGetValue(out Beer beer);
        return (await (await persister.DeleteAsync(beer, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(beer.ToBeerDeleted(), cancellationToken)))
            .Match(
                _ => Result<bool>.Success(true),
                Result<bool>.Error);
    }
}