using Application.Dtos;

namespace Application.Abstractions.Hubs;

public interface ISessionsHubClient
{
    public Task CreatedNewSession(UserSessionsViewModel sessionsViewModel);
    public Task ClosedSession(UserSessionsViewModel sessionsViewModel);
    public Task JoinToSession(string userId);
}