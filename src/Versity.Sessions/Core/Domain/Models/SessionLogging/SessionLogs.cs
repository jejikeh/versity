using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.SessionLogging;

[Serializable]
public class SessionLogs
{
    [BsonId, BsonElement("_id")]
    public Guid Id { get; set; }
    
    [BsonElement("session_id")]
    public Guid SessionId { get; set; }

    [BsonIgnore] 
    public Session Session { get; set; } = null!;

    [BsonIgnore, JsonIgnore] 
    public ICollection<LogData> Logs { get; set; } = null!;
}
