namespace Domain.Models;

public class Product
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Title { get; set; }
}