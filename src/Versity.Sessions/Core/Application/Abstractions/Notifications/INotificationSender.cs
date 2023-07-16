using Application.Dtos;
using MediatR;

namespace Application.Abstractions.Notifications;

public interface INotificationSender
{
    public void PushClosedSession(string userId, UserSessionsViewModel viewModel);
    public void PushCreatedNewSession(string userId, UserSessionsViewModel viewModel);
}