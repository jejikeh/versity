using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, IEnumerable<Session>>
{
    private readonly ISessionsRepository _sessions;

    public GetAllSessionsQueryHandler(ISessionsRepository sessions)
    {
        _sessions = sessions;
    }

    public Task<IEnumerable<Session>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessions
            .GetAllSessions()
            .OrderBy(x => x.UserId)
            .Skip(10 * (request.Page - 1))
            .Take(10)
            .ToList();
        
        return Task.Run(() => (IEnumerable<Session>)sessions, cancellationToken);
    }
}