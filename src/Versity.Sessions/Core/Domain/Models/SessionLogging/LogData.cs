using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.SessionLogging;

[Serializable]
public class LogData
{
    [BsonId, BsonElement("id")]
    public Guid Id { get; set; }
    
    [BsonElement("time")]
    public DateTime Time { get; set; }
    
    [BsonElement("log-level")]
    public LogLevel LogLevel { get; set; }
    
    [BsonElement("data")]
    public string Data { get; set; } = string.Empty;
    
    [BsonElement("session_logs_id")]
    public Guid SessionLogsId { get; set; }

    [BsonIgnore] 
    public SessionLogs SessionLogs { get; set; } = null!;
}