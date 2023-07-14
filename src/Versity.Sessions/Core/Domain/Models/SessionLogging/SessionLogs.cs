using System.Text.Json.Serialization;

namespace Domain.Models.SessionLogging;

public class SessionLogs
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    [JsonIgnore]
    public Session Session { get; set; } = null!;
    public ICollection<LogData> Logs { get; set; } = new List<LogData>();
}