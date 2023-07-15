using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SignalHub : Hub
{
    private readonly ILogger<SignalHub> _logger;
    private readonly string _botUser;

    public SignalHub(ILogger<SignalHub> logger)
    {
        _logger = logger;
        _botUser = "MyChatBot";
    }

    public async Task JoinToSession(UserConnection userConnection)
    {
        await Clients.All.SendAsync("ReceiveMessage", _botUser,$"{userConnection.User} has joined {userConnection.Session}");
    }
}