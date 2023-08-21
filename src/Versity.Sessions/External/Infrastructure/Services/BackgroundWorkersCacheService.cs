using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using Domain.Models.SessionLogging;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class BackgroundWorkersCacheService
{
    private readonly ILogger<BackgroundWorkersCacheService> _logger;
    private readonly ICacheService _cacheService;
    private readonly ISessionLogsRepository _sessionLogsRepository;
    private readonly ILogsDataRepository _logsDataRepository;

    public BackgroundWorkersCacheService(ILogger<BackgroundWorkersCacheService> logger, ICacheService cacheService, ISessionLogsRepository sessionLogsRepository, ILogsDataRepository logsDataRepository)
    {
        _logger = logger;
        _cacheService = cacheService;
        _sessionLogsRepository = sessionLogsRepository;
        _logsDataRepository = logsDataRepository;
    }

    public void PushSessionLogs()
    {
        _logger.LogInformation("--> Start pushing session logs...");

        var logs = _cacheService
            .GetSetAsync<CacheLogDataCommand>(CachingKeys.SessionLogs)
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
            };
            
            _logsDataRepository.CreateLogDataAsync(logData, default);
            _cacheService.SetRemoveMember(CachingKeys.SessionLogs, log);
        }

        _logsDataRepository.SaveChanges();
        _sessionLogsRepository.SaveChanges();
    }
}