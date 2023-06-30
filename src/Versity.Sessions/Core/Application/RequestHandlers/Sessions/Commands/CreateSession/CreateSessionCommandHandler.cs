using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CreateSession;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Session>
{
    private readonly ISessionsRepository _sessions;
    private readonly IVersityUsersDataService _users;

    public CreateSessionCommandHandler(ISessionsRepository sessionsRepository, IVersityUsersDataService users)
    {
        _sessions = sessionsRepository;
        _users = users;
    }

    public async Task<Session> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        if (!await _users.IsUserExistAsync(request.UserId))
        {
            throw new NotFoundExceptionWithStatusCode("User with specified Id doesnt exist!");
        }
        
        var sessionId = Guid.NewGuid();
        while (await _sessions.GetSessionByIdAsync(sessionId, cancellationToken) is not null)
        {
            sessionId = Guid.NewGuid();
        }
        var session = new Session
        {
            Id = sessionId,
            UserId = request.UserId,
            ProductId = request.ProductId,
            Start = request.Start,
            Expiry = request.Expiry,
            Status = SessionStatus.Inactive
        };
        var result = await _sessions.CreateSessionAsync(session, cancellationToken);
        await _sessions.SaveChangesAsync(cancellationToken);

        return result;
    }
}