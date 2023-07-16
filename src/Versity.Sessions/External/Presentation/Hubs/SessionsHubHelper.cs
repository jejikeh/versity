using Application.Abstractions.Hubs;
using Application.Dtos;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SessionsHubHelper : ISessionsHubHelper
{
    private readonly IHubContext<SessionsHub, ISessionsHubClient> _hub;

    public SessionsHubHelper(IHubContext<SessionsHub, ISessionsHubClient> hub)
    {
        _hub = hub;
    }

    public void PushCreatedNewSession(string userId, UserSessionsViewModel viewModel)
    {
        _hub.Clients.User(userId).CreatedNewSession(viewModel);
    }

    public void PushClosedSession(string userId, UserSessionsViewModel viewModel)
    {
        _hub.Clients.User(userId).ClosedSession(viewModel);
    }
}