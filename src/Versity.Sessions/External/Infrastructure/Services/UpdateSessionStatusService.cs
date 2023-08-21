using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UpdateSessionStatusService
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<UpdateSessionStatusService> _logger;
    private readonly INotificationSender _notificationSender;

    public UpdateSessionStatusService(
        ISessionsRepository sessionsRepository, 
        ILogger<UpdateSessionStatusService> logger, 
        INotificationSender notificationSender)
    {
        _sessionsRepository = sessionsRepository;
        _logger = logger;
        _notificationSender = notificationSender;
    }

    public void ExpireExpiredSessions()
    {
        _logger.LogInformation("--> Start updating statuses of sessions...");

        var expiredSessions = _sessionsRepository.GetExpiredSessions();
    
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

        var inactiveSessions = _sessionsRepository.GetInactiveSessions();
    
        foreach (var session in inactiveSessions)
        {
            session.Status = SessionStatus.Open;
            _sessionsRepository.UpdateSession(session);
            _notificationSender.PushCreatedNewSession(session.UserId, UserSessionsViewModel.MapWithModel(session));
            _logger.LogInformation($"--> Session {session.Id} was open!");
        }

        _sessionsRepository.SaveChanges();
    }
}