using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CreateLogData;

public class CreateLogDataCommandHandler : IRequestHandler<CreateLogDataCommand, LogData>
{
    private readonly ILogsDataRepository _logsDataRepository;
    private readonly ISessionLogsRepository _sessionLogsRepository;

    public CreateLogDataCommandHandler(ILogsDataRepository logsDataRepository, ISessionLogsRepository sessionLogsRepository)
    {
        _logsDataRepository = logsDataRepository;
        _sessionLogsRepository = sessionLogsRepository;
    }

    public async Task<LogData> Handle(CreateLogDataCommand request, CancellationToken cancellationToken)
    {
        var sessionLogs = await _sessionLogsRepository.GetSessionLogsByIdAsync(request.SessionLogsId, cancellationToken);
        if (sessionLogs is null)
        {
            throw new NotFoundExceptionWithStatusCode($"There is no session logs with this Id({request.SessionLogsId})");
        }

        var logData = new LogData
        {
            Id = Guid.NewGuid(),
            Time = request.Time,
            LogLevel = request.LogLevel,
            Data = request.Data,
            SessionLogsId = request.SessionLogsId,
        };
        
        var result = await _logsDataRepository.CreateLogDataAsync(logData, cancellationToken);
        
        await _sessionLogsRepository.SaveChangesAsync(cancellationToken);
        await _logsDataRepository.SaveChangesAsync(cancellationToken);

        return result;
    }
}