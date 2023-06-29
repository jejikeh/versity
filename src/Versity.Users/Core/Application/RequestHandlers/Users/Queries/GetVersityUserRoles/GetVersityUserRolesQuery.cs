using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserRoles;

public record GetVersityUserRolesCommand(string UserId) : IRequest<IEnumerable<string>>;
