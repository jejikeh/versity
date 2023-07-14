using System.Text.Json.Serialization;

namespace Domain.Models.SessionLogging;

public class LogData
{
    public Guid Id { get; set; }
    public DateTime Time { get; set; }
    public LogLevel LogLevel { get; set; }
    public string Data { get; set; } = string.Empty;
    public Guid SessionLogsId { get; set; }
    [JsonIgnore]
    public SessionLogs SessionLogs { get; set; } = null!;
}