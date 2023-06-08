using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Versity.Users.Core.Application.RequestHandlers.Commands.RegisterVersityUser;

public record RegisterVersityUserCommand(
    string FirstName, 
    string LastName,
    string Email,
    string Phone,
    string Password) : IRequest<IdentityResult>;
