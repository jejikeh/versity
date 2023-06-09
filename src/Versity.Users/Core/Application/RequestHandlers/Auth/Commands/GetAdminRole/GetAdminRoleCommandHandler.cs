using MediatR;
using Microsoft.AspNetCore.Identity;
using Versity.Users.Core.Application.Exceptions;
using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Core.Application.RequestHandlers.Auth.Commands.GetAdminRole;

public class GetAdminRoleCommandHandler : IRequestHandler<GetAdminRoleCommand, VersityUser>
{
    private readonly UserManager<VersityUser> _userManager;

    public GetAdminRoleCommandHandler(UserManager<VersityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<VersityUser> Handle(GetAdminRoleCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _userManager.FindByIdAsync(request.UserId);
        if (versityUser is null)
            throw new NotFoundException<VersityUser>(nameof(request.UserId));

        await _userManager.AddToRoleAsync(versityUser, "Admin");
        return versityUser;
    }
}