using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Queries.GetVersityUserById;
using Application.RequestHandlers.Users.Queries.GetVersityUserRoles;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class GetVersityUserByIdTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly GetVersityUserByIdQueryHandler _getVersityUserByIdQueryHandler;
    private readonly GetVersityUserByIdQueryValidator _getVersityUserByIdQueryValidator;
    
    public GetVersityUserByIdTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _getVersityUserByIdQueryHandler = new GetVersityUserByIdQueryHandler(_versityUsersRepository.Object);
        _getVersityUserByIdQueryValidator = new GetVersityUserByIdQueryValidator();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExists()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var request = new GetVersityUserByIdQuery(Guid.NewGuid().ToString());

        // Act
        var act = async () => await _getVersityUserByIdQueryHandler.Handle(request, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(GetAllVersityUsersTests.VersityUsersPayload[0]);
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserByIdQuery(Guid.NewGuid().ToString());

        // Act
        var result = await _getVersityUserByIdQueryHandler.Handle(request, default);

        // Assert
        result.Should().BeEquivalentTo(GetAllVersityUsersTests.DtoPayload[0]);
    }
    
    [Fact]
    public async Task Validator_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(GetAllVersityUsersTests.VersityUsersPayload[0]);
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserByIdQuery(Guid.NewGuid().ToString());

        // Act
        var result = await _getVersityUserByIdQueryHandler.Handle(request, default);

        // Assert
        result.Should().BeEquivalentTo(GetAllVersityUsersTests.DtoPayload[0]);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNull()
    {
        // Arrange
        var command = new GetVersityUserByIdQuery(string.Empty);
        
        // Act
        var result = await _getVersityUserByIdQueryValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNotGuid()
    {
        // Arrange
        var command = new GetVersityUserByIdQuery("not a guid");
        
        // Act
        var result = await _getVersityUserByIdQueryValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenIdIsValidGuid()
    {
        // Arrange
        var command = new GetVersityUserByIdQuery(Guid.NewGuid().ToString());
        
        // Act
        var result = await _getVersityUserByIdQueryValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
}