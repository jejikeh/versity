using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.RegisterVersityUser;

public class RegisterVersityUserCommandHandler : IRequestHandler<RegisterVersityUserCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public RegisterVersityUserCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IdentityResult> Handle(RegisterVersityUserCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid().ToString();
        while (await _versityUsersRepository.GetUserByEmailAsync(userId) != null)
            userId = Guid.NewGuid().ToString();
        
        var versityUser = new VersityUser
        {
            Id = userId,
            Email = request.Email,
            PhoneNumber = request.Phone,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = $"{request.FirstName} {request.LastName}",
        };

        var result = await _versityUsersRepository.CreateUserAsync(versityUser, request.Password);
        await _versityUsersRepository.SetUserRoleAsync(versityUser, VersityRole.Member);
        return result;
    }
}