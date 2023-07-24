using Bogus;
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
        // Arrange
        var faker = new Faker();
        var emailSender = new EmailSenderService(_smtpClient.Object, _emailServicesConfiguration.Object);
        var name = faker.Name.FullName();
        var email = faker.Internet.Email();
        var subject = faker.Lorem.Word();
        var message = faker.Lorem.Paragraph();
        _emailServicesConfiguration
            .Setup(configuration =>  configuration.From)
            .Returns("from@gmail.com");

        // Act
        await emailSender.SendEmailAsync(name, email, subject, message);

        // Assert
        _smtpClient.Verify(smtpClient => smtpClient.SendAsync(
            It.Is<MimeMessage>(mimeMessage => MatchMimeMessage(mimeMessage, subject, email, name, message)),
            It.IsAny<CancellationToken>(),
            It.IsAny<ITransferProgress>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldDisconnectSmtpClient_WhenDisposed()
    {
        // Arrange
        using var emailSender = new EmailSenderService(_smtpClient.Object, _emailServicesConfiguration.Object);
        
        // Act
        emailSender.Dispose();
        
        // Assert
        _smtpClient.Verify(smtpClient => smtpClient.Disconnect(true, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    private static bool MatchMimeMessage(MimeMessage mimeMessage, string subject, string email, string name, string message)
    {
        var subjectCase = mimeMessage.Subject == subject;
        var toCase = mimeMessage.To.Contains(MailboxAddress.Parse(email));
        var fromCase = mimeMessage.From.Contains(new MailboxAddress(name, "from@gmail.com"));
        var bodyCase = mimeMessage.HtmlBody == message;
        
        return subjectCase && toCase && fromCase && bodyCase;
    }
}