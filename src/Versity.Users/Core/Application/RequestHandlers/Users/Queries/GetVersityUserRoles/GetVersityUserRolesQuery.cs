using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserRoles;

public record GetVersityUserRolesQuery(string UserId) : IRequest<IEnumerable<string>>;
