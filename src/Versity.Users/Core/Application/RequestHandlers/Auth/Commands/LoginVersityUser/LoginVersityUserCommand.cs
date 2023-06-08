using MediatR;

namespace Versity.Users.Core.Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public record LoginVersityUserCommand(string Email, string Password) : IRequest<bool>;