using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.RegisterVersityUser;

public class RegisterVersityUserCommandHandler : IRequestHandler<RegisterVersityUserCommand, IdentityResult>
{
    private readonly UserManager<VersityUser> _userManager;

    public RegisterVersityUserCommandHandler(UserManager<VersityUser> userManager)
    {
        _userManager = userManager;
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

        var result = await _userManager.CreateAsync(versityUser, request.Password);
        await _userManager.AddToRoleAsync(versityUser, "Member");
        return result;
    }
}