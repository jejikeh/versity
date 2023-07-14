using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetLogDataById;

public record GetLogDataByIdQuery(Guid Id) : IRequest<LogData>;
