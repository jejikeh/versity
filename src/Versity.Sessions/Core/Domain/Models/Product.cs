using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models;

[Serializable]
public class Product
{
    [BsonId, BsonElement("_id")]
    public Guid Id { get; set; }
    
    [BsonElement("external_id")]
    public Guid ExternalId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; }
}