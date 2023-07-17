using Application.Abstractions;
using Application.Abstractions.Hubs;
using Application.Abstractions.Notifications;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using Domain.Models;
using Domain.Models.SessionLogging;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UpdateSessionStatusService
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<UpdateSessionStatusService> _logger;
    private readonly INotificationSender _notificationSender;
    private readonly ICacheService _cacheService;
    private readonly ISessionLogsRepository _sessionLogsRepository;
    private readonly ILogsDataRepository _logsDataRepository;

    public UpdateSessionStatusService(ISessionsRepository sessionsRepository, ILogger<UpdateSessionStatusService> logger, INotificationSender notificationSender, ICacheService cacheService, ISessionLogsRepository sessionLogsRepository, ILogsDataRepository logsDataRepository)
    {
        _sessionsRepository = sessionsRepository;
        _logger = logger;
        _notificationSender = notificationSender;
        _cacheService = cacheService;
        _sessionLogsRepository = sessionLogsRepository;
        _logsDataRepository = logsDataRepository;
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
    
    public void PushSessionLogs()
    {
        _logger.LogInformation("--> Start pushing session logs...");

        var logs = _cacheService
            .GetSetAsync<CreateLogDataCommand>("session-logs")
            .ToBlockingEnumerable();
    
        foreach (var log in logs)
        {
            var sessionLogs = _sessionLogsRepository.GetSessionLogsByIdAsync(log.SessionLogsId, default).Result;
            if (sessionLogs is null)
            {
                throw new NotFoundExceptionWithStatusCode($"There is no session logs with this Id({log.SessionLogsId})");
            }
            
            var logData = new LogData
            {
                Id = Guid.NewGuid(),
                Time = log.Time,
                LogLevel = log.LogLevel,
                Data = log.Data,
                SessionLogsId = log.SessionLogsId,
                SessionLogs = sessionLogs
            };
            
            sessionLogs.Logs.Add(logData);
            _logsDataRepository.CreateLogDataAsync(logData, default);

        }

        _logsDataRepository.SaveChanges();
        _sessionsRepository.SaveChanges();
    }
}