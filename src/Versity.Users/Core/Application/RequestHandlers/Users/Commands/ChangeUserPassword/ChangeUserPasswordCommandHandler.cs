using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public ChangeUserPasswordCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IdentityResult> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.Id);
        if (versityUser == null)
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        
        if (!await _versityUsersRepository.CheckPasswordAsync(versityUser, request.OldPassword))
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();
        
        // TODO: It would be great to send that token by email or another provider.
        var token = await _versityUsersRepository.GeneratePasswordResetTokenAsync(versityUser);
        return await _versityUsersRepository.ResetPasswordAsync(versityUser, token, request.NewPassword);
    }
}