using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetSessionById;

public record GetSessionByIdQuery(Guid Id) : IRequest<Session>;
