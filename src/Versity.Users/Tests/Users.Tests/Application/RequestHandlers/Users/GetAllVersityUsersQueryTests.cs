using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.RequestHandlers.Users.Queries.GetAllVersityUsers;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class GetAllVersityUsersQueryTests
{
    public static readonly List<VersityUser> VersityUsersPayload = new List<VersityUser>()
    {
        new VersityUser()
        {
            Id = "dd44e461-7217-41ab-8a41-f230381e0ed8",
            UserName = "test1",
            Email = "test1@gmail.com",
            FirstName = "test1",
            LastName = "test1",
            PhoneNumber = "123456789",
        },
        new VersityUser()
        {
            Id = "b7963cd8-449a-47a8-a046-56984803a1a2",
            UserName = "test2",
            Email = "test2@gmail.com",
            FirstName = "test2",
            LastName = "test2",
            PhoneNumber = "223456789",
        },
        new VersityUser()
        {
            Id = "a14628e0-7470-4895-8a7c-6ebf1fa6e241",
            UserName = "test3",
            Email = "test3@gmail.com",
            FirstName = "test3",
            LastName = "test3",
            PhoneNumber = "323456789",
        },
        new VersityUser()
        {
            Id = "2a2aaca4-5375-4451-a6d8-107dec25ae1c",
            UserName = "test4",
            Email = "test4@gmail.com",
            FirstName = "test4",
            LastName = "test4",
            PhoneNumber = "423456789",
        },
        new VersityUser()
        {
            Id = "5dfa77cf-36e4-4239-abff-13ad79797d55",
            UserName = "test5",
            Email = "test5@gmail.com",
            FirstName = "test5",
            LastName = "test5",
            PhoneNumber = "523456789",
        },
        new VersityUser()
        {
            Id = "bc0b7855-e020-4a93-bddc-287c43d79e38",
            UserName = "test6",
            Email = "test6@gmail.com",
            FirstName = "test6",
            LastName = "test6",
            PhoneNumber = "623456789",
        },
        new VersityUser()
        {
            Id = "2faf18e8-873c-444a-9ad2-5551de7f99db",
            UserName = "test7",
            Email = "test7@gmail.com",
            FirstName = "test7",
            LastName = "test7",
            PhoneNumber = "723456789",
        },
        new VersityUser()
        {
            Id = "962161ab-e2fa-4f6b-b23a-7371458950be",
            UserName = "test8",
            Email = "test8@gmail.com",
            FirstName = "test8",
            LastName = "test8",
            PhoneNumber = "823456789",
        },
        new VersityUser()
        {
            Id = "ea5a8468-3e8d-4466-93c3-034c0e0cd4c3",
            UserName = "test9",
            Email = "test9@gmail.com",
            FirstName = "test9",
            LastName = "test9",
            PhoneNumber = "923456789",
        },
        new VersityUser()
        {
            Id = "6032e280-7f63-4497-a8fe-3bc6f9dd06ee",
            UserName = "test10",
            Email = "test10@gmail.com",
            FirstName = "test10",
            LastName = "test10",
            PhoneNumber = "1023456789",
        },
        new VersityUser()
        {
            Id = "419b8912-d18f-411e-b665-11e5978237ad",
            UserName = "test11",
            Email = "test11@gmail.com",
            FirstName = "test11",
            LastName = "test11",
            PhoneNumber = "1123456789",
        },
    };

    private static List<ViewVersityUserDto> _dtoPayload = new List<ViewVersityUserDto>()
    {
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[0], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[1], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[2], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[3], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[4], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[5], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[6], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[7], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[8], new List<string>() { "Admin" }),
        ViewVersityUserDto.MapFromModel(VersityUsersPayload[9], new List<string>() { "Admin" }),
    };
    
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;

    public GetAllVersityUsersQueryTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldReturnListOfDtos_InAllCases()
    {
        _versityUsersRepository.Setup(x =>
            x.GetAllUsers()).Returns(VersityUsersPayload.AsQueryable());
        
        _versityUsersRepository.Setup(x => 
            x.ToListAsync(It.IsAny<IQueryable<VersityUser>>())).ReturnsAsync(VersityUsersPayload.GetRange(0, 10));

        _versityUsersRepository.Setup(x =>
            x.GetRolesAsync(It.IsAny<VersityUser>())).ReturnsAsync(new[] { "Admin" });
        
        var request = new GetAllVersityUsersQuery(1);
        var handler = new GetAllVersityUsersQueryHandler(_versityUsersRepository.Object);
        
        var act = await handler.Handle(request, default);
        
        act.Should().BeEquivalentTo(_dtoPayload);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPageNumberIsLessThanOne()
    {
        var validator = new GetAllVersityUsersQueryValidator();
        var command = new GetAllVersityUsersQuery(-12);
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPageNumberIsGreaterThanZero()
    {
        var validator = new GetAllVersityUsersQueryValidator();
        var command = new GetAllVersityUsersQuery(1);
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeTrue();
    }
}