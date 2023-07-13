using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UpdateSessionStatusService
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<UpdateSessionStatusService> _logger;

    public UpdateSessionStatusService(ISessionsRepository sessionsRepository, ILogger<UpdateSessionStatusService> logger)
    {
        _sessionsRepository = sessionsRepository;
        _logger = logger;
    }

    public void ExpireExpiredSessions()
    {
        _logger.LogInformation("--> Start updating statuses of sessions...");

        var expiredSessions = _sessionsRepository
            .GetAllSessions()
            .Where(x => x.Expiry < DateTime.Today)
            .Where(x => x.Status != SessionStatus.Closed || x.Status != SessionStatus.Expired)
            .ToList();
    
        foreach (var session in expiredSessions)
        {
            session.Status = SessionStatus.Expired;
            _sessionsRepository.UpdateSession(session);
            _logger.LogInformation($"--> Session {session.Id} has been expired!");
        }

        _sessionsRepository.SaveChanges();
    }
    
    public void OpenInactiveSessions()
    {
        _logger.LogInformation("--> Start activating sessions...");

        var expiredSessions = _sessionsRepository
            .GetAllSessions()
            .Where(x => x.Start <= DateTime.Today)
            .Where(x => x.Status == SessionStatus.Inactive)
            .ToList();
    
        foreach (var session in expiredSessions)
        {
            session.Status = SessionStatus.Open;
            _sessionsRepository.UpdateSession(session);
            _logger.LogInformation($"--> Session {session.Id} was open!");
        }

        _sessionsRepository.SaveChanges();
    }
}