using Application.Abstractions;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.GetAdminRole;

public class GetAdminRoleCommandHandler : IRequestHandler<GetAdminRoleCommand, string>
{
    private readonly UserManager<VersityUser> _userManager;
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;


    public GetAdminRoleCommandHandler(UserManager<VersityUser> userManager, IAuthTokenGeneratorService tokenGeneratorService)
    {
        _userManager = userManager;
        _tokenGeneratorService = tokenGeneratorService;
    }

    public async Task<string> Handle(GetAdminRoleCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _userManager.FindByIdAsync(request.UserId);
        if (versityUser is null)
            throw new NotFoundException<VersityUser>(nameof(request.UserId));

        await _userManager.AddToRoleAsync(versityUser, "Admin");
        var userRoles = await _userManager.GetRolesAsync(versityUser);
        return _tokenGeneratorService.GenerateToken(versityUser.Id, versityUser.NormalizedEmail, userRoles.ToArray());
    }
}