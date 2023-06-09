using Application.Abstractions;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public class LoginVersityUserCommandHandler : IRequestHandler<LoginVersityUserCommand, string>
{
    private readonly UserManager<VersityUser> _userManager;
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;
    
    public LoginVersityUserCommandHandler(UserManager<VersityUser> userManager, IAuthTokenGeneratorService tokenGeneratorService)
    {
        _userManager = userManager;
        _tokenGeneratorService = tokenGeneratorService;
    }

    public async Task<string> Handle(LoginVersityUserCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _userManager.FindByEmailAsync(request.Email);
        if (versityUser is null)
            throw new NotFoundException<VersityUser>(nameof(request.Email));

        if (!await _userManager.CheckPasswordAsync(versityUser, request.Password))
            throw new Exception("Invalid username or password!");

        var userRoles = await _userManager.GetRolesAsync(versityUser);
        return _tokenGeneratorService.GenerateToken(versityUser.Id, versityUser.NormalizedEmail, userRoles.ToArray());
    }
}