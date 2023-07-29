using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;

public record GetAllLogsDataQuery(int Page) : IRequest<IEnumerable<LogData>>;
