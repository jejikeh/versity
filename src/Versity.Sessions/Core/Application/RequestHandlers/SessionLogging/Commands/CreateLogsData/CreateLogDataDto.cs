using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;

public record CreateLogDataDto(DateTime Time, LogLevel LogLevel, string Data); 