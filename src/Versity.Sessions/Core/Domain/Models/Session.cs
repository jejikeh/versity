using System.ComponentModel.DataAnnotations.Schema;
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
    public string UserId { get; set; } = new Guid().ToString();
    
    [BsonElement("product_id")]
    public Guid ProductId { get; set; }

    [BsonIgnore] 
    public Product Product { get; set; } = null!;

    [BsonElement("session_logs_id"), Column("LogsId")]
    public Guid LogsId { get; set; }

    [BsonIgnore] 
    public SessionLogs SessionLogs { get; set; } = null!;

    [BsonElement("start"), BsonRepresentation(BsonType.DateTime)]
    public DateTime Start { get; set; }
    
    [BsonElement("expiry"), BsonRepresentation(BsonType.DateTime)]
    public DateTime Expiry { get; set; }
    
    [BsonElement("status")]
    public SessionStatus Status { get; set; }
}