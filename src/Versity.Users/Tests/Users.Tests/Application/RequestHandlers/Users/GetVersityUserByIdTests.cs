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
    
    public GetVersityUserByIdTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var request = new GetVersityUserByIdQuery("419b8912-d18f-411e-b665-11e5978237ad");
        var handler = new GetVersityUserByIdQueryHandler(_versityUsersRepository.Object);

        var act = async () => await handler.Handle(request, default);

        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnUserDto_WhenUserExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(GetAllVersityUsersTests.VersityUsersPayload[0]);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserByIdQuery("419b8912-d18f-411e-b665-11e5978237ad");
        var handler = new GetVersityUserByIdQueryHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(request, default);

        result.Should().BeEquivalentTo(GetAllVersityUsersTests.DtoPayload[0]);
    }
    
    [Fact]
    public async Task Validator_ShouldReturnUserDto_WhenUserExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(GetAllVersityUsersTests.VersityUsersPayload[0]);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserByIdQuery("419b8912-d18f-411e-b665-11e5978237ad");
        var handler = new GetVersityUserByIdQueryHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(request, default);

        result.Should().BeEquivalentTo(GetAllVersityUsersTests.DtoPayload[0]);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNull()
    {
        var validator = new GetVersityUserByIdQueryValidator();
        var command = new GetVersityUserByIdQuery(string.Empty);
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNotGuid()
    {
        var validator = new GetVersityUserByIdQueryValidator();
        var command = new GetVersityUserByIdQuery("dd44e461-7217-41ab-8a41-f230381ed8");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenIdIsValidGuid()
    {
        var validator = new GetVersityUserByIdQueryValidator();
        var command = new GetVersityUserByIdQuery("f15f9434-248b-4030-9469-75440a2868f0");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeTrue();
    }
}