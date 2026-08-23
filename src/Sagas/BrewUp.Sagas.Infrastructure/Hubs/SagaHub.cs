using Microsoft.AspNetCore.SignalR;

namespace BrewUp.Sagas.Infrastructure.Hubs;

public class SagaHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("SagasHubConnected", "BrewUp Sagas is Connected").ConfigureAwait(false);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("SagasHubDisconnected", "BrewUp Sagas Disconnected").ConfigureAwait(false);

        await base.OnDisconnectedAsync(exception);
    }
}