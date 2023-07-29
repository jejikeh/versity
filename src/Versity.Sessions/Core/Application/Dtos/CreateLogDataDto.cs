using Domain.Models.SessionLogging;

namespace Application.Dtos;

public record CreateLogDataDto(DateTime Time, LogLevel LogLevel, string Data); 