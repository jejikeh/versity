namespace Infrastructure.Services.EmailServices;

public interface IEmailServicesConfiguration
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string ConfirmUrl { get; set; }
}