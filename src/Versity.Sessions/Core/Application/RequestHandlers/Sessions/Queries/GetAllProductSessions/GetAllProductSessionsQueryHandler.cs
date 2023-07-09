using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var sessions = await _sessions
            .GetAllProductSessions(request.ProductId, cancellationToken)
            .OrderBy(x => x.UserId)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage)
            .ToListAsync(cancellationToken);
        
        return sessions;
    }
}