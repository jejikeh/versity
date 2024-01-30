using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetAllSessionsLogs;

public record GetAllSessionsLogsQuery(int Page) : IRequest<IEnumerable<SessionLogs>>;
