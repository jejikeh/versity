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
}
