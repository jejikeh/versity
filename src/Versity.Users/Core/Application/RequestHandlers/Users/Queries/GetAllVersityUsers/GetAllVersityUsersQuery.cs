using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public record GetAllVersityUsersQuery(int Page) : IRequest<IEnumerable<ViewVersityUserDto>>;
