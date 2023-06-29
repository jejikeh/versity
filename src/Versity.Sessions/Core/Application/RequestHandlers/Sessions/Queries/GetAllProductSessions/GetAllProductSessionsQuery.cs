using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;

public record GetAllProductSessionsQuery(Guid ProductId) : IRequest<IEnumerable<Session>>;
