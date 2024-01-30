using Application.Dtos;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public record GetAllSessionsQuery(int Page) : IRequest<IEnumerable<SessionViewModel>>;
