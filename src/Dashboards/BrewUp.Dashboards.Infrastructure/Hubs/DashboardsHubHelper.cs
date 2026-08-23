using Microsoft.AspNetCore.SignalR;

namespace BrewUp.Dashboards.Infrastructure.Hubs;

internal sealed class DashboardsHubHelper(IHubContext<DashboardsHub> hubContext) : IDashboardsHubHelper
{
    public async Task TellChildrenThatCustomersDashboardWasUpdated(string customerName, CancellationToken cancellationToken)
    {
        // Use CancellationToken.None: the caller's token (e.g. from a RabbitMQ consumer) can
        // be cancelled after the message is acknowledged, before the WebSocket frame is flushed,
        // silently dropping the notification. The actual send must not be tied to that token.
        await hubContext.Clients.All.SendAsync("CustomersDashboardUpdated", customerName, CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task TellChildrenThatProductsDashboardWasUpdated(string productId, CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.SendAsync("ProductsDashboardUpdated", productId, CancellationToken.None)
            .ConfigureAwait(false);
    }
}