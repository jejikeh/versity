using MediatR;

namespace Application.RequestHandlers.Auth.Commands.GiveAdminRoleToUser;

public record GiveAdminRoleToUserCommand(string UserId) : IRequest<string>;