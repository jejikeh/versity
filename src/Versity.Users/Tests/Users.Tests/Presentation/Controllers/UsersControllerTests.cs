using Application.Dtos;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.RefreshJwtToken;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Application.RequestHandlers.Users.Commands.ChangeUserPassword;
using Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;
using Application.RequestHandlers.Users.Queries.GetAllVersityUsers;
using Application.RequestHandlers.Users.Queries.GetVersityUserById;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace Users.Tests.Presentation.Controllers;

public class UsersControllerTests
{
    private Mock<ISender> _sender;

    public UsersControllerTests()
    {
        _sender = new Mock<ISender>();
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<GetVersityUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeUser());
        var service = new UsersController(_sender.Object);
        
        // Act
        var response = await service.GetUserById(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOk_WhenUserExist()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<GetAllVersityUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeUserList());
        var service = new UsersController(_sender.Object);
        
        // Act
        var response = await service.GetAllUsers(1, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ChangeUserPassword_ShouldReturnOk_WhenUserExist()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<ChangeUserPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        var request = new ChangeUserPasswordDto("test", "test");
        var service = new UsersController(_sender.Object);
        
        // Act
        var response = await service.ChangeUserPassword(Guid.NewGuid(), request, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task SetAdmin_ShouldReturnOk_WhenUserExist()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<GiveAdminRoleToUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());
        var service = new UsersController(_sender.Object);
        
        // Act
        var response = await service.SetAdmin(Guid.NewGuid(),CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    private static ViewVersityUserDto GenerateFakeUser()
    {
        return new Faker<ViewVersityUserDto>().CustomInstantiator(f => new ViewVersityUserDto(
            Guid.NewGuid().ToString(),
            f.Name.FirstName(),
            f.Name.LastName(),
            f.Internet.Email(),
            f.Phone.PhoneNumber(),
            new []{ "Member"})).Generate();
    }
    
    private static List<ViewVersityUserDto> GenerateFakeUserList()
    {
        return new Faker<ViewVersityUserDto>().CustomInstantiator(f => new ViewVersityUserDto(
            Guid.NewGuid().ToString(),
            f.Name.FirstName(),
            f.Name.LastName(),
            f.Internet.Email(),
            f.Phone.PhoneNumber(),
            new []{ "Member"})).Generate(20);
    }
}