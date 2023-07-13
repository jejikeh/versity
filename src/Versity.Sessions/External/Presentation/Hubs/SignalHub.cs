using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SignalHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync(
            "ReceiveMessage",
            $"{Context.ConnectionId} has joined");
    }
}