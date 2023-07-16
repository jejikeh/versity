using Application.Abstractions.Hubs;
using Application.Abstractions.Notifications;
using Application.Dtos;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class SessionNotificationsSenderService : INotificationSender
{
    private readonly ILogger<SessionNotificationsSenderService> _logger;

    public SessionNotificationsSenderService(ILogger<SessionNotificationsSenderService> logger)
    {
        _logger = logger;
    }

    public void PushClosedSession(string userId, UserSessionsViewModel viewModel)
    {
        _logger.LogInformation($"--> Push notification ClosedSession to the {userId} user");
        BackgroundJob.Enqueue<ISessionsHubHelper>(x => x.PushClosedSession(userId, viewModel));
    }

    public void PushCreatedNewSession(string userId, UserSessionsViewModel viewModel)
    {
        _logger.LogInformation($"--> Push notification CreatedNewSession to the {userId} user");
        BackgroundJob.Enqueue<ISessionsHubHelper>(x => x.PushCreatedNewSession(userId, viewModel));
    }
}