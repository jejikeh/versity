using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

[Authorize]
public class SignalHub : Hub<ISessionClient>
{
    private readonly ILogger<SignalHub> _logger;

    public SignalHub(ILogger<SignalHub> logger)
    {
        _logger = logger;
    }

    [Authorize("Admin")]
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ConnectToSession($"{Context.ConnectionId} connect to session");
    }
}