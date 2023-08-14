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
        var sessions = _sessionsRepository.GetSessions(
            PageFetchSettings.ItemsOnPage * (request.Page - 1),
            PageFetchSettings.ItemsOnPage);
        
        var viewModels = SessionViewModel.MapWithModels(sessions);
        
        return viewModels;
    }
}