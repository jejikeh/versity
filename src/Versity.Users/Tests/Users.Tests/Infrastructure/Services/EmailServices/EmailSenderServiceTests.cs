using Infrastructure.Services.EmailServices;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;

namespace Users.Tests.Infrastructure.Services.EmailServices;

public class EmailSenderServiceTests
{
    private readonly Mock<ISmtpClient> _smtpClient;
    private readonly Mock<IEmailServicesConfiguration> _emailServicesConfiguration;

    public EmailSenderServiceTests()
    {
        _emailServicesConfiguration = new Mock<IEmailServicesConfiguration>();
        _smtpClient = new Mock<ISmtpClient>();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldCreateMimeMessageAndCallSmtpClientSendAsync_WhenCalled()
    {
        var emailSender = new EmailSenderService(_smtpClient.Object, _emailServicesConfiguration.Object);
        var name = "name";
        var email = "email";
        var subject = "subject";
        var message = "message";
        _emailServicesConfiguration
            .Setup(configuration =>  configuration.From)
            .Returns("from@gmail.com");

        await emailSender.SendEmailAsync(name, email, subject, message);

        _smtpClient.Verify(smtpClient => smtpClient.SendAsync(
            It.Is<MimeMessage>(mimeMessage => MatchMimeMessage(mimeMessage, subject, email, name, message)),
            It.IsAny<CancellationToken>(),
            It.IsAny<ITransferProgress>()), Times.Once);
    }
    
    private bool MatchMimeMessage(MimeMessage mimeMessage, string subject, string email, string name, string message)
    {
        var subjectCase = mimeMessage.Subject == subject;
        var toCase = mimeMessage.To.Contains(MailboxAddress.Parse(email));
        var fromCase = mimeMessage.From.Contains(new MailboxAddress(name, "from@gmail.com"));
        var bodyCase = mimeMessage.HtmlBody == message;
        
        return subjectCase && toCase && fromCase && bodyCase;
    }
    
    [Fact]
    public async Task SendEmailAsync_ShouldDisconnectSmtpClient_WhenDisposed()
    {
        using var emailSender = new EmailSenderService(_smtpClient.Object, _emailServicesConfiguration.Object);
        
        emailSender.Dispose();
        
        _smtpClient.Verify(smtpClient => smtpClient.Disconnect(true, It.IsAny<CancellationToken>()), Times.Once);
    }
}