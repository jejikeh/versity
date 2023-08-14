using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;

public class CreateLogsDataCommandHandler : IRequestHandler<CreateLogsDataCommand, IEnumerable<LogData>>
{
    private readonly ILogsDataRepository _logsDataRepository;
    private readonly ISessionLogsRepository _sessionLogsRepository;

    public CreateLogsDataCommandHandler(ILogsDataRepository logsDataRepository, ISessionLogsRepository sessionLogsRepository)
    {
        _logsDataRepository = logsDataRepository;
        _sessionLogsRepository = sessionLogsRepository;
    }

    public async Task<IEnumerable<LogData>> Handle(CreateLogsDataCommand request, CancellationToken cancellationToken)
    {
        var results = new List<LogData>();
        foreach (var command in request.CreateLogDataCommands)
        {
            var sessionLogs = await _sessionLogsRepository.GetSessionLogsByIdAsync(request.SessionLogsId, cancellationToken);
            if (sessionLogs is null)
            {
                throw new NotFoundExceptionWithStatusCode($"There is no session logs with this Id({request.SessionLogsId})");
            }
            
            var logData = new LogData
            {
                Id = Guid.NewGuid(),
                Time = command.Time,
                LogLevel = command.LogLevel,
                Data = command.Data,
                SessionLogsId = request.SessionLogsId,
            };
        
            results.Add(logData);
        }

        await _logsDataRepository.CreateRangeLogsDataAsync(results, cancellationToken);
        await _sessionLogsRepository.SaveChangesAsync(cancellationToken);
        await _logsDataRepository.SaveChangesAsync(cancellationToken);

        return results;
    }
}