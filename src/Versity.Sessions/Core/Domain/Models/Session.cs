using Domain.Models.SessionLogging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models;

[Serializable]
public class Session
{
    [BsonId, BsonElement("id")]
    public Guid Id { get; set; }
    
    [BsonElement("user_id")]
    public string UserId { get; set; }
    
    [BsonElement("product_id")]
    public Guid ProductId { get; set; }
    
    [BsonElement("session_logs_id")]
    public Guid? LogsId { get; set; }
    
    [BsonElement("start"), BsonRepresentation(BsonType.DateTime)]
    public DateTime Start { get; set; }
    
    [BsonElement("expiry"), BsonRepresentation(BsonType.DateTime)]
    public DateTime Expiry { get; set; }
    
    [BsonElement("status")]
    public SessionStatus Status { get; set; }
}