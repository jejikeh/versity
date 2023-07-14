using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetLogDataById;

public class GetLogDataByIdQueryHandler : IRequestHandler<GetLogDataByIdQuery, LogData>
{
    private readonly ILogsDataRepository _logsDataRepository;

    public GetLogDataByIdQueryHandler(ILogsDataRepository logsDataRepository)
    {
        _logsDataRepository = logsDataRepository;
    }

    public async Task<LogData> Handle(GetLogDataByIdQuery request, CancellationToken cancellationToken)
    {
        var logData = await _logsDataRepository.GetLogDataByIdAsync(request.Id, cancellationToken);
        if (logData is null)
        {
            throw new NotFoundExceptionWithStatusCode($"There is no log with this Id({request.Id})");
        }

        return logData;
    }
}