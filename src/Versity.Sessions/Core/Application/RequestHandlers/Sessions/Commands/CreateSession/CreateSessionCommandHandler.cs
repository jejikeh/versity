﻿using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Domain.Models;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CreateSession;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, SessionViewModel>
{
    private readonly ISessionsRepository _sessions;
    private readonly IVersityUsersDataService _users;
    private readonly IProductsRepository _productsRepository;
    private readonly ISessionLogsRepository _sessionLogsRepository;

    public CreateSessionCommandHandler(ISessionsRepository sessionsRepository, IVersityUsersDataService users, IProductsRepository productsRepository, ISessionLogsRepository sessionLogsRepository)
    {
        _sessions = sessionsRepository;
        _users = users;
        _productsRepository = productsRepository;
        _sessionLogsRepository = sessionLogsRepository;
    }

    public async Task<SessionViewModel> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
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
        
        var sessionLogs = new SessionLogs()
        {
            Id = Guid.NewGuid()
        };
        
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Product = product,
            Start = request.Start,
            Expiry = request.Expiry,
            Status = SessionStatus.Inactive,
            Logs = sessionLogs
        };
        
        sessionLogs.SessionId = session.Id;
        
        var result = await _sessions.CreateSessionAsync(session, cancellationToken);
        await _sessionLogsRepository.CreateSessionLogsAsync(sessionLogs, cancellationToken);
        
        await _sessionLogsRepository.SaveChangesAsync(cancellationToken);
        await _sessions.SaveChangesAsync(cancellationToken);
        
        return SessionViewModel.MapWithModel(result);
    }
}