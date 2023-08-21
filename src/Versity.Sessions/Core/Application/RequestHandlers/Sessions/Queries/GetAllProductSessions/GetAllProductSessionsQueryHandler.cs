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

    public Task<IEnumerable<SessionViewModel>> Handle(GetAllProductSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessionsRepository.GetAllProductSessions(
            request.ProductId,
            PageFetchSettings.ItemsOnPage * (request.Page - 1),
            PageFetchSettings.ItemsOnPage);
        
        return Task.FromResult(SessionViewModel.MapWithModels(sessions));
    }
}