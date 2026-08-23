using BrewUp.Sales.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.ReadModel.EventHandlers;

public sealed class SalesOrderConfirmedEventHandler(ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<SalesOrderConfirmed>(loggerFactory)
{
    public override Task HandleAsync(SalesOrderConfirmed @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        // MongoDB read model upsert would be placed here when a read-side DTO is defined
        return Task.CompletedTask;
    }
}
