using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetSessionLogsById;

public class GetSessionLogsByIdQueryHandler : IRequestHandler<GetSessionLogsByIdQuery, SessionLogs>
{
    private readonly ISessionLogsRepository _sessionLogsRepository;

    public GetSessionLogsByIdQueryHandler(ISessionLogsRepository sessionLogsRepository)
    {
        _sessionLogsRepository = sessionLogsRepository;
    }

    public async Task<SessionLogs> Handle(GetSessionLogsByIdQuery request, CancellationToken cancellationToken)
    {
        var sessionLogs = await _sessionLogsRepository.GetSessionLogsByIdAsync(request.Id, cancellationToken);
        if (sessionLogs is null)
        {
            throw new NotFoundExceptionWithStatusCode($"There is no session logs with this Id({request.Id})");
        }

        return sessionLogs;
    }
}