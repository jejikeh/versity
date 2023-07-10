namespace Domain.Models;

public class Session
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime Start { get; set; }
    public DateTime Expiry { get; set; }
    public SessionStatus Status { get; set; }
}