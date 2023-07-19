using Application.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Users.Tests.Infrastructure;

public class EmailConfirmMessageGmailServiceTests
{
    private readonly Mock<UserManager<VersityUser>> _userManager;
    private readonly Mock<IEmailSenderService> _emailSenderService;

    public EmailConfirmMessageGmailServiceTests()
    {
        _userManager = new Mock<UserManager<VersityUser>>();
        _emailSenderService = new Mock<IEmailSenderService>();
    }
}