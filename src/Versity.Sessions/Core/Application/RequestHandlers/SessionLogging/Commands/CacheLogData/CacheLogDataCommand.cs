using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CacheLogData;

public record CacheLogDataCommand(Guid SessionLogsId, DateTime Time, LogLevel LogLevel, string Data) : IRequest; 