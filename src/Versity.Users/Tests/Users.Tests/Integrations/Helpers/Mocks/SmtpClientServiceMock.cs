using Infrastructure.Services.EmailServices;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace Presentation.bin;

public class SmtpClientServiceMock : SmptClientService
{
    public override bool IsConnected { get; } = true;
    
    public SmtpClientServiceMock(IEmailServicesConfiguration configuration) : base(configuration)
    {
    }

    public new void Connect()
    {
        
    }

    public override Task<string> SendAsync(
        MimeMessage message,
        CancellationToken cancellationToken = default(CancellationToken),
        ITransferProgress progress = null)
    {
        return Task.FromResult("");
    }

    public override void Disconnect(bool quit, CancellationToken cancellationToken = default (CancellationToken))
    {
        
    }
}