using BrewUp.Purchases.ReadModel.Services;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.MasterData;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Purchases.Facade.Acl;

public sealed class SupplierCreatedEventHandler(ISupplierService supplierService,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<SupplierCreated>(loggerFactory)
{
    public override async Task HandleAsync(SupplierCreated @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await supplierService.AddSupplierAsync(new SupplierId(@event.AggregateId.Value),
            @event.RagioneSociale,
            @event.PartitaIva,
            @event.Indirizzo,
            cancellationToken);
    }
}