using System.Security.Claims;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangeUserPasswordCommandHandler(IVersityUsersRepository versityUsersRepository, IHttpContextAccessor httpContextAccessor)
    {
        _versityUsersRepository = versityUsersRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IdentityResult> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = request.Id;
        if (string.IsNullOrEmpty(userId))
        {
            userId = _httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(userId)) 
            { 
                throw new InvalidOperationException("User claims was empty!");
            }
        }

        var versityUser = await _versityUsersRepository.GetUserByIdAsync(userId);
        if (versityUser is null) 
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        if (!await _versityUsersRepository.CheckPasswordAsync(versityUser, request.OldPassword))
        {
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();
        }
        var token = await _versityUsersRepository.GeneratePasswordResetTokenAsync(versityUser);
        return await _versityUsersRepository.ResetPasswordAsync(versityUser, token, request.NewPassword);
    }
}