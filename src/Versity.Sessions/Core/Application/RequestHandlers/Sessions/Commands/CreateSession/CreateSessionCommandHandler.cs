using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Domain.Models;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CreateSession;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, SessionViewModel>
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IVersityUsersDataService _users;
    private readonly IProductsRepository _productsRepository;
    private readonly ISessionLogsRepository _sessionLogsRepository;
    private readonly INotificationSender _notificationSender;

    public CreateSessionCommandHandler(
        ISessionsRepository sessionsRepositoryRepository, 
        IVersityUsersDataService users, 
        IProductsRepository productsRepository, 
        ISessionLogsRepository sessionLogsRepository, 
        INotificationSender notificationSender)
    {
        _sessionsRepository = sessionsRepositoryRepository;
        _users = users;
        _productsRepository = productsRepository;
        _sessionLogsRepository = sessionLogsRepository;
        _notificationSender = notificationSender;
    }

    public async Task<SessionViewModel> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _users.IsUserExistAsync(request.UserId);
        if (!userExists)
        {
            throw new NotFoundExceptionWithStatusCode("User with specified Id doesnt exist!");
        }

        var product = await _productsRepository.GetProductByExternalIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("Product with specified Id doesnt exist!");
        }
        
        var sessionLogs = new SessionLogs()
        {
            Id = Guid.NewGuid()
        };
        
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = product.Id,
            Start = request.Start,
            Expiry = request.Expiry,
            Status = SessionStatus.Inactive,
            LogsId = sessionLogs.Id
        };
        
        var result = await _sessionsRepository.CreateSessionAsync(session, cancellationToken);
        
        sessionLogs.SessionId = result.Id;
        await _sessionLogsRepository.CreateSessionLogsAsync(sessionLogs, cancellationToken);
        
        await _sessionLogsRepository.SaveChangesAsync(cancellationToken);
        await _sessionsRepository.SaveChangesAsync(cancellationToken);

        var sessionViewModel = SessionViewModel.MapWithModel(result);
        _notificationSender.PushCreatedNewSession(result.UserId, UserSessionsViewModel.MapWithModel(session));
        
        return sessionViewModel;
    }
}