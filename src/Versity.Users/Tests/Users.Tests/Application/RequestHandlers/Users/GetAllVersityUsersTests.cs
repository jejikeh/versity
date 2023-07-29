using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.RequestHandlers.Users.Queries.GetAllVersityUsers;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class GetAllVersityUsersTests
{
    public static readonly List<VersityUser> VersityUsersPayload = new Faker<VersityUser>().CustomInstantiator(faker =>
        new VersityUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = faker.Internet.UserName(),
            Email = faker.Internet.Email(),
            FirstName = faker.Name.FirstName(),
            LastName = faker.Name.LastName(),
            PhoneNumber = faker.Phone.PhoneNumber(),
        }).Generate(10);

    public static readonly List<ViewVersityUserDto> DtoPayload = VersityUsersPayload
        .Take(10)
        .Select(userPayload => ViewVersityUserDto.MapFromModel(userPayload, new List<string>() { "Admin" }))
        .ToList();
    
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;

    public GetAllVersityUsersTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldReturnListOfDtos_InAllCases()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository =>
            versityUsersRepository.GetAllUsers()).Returns(VersityUsersPayload.AsQueryable());
        
        _versityUsersRepository.Setup(versityUsersRepository => 
            versityUsersRepository.ToListAsync(It.IsAny<IQueryable<VersityUser>>())).ReturnsAsync(VersityUsersPayload.GetRange(0, 10));

        _versityUsersRepository.Setup(versityUsersRepository =>
            versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>())).ReturnsAsync(new[] { "Admin" });
        
        var request = new GetAllVersityUsersQuery(new Random().Next(0, 10));
        var handler = new GetAllVersityUsersQueryHandler(_versityUsersRepository.Object);
        
        // Act
        var act = await handler.Handle(request, default);
        
        // Assert
        act.Should().BeEquivalentTo(DtoPayload);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var validator = new GetAllVersityUsersQueryValidator();
        var command = new GetAllVersityUsersQuery(-new Random().Next(0, 10));
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPageNumberIsGreaterThanZero()
    {
        // Arrange
        var validator = new GetAllVersityUsersQueryValidator();
        var command = new GetAllVersityUsersQuery(new Random().Next(1, 10));
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
}