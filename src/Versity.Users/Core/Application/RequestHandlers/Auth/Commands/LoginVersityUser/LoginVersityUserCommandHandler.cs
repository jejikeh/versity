﻿using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public class LoginVersityUserCommandHandler : IRequestHandler<LoginVersityUserCommand, string>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;
    
    public LoginVersityUserCommandHandler(IAuthTokenGeneratorService tokenGeneratorService, IVersityUsersRepository versityUsersRepository)
    {
        _tokenGeneratorService = tokenGeneratorService;
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<string> Handle(LoginVersityUserCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByEmailAsync(request.Email);
        if (versityUser is null || !await _versityUsersRepository.CheckPasswordAsync(versityUser, request.Password)) 
        { 
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();
        }
        if (!versityUser.EmailConfirmed)
        {
            throw new EmailNotConfirmedExceptionWithStatusCode();
        }
        var userRoles = await _versityUsersRepository.GetRolesAsync(versityUser);
        
        return _tokenGeneratorService.GenerateToken(versityUser.Id, versityUser.Email, userRoles);
    }
}