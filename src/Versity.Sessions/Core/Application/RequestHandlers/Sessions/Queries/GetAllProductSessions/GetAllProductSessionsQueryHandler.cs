using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;

public class GetAllProductSessionsQueryHandler : IRequestHandler<GetAllProductSessionsQuery, IEnumerable<Session>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public GetAllProductSessionsQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<IEnumerable<Session>> Handle(GetAllProductSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessionsRepository
            .GetAllProductSessions(request.ProductId)
            .OrderBy(x => x.UserId)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        return await _sessionsRepository.ToListAsync(sessions);
    }
}