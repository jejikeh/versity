using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;

public record CreateLogsDataCommand(Guid SessionLogsId, IEnumerable<CreateLogDataDto> CreateLogDataCommands) : IRequest<IEnumerable<LogData>>;