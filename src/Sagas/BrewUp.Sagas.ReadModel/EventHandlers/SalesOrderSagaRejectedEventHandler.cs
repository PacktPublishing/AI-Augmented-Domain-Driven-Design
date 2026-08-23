using BrewUp.Sagas.Infrastructure.Hubs;
using BrewUp.Sagas.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SalesOrderSagaRejectedEventHandler(ISagasHubHelper sagasHubHelper,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<SalesOrderSagaRejected>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderSagaRejected @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // Use signalR Hub to send response to the Client
        await sagasHubHelper.TellChildrenThatSalesOrderSagaWasRejected(@event.Message, cancellationToken);
    }
}