using BrewUp.Sales.ReadModel.Services;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events;
using BrewUp.Shared.Messages.Events.MasterData;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.Facade.Acl;

public sealed class CustomerCreatedEventHandler(ICustomerService customerService,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<CustomerCreated>(loggerFactory)
{
    public override async Task HandleAsync(CustomerCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await customerService.AddCustomerAsync((CustomerId)@event.AggregateId,
            @event.RagioneSociale,
            @event.PartitaIva,
            @event.BeerConsumerLevel,
            @event.Indirizzo,
            cancellationToken);
    }
}