using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;

public class ResendEmailVerificationTokenCommandHandler : IRequestHandler<ResendEmailVerificationTokenCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IEmailConfirmMessageService _emailConfirmMessageService;

    public ResendEmailVerificationTokenCommandHandler(IVersityUsersRepository versityUsersRepository, IEmailConfirmMessageService emailConfirmMessageService)
    {
        _versityUsersRepository = versityUsersRepository;
        _emailConfirmMessageService = emailConfirmMessageService;
    }

    public async Task<IdentityResult> Handle(ResendEmailVerificationTokenCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByEmailAsync(request.Email);
        if (versityUser is null || !await _versityUsersRepository.CheckPasswordAsync(versityUser, request.Password)) 
        { 
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();
        }
        
        if (versityUser.EmailConfirmed)
        {
            throw new IdentityExceptionWithStatusCode("The Email already confirmed");
        }
        
        await _emailConfirmMessageService.SendEmailConfirmMessageAsync(versityUser);
        
        return IdentityResult.Success;
    }
}