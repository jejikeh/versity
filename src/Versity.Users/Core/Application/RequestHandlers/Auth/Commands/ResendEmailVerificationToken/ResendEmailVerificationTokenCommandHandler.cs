using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;

public class ResendEmailVerificationTokenCommandHandler : IRequestHandler<ResendEmailVerificationTokenCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public ResendEmailVerificationTokenCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IdentityResult> Handle(ResendEmailVerificationTokenCommand request, CancellationToken cancellationToken)
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