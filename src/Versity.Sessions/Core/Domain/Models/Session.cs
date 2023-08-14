using Domain.Models.SessionLogging;

namespace Domain.Models;

public class Session
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public SessionLogs? Logs { get; set; }
    public DateTime Start { get; set; }
    public DateTime Expiry { get; set; }
    public SessionStatus Status { get; set; }
}