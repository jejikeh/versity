using MediatR;

namespace Application.RequestHandlers.Auth.Commands.GetAdminRole;

public record GetAdminRoleCommand(string UserId) : IRequest<string>;