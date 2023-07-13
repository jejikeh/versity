using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CloseSession;

public class CloseSessionCommandHandler : IRequestHandler<CloseSessionCommand>
{
    private readonly ISessionsRepository _sessionsRepository;

    public CloseSessionCommandHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task Handle(CloseSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetSessionByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }

        session.Status = SessionStatus.Closed;
        _sessionsRepository.UpdateSession(session);
        await _sessionsRepository.SaveChangesAsync(cancellationToken);
    }
}