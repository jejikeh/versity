using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.RegisterVersityUser;

public record RegisterVersityUserCommand(
    string FirstName, 
    string LastName,
    string Email,
    string Phone,
    string Password) : IRequest<IdentityResult>;
