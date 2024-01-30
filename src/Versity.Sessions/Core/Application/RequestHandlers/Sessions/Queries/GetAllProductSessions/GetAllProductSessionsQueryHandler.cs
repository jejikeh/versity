using Application.Abstractions.Repositories;
using Application.Common;
using Application.Dtos;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;

public class GetAllProductSessionsQueryHandler : IRequestHandler<GetAllProductSessionsQuery, IEnumerable<SessionViewModel>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public GetAllProductSessionsQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<IEnumerable<SessionViewModel>> Handle(GetAllProductSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessionsRepository
            .GetAllProductSessions(request.ProductId)
            .OrderBy(x => x.Status)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);
        
        var viewModels = SessionViewModel.MapWithModels(await _sessionsRepository.ToListAsync(sessions));

        return viewModels;
    }
}