using Microsoft.AspNetCore.SignalR;

namespace BrewUp.Sagas.Infrastructure.Hubs;

internal sealed class SagasHubHelper(IHubContext<SagaHub> hubContext) : ISagasHubHelper
{
    public async Task TellChildrenThatSalesOrderSagaWasRejected(string salesOrderId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await hubContext.Clients.All.SendAsync("SalesOrderRejected", salesOrderId, CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task TellChildrenThatSalesOrderSagaWasCompleted(string salesOrderId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await hubContext.Clients.All.SendAsync("SalesOrderCompleted", salesOrderId, CancellationToken.None)
            .ConfigureAwait(false);
    }
}