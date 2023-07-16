using Application.Abstractions.Hubs;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SessionsHub : Hub<ISessionsHubClient>
{
    private readonly ILogger<SessionsHub> _logger;

    public SessionsHub(ILogger<SessionsHub> logger)
    {
        _logger = logger;
    }
    
    [Authorize(Roles = "Member")]
    public async Task JoinToSession(UserConnection userConnection)
    {
        await Clients.All.JoinToSession($"{userConnection.User} has joined with id {Context.UserIdentifier}");
    }
}