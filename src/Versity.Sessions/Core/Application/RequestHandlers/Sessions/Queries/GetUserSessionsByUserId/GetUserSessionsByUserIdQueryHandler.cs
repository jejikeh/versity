using System.Security.Claims;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;

public class GetUserSessionsByUserIdQueryHandler : IRequestHandler<GetUserSessionsByUserIdQuery, IEnumerable<Session>>
{
    private readonly ISessionsRepository _sessions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IVersityUsersDataService _usersDataService;

    public GetUserSessionsByUserIdQueryHandler(ISessionsRepository sessions, IHttpContextAccessor httpContextAccessor, IVersityUsersDataService usersDataService)
    {
        _sessions = sessions;
        _httpContextAccessor = httpContextAccessor;
        _usersDataService = usersDataService;
    }

    public async Task<IEnumerable<Session>> Handle(GetUserSessionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var claimId = _httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (string.IsNullOrEmpty(claimId)) 
        {
            throw new InvalidOperationException("User claims was empty!");
        }

        if (claimId != request.UserId)
        {
            _usersDataService.GetUserRoles(claimId).
        }
        var session = await _sessions.GetAllUserSessionsAsync(request.UserId, cancellationToken);
        
        return session;
    }
}