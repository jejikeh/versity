namespace Application.Abstractions;

public interface IEmailSenderService
{
    public Task SendEmailAsync(string name, string email, string subject, string message);
}