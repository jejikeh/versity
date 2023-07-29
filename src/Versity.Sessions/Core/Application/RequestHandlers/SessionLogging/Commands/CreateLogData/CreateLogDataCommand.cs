using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CreateLogData;

public record CreateLogDataCommand(Guid SessionLogsId, DateTime Time, LogLevel LogLevel, string Data) : IRequest<LogData>; 