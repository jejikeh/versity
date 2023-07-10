using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, IEnumerable<Session>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public GetAllSessionsQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<IEnumerable<Session>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessionsRepository
            .GetAllSessions()
            .OrderBy(x => x.UserId)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        return await _sessionsRepository.ToListAsync(sessions);
    }
}