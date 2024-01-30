using Application.Dtos;

namespace Application.Abstractions.Hubs;

public interface ISessionsHubHelper
{
    public void PushCreatedNewSession(string userId, UserSessionsViewModel viewModel);
    public void PushClosedSession(string userId, UserSessionsViewModel viewModel);
}