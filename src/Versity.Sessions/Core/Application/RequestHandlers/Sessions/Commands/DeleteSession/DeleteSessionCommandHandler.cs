using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.DeleteSession;

public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand>
{
    private readonly ISessionsRepository _sessions;

    public DeleteSessionCommandHandler(ISessionsRepository sessions)
    {
        _sessions = sessions;
    }

    public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        var sessions = await _sessions.GetSessionByIdAsync(request.Id, cancellationToken);
        
        if (sessions is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        
        _sessions.DeleteSession(sessions);
        await _sessions.SaveChangesAsync(cancellationToken);
    }
}