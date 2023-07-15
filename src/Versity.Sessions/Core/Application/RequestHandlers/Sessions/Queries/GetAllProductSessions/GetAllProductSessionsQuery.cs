using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;

public record GetAllProductSessionsQuery(Guid ProductId, int Page) : IRequest<IEnumerable<SessionViewModel>>;
