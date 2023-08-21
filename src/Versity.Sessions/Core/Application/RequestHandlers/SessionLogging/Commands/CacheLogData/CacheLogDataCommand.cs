using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Commands.CacheLogData;

public class CacheLogDataCommand : IRequest, IEquatable<CacheLogDataCommand>
{
    public Guid SessionLogsId { get; }
    public DateTime Time { get; }
    public LogLevel LogLevel { get; }
    public string Data { get; }
    
    public CacheLogDataCommand(
        Guid sessionLogsId, 
        DateTime time, 
        LogLevel logLevel, 
        string data)
    {
        SessionLogsId = sessionLogsId;
        Time = time;
        LogLevel = logLevel;
        Data = data;
    }


    public bool Equals(CacheLogDataCommand? other)
    {
        return Equals(other?.LogLevel, LogLevel) &&
               other.SessionLogsId.Equals(SessionLogsId) &&
               other.Data.Equals(Data) &&
               other.Time.Equals(Time) ||
               Equals(other, this);
    }
} 