using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetAllSessionsLogs;

public class GetAllSessionsLogsQueryHandler : IRequestHandler<GetAllSessionsLogsQuery, IEnumerable<SessionLogs>>
{
    private readonly ISessionLogsRepository _sessionLogsRepository;

    public GetAllSessionsLogsQueryHandler(ISessionLogsRepository sessionLogsRepository)
    {
        _sessionLogsRepository = sessionLogsRepository;
    }

    public async Task<IEnumerable<SessionLogs>> Handle(GetAllSessionsLogsQuery request, CancellationToken cancellationToken)
    {
        var sessionLogs = _sessionLogsRepository
            .GetAllSessionsLogs()
            .OrderBy(x => x.Session.Start)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        return await _sessionLogsRepository.ToListAsync(sessionLogs);
    }
}