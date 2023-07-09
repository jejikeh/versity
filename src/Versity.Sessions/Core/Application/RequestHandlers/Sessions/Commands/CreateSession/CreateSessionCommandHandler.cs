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
    private readonly IProductsRepository _productsRepository;

    public CreateSessionCommandHandler(ISessionsRepository sessionsRepository, IVersityUsersDataService users, IProductsRepository productsRepository)
    {
        _sessions = sessionsRepository;
        _users = users;
        _productsRepository = productsRepository;
    }

    public async Task<Session> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        if (!await _users.IsUserExistAsync(request.UserId))
        {
            throw new NotFoundExceptionWithStatusCode("User with specified Id doesnt exist!");
        }

        var product = await _productsRepository.GetProductByExternalIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("Product with specified Id doesnt exist!");
        }

        var sessionId = Guid.NewGuid();
        while (await _sessions.GetSessionByIdAsync(sessionId, cancellationToken) is not null)
        {
            sessionId = Guid.NewGuid();
        }
        
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = product.Id,
            Start = request.Start,
            Expiry = request.Expiry,
            Status = SessionStatus.Inactive
        };
        
        var result = await _sessions.CreateSessionAsync(session, cancellationToken);
        await _sessions.SaveChangesAsync(cancellationToken);

        return result;
    }
}