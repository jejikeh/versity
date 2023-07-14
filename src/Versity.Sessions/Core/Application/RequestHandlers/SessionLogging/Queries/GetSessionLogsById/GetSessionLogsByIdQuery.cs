using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetSessionLogsById;

public record GetSessionLogsByIdQuery(Guid Id) : IRequest<SessionLogs>;
