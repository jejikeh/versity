using Application.Abstractions.Hubs;
using Application.Abstractions.Notifications;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Domain.Models;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UpdateSessionStatusService
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<UpdateSessionStatusService> _logger;
    private readonly INotificationSender _notificationSender;
    
    public UpdateSessionStatusService(ISessionsRepository sessionsRepository, ILogger<UpdateSessionStatusService> logger, INotificationSender notificationSender)
    {
        _sessionsRepository = sessionsRepository;
        _logger = logger;
        _notificationSender = notificationSender;
    }

    public void ExpireExpiredSessions()
    {
        _logger.LogInformation("--> Start updating statuses of sessions...");

        var expiredSessions = _sessionsRepository
            .GetAllSessions()
            .Where(x => x.Expiry < DateTime.Now)
            .Where(x => x.Status != SessionStatus.Closed && x.Status != SessionStatus.Expired)
            .ToList();
    
        foreach (var session in expiredSessions)
        {
            session.Status = SessionStatus.Expired;
            _sessionsRepository.UpdateSession(session);
            _notificationSender.PushClosedSession(session.UserId, UserSessionsViewModel.MapWithModel(session));
            _logger.LogInformation($"--> Session {session.Id} has been expired!");
        }

        _sessionsRepository.SaveChanges();
    }
    
    public void OpenInactiveSessions()
    {
        _logger.LogInformation("--> Start activating sessions...");

        var expiredSessions = _sessionsRepository
            .GetAllSessions()
            .Where(x => x.Start <= DateTime.Now)
            .Where(x => x.Status == SessionStatus.Inactive)
            .ToList();
    
        foreach (var session in expiredSessions)
        {
            session.Status = SessionStatus.Open;
            _sessionsRepository.UpdateSession(session);
            _notificationSender.PushCreatedNewSession(session.UserId, UserSessionsViewModel.MapWithModel(session));
            _logger.LogInformation($"--> Session {session.Id} was open!");
        }

        _sessionsRepository.SaveChanges();
    }
}