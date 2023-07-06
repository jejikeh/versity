using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, IEnumerable<Session>>
{
    private readonly ISessionsRepository _sessions;

    public GetAllSessionsQueryHandler(ISessionsRepository sessions)
    {
        _sessions = sessions;
    }

    public async Task<IEnumerable<Session>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _sessions
            .GetAllSessions()
            .OrderBy(x => x.UserId)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage)
            .ToListAsync(cancellationToken);
        
        return sessions;
    }
}