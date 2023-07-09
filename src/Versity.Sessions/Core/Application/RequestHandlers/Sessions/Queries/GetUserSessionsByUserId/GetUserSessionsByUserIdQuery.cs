using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;

public record GetUserSessionsByUserIdQuery(string UserId, int Page) : IRequest<IEnumerable<Session>>;