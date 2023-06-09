using MediatR;
using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Core.Application.RequestHandlers.Auth.Commands.GetAdminRole;

public record GetAdminRoleCommand(string UserId) : IRequest<VersityUser>;