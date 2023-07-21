using System.Text;
using Application.Abstractions;
using Domain.Models;
using Infrastructure.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;

namespace Users.Tests.Infrastructure.EmailServices;

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
        _userManager.Setup(
            x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync("token");
        
        var emailConfirmMessageService = new SendConfirmMessageEmailService(_userManager.Object, _emailSenderService.Object, _configuration.Object);
        var user = new VersityUser()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Ivan",
            Email = "a@a.com"
        };

        await emailConfirmMessageService.SendEmailConfirmMessageAsync(user);
        
        _emailSenderService.Verify(x => x.SendEmailAsync(
            "Versity Identity Server", 
            user.Email,
            "Confirm Email", 
            It.IsAny<string>()), 
            Times.Once);
    }
}