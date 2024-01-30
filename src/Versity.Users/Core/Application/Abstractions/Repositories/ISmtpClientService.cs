using MailKit.Net.Smtp;

namespace Application.Abstractions.Repositories;

public interface ISmtpClientService : ISmtpClient
{
    public void Connect();
}