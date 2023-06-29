using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;

public class GetAllProductSessionsQueryHandler : IRequestHandler<GetAllProductSessionsQuery, IEnumerable<Session>>
{
    private readonly ISessionsRepository _sessions;

    public GetAllProductSessionsQueryHandler(ISessionsRepository sessions)
    {
        _sessions = sessions;
    }

    public async Task<IEnumerable<Session>> Handle(GetAllProductSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _sessions.GetAllProductSessionsAsync(request.ProductId, cancellationToken);

        return sessions;
    }
}