using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public record GetAllVersityUsersCommand(int Page) : IRequest<IEnumerable<ViewVersityUserDto>>;
