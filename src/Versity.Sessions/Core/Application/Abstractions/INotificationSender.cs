using Application.Dtos;

namespace Application.Abstractions;

public interface INotificationSender
{
    public void PushClosedSession(string userId, UserSessionsViewModel viewModel);
    public void PushCreatedNewSession(string userId, UserSessionsViewModel viewModel);
}