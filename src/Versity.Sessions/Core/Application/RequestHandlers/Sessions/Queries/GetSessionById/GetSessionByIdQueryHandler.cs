using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetSessionById;

public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Session>
{
    private readonly ISessionsRepository _sessions;

    public GetSessionByIdQueryHandler(ISessionsRepository sessions)
    {
        _sessions = sessions;
    }

    public async Task<Session> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetSessionByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no session with this Id");
        }

        return session;
    }
}