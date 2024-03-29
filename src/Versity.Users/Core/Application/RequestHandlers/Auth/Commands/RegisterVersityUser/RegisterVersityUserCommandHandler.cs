﻿using System.Net.WebSockets;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.RegisterVersityUser;

public class RegisterVersityUserCommandHandler : IRequestHandler<RegisterVersityUserCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IEmailConfirmMessageService _emailConfirmMessageService;

    public RegisterVersityUserCommandHandler(IVersityUsersRepository versityUsersRepository, IEmailConfirmMessageService emailConfirmMessageService)
    {
        _versityUsersRepository = versityUsersRepository;
        _emailConfirmMessageService = emailConfirmMessageService;
    }

    public async Task<IdentityResult> Handle(RegisterVersityUserCommand request, CancellationToken cancellationToken)
    {
        var versityUser = new VersityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            PhoneNumber = request.Phone,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = $"{request.FirstName} {request.LastName}",
        };
        
        var result = await _versityUsersRepository.CreateUserAsync(versityUser, request.Password);
        Utils.AggregateIdentityErrorsAndThrow(result);
        Utils.AggregateIdentityErrorsAndThrow(await _versityUsersRepository.SetUserRoleAsync(versityUser, VersityRole.Member));
        await _emailConfirmMessageService.SendEmailConfirmMessageAsync(versityUser);
        
        return result;
    }
}