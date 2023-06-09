using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public record LoginVersityUserCommand(string Email, string Password) : IRequest<string>;