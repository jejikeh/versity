using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CloseSession;

public class CloseSessionCommandHandler : IRequestHandler<CloseSessionCommand, GetSessionByIdViewModel>
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly INotificationSender _notificationSender;

    public CloseSessionCommandHandler(ISessionsRepository sessionsRepository, INotificationSender notificationSender)
    {
        _sessionsRepository = sessionsRepository;
        _notificationSender = notificationSender;
    }

    public async Task<GetSessionByIdViewModel> Handle(CloseSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetSessionByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }

        session.Status = SessionStatus.Closed;
        _sessionsRepository.UpdateSession(session);
        await _sessionsRepository.SaveChangesAsync(cancellationToken);
        _notificationSender.PushClosedSession(session.UserId, UserSessionsViewModel.MapWithModel(session));
        
        return GetSessionByIdViewModel.MapWithModel(session);
    }
}