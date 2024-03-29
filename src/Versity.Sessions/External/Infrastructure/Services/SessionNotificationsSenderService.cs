﻿using Application.Abstractions;
using Application.Abstractions.Hubs;
using Application.Dtos;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class SessionNotificationsSenderService : INotificationSender
{
    private readonly ILogger<SessionNotificationsSenderService> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public SessionNotificationsSenderService(ILogger<SessionNotificationsSenderService> logger, IBackgroundJobClient backgroundJobClient)
    {
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public void PushClosedSession(string userId, UserSessionsViewModel viewModel)
    {
        _logger.LogInformation($"--> Push notification ClosedSession to the {userId} user");
        _backgroundJobClient.Enqueue<ISessionsHubHelper>(x => x.PushClosedSession(userId, viewModel));
    }

    public void PushCreatedNewSession(string userId, UserSessionsViewModel viewModel)
    {
        _logger.LogInformation($"--> Push notification CreatedNewSession to the {userId} user");
        _backgroundJobClient.Enqueue<ISessionsHubHelper>(x => x.PushCreatedNewSession(userId, viewModel));
    }
}