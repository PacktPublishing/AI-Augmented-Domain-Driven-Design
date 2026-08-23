using Microsoft.AspNetCore.SignalR;

namespace BrewUp.Dashboards.Infrastructure.Hubs;

public class DashboardsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("DashboardsHubConnected", "BrewUp Dashboards is Connected").ConfigureAwait(false);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("DashboardsHubDisconnected", "BrewUp Dashboards Disconnected").ConfigureAwait(false);

        await base.OnDisconnectedAsync(exception);
    }
}