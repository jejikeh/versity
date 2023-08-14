using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("Product")]
public class Product
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Title { get; set; }
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}