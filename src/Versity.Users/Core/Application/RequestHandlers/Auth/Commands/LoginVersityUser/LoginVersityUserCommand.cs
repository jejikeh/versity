using MediatR;
using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Core.Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public record LoginVersityUserCommand(string Email, string Password) : IRequest<(VersityUser, bool)>;