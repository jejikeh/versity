using MediatR;

namespace Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;

public record GiveAdminRoleToUserCommand(string UserId = "") : IRequest<string>;