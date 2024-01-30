using Application.Abstractions;
using Bogus;
using Domain.Models;
using Infrastructure.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Users.Tests.Infrastructure.Services.EmailServices;

public class SendConfirmMessageEmailServiceTests
{
    private readonly Mock<UserManager<VersityUser>> _userManager;
    private readonly Mock<IEmailSenderService> _emailSenderService;
    private readonly Mock<IEmailServicesConfiguration> _configuration;

    public SendConfirmMessageEmailServiceTests()
    {
        _userManager = new Mock<UserManager<VersityUser>>(Mock.Of<IUserStore<VersityUser>>(), null, null, null, null, null, null, null, null);
        _emailSenderService = new Mock<IEmailSenderService>();
        _configuration = new Mock<IEmailServicesConfiguration>();
    }

    [Fact]
    public async Task SendEmailConfirmMessageAsync_ShouldSendMessageWithCorrectData_WhenCalled()
    {
        // Arrange
        var faker = new Faker();
        var firstName = faker.Name.FirstName();
        var email = faker.Internet.Email();
        
        _userManager.Setup(userManager => 
                userManager.GenerateEmailConfirmationTokenAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(Guid.NewGuid().ToString());
        
        var emailConfirmMessageService = new SendConfirmMessageEmailService(_userManager.Object, _emailSenderService.Object, _configuration.Object);
        var user = new VersityUser()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = firstName,
            Email = email
        };

        // Act
        await emailConfirmMessageService.SendEmailConfirmMessageAsync(user);
        
        // Assert
        _emailSenderService.Verify(emailSenderService => 
                emailSenderService.SendEmailAsync(
                    "Versity Identity Server", 
                    user.Email, 
                    "Confirm Email", 
                    It.IsAny<string>()), 
            Times.Once);
    }
}