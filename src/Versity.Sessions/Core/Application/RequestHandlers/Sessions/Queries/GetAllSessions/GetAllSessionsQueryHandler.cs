using Application.Abstractions.Repositories;
using Application.Common;
using Application.Dtos;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, IEnumerable<SessionViewModel>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public GetAllSessionsQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<IEnumerable<SessionViewModel>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessionsRepository
            .GetAllSessions()
            .OrderBy(x => x.Status)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);
        
        var viewModels = SessionViewModel.MapWithModels(await _sessionsRepository.ToListAsync(sessions));
        
        return viewModels;
    }
}