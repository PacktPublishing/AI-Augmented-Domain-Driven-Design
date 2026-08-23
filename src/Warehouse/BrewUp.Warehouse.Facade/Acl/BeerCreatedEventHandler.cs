using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.MasterData;
using BrewUp.Warehouse.ReadModel.Services;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.Facade.Acl;

public sealed class BeerCreatedEventHandler(IBeerService beerService,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<BeerCreated>(loggerFactory)
{
    public override async Task HandleAsync(BeerCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        await beerService.AddBeerAsync((BeerId) @event.AggregateId, @event.BeerName, @event.BeerStyle,
            @event.AlcoholByVolume, @event.Packaging, @event.Price, @event.IsActive, cancellationToken);
    }
}