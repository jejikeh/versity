using MediatR;
using Microsoft.AspNetCore.Identity;
using Versity.Users.Core.Application.Exceptions;
using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Core.Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public class LoginVersityUserCommandHandler : IRequestHandler<LoginVersityUserCommand, bool>
{
    private readonly UserManager<VersityUser> _userManager;

    public LoginVersityUserCommandHandler(UserManager<VersityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(LoginVersityUserCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _userManager.FindByEmailAsync(request.Email);
        if (versityUser is null)
            throw new NotFoundException<VersityUser>(nameof(request.Email));
        
        return await _userManager.CheckPasswordAsync(versityUser, request.Password);
    }
}