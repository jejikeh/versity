using System.Security.Claims;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Dtos;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;

public class GetUserSessionsByUserIdQueryHandler 
    : IRequestHandler<GetUserSessionsByUserIdQuery, IEnumerable<UserSessionsViewModel>>
{
    private readonly ISessionsRepository _sessions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IVersityUsersDataService _usersDataService;

    public GetUserSessionsByUserIdQueryHandler(
        ISessionsRepository sessions, 
        IHttpContextAccessor httpContextAccessor, 
        IVersityUsersDataService usersDataService)
    {
        _sessions = sessions;
        _httpContextAccessor = httpContextAccessor;
        _usersDataService = usersDataService;
    }

    public async Task<IEnumerable<UserSessionsViewModel>> Handle(GetUserSessionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var claimId = _httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (string.IsNullOrEmpty(claimId)) 
        {
            throw new InvalidOperationException("User claims was empty!");
        }

        if (claimId != request.UserId)
        {
            var roles = await _usersDataService.GetUserRolesAsync(claimId);
            if (!roles.Contains("Admin"))
            {
                throw new ExceptionWithStatusCode(StatusCodes.Status403Forbidden, "Not enough rights");
            }
        }

        var sessions = _sessions
            .GetAllUserSessions(request.UserId)
            .OrderBy(x => x.Status)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        var viewModels = UserSessionsViewModel.MapWithModels(await _sessions.ToListAsync(sessions));

        return viewModels;
    }
}