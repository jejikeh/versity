using Application.Abstractions.Repositories;
using Application.RequestHandlers.Auth.Queries;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class GetAllRefreshTokensTests
{
    private readonly Mock<IVersityRefreshTokensRepository> _tokensRepository;
    private readonly IEnumerable<RefreshToken> _payload = new[]
    {
        new RefreshToken
        {
            Id = Guid.Parse("dd44e461-7217-41ab-8a41-f230381e0ed8"),
            UserId = "dd44e461-7217-41ab-8a41-f230381e0ed8",
            Token = "dd44e461-7217-41ab-8a41-f230381e0ed8",
            IsUsed = false,
            IsRevoked = false,
            AddedTime = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddDays(1)
        },
        new RefreshToken
        {
            Id = Guid.Parse("dd44e461-7217-41ab-8a41-f230381e0ed9"),
            UserId = "dd44e461-7217-41ab-8a41-f230381e0ed8",
            Token = "dd44e461-7217-41ab-8a41-f230381e0ed8",
            IsUsed = true,
            IsRevoked = false,
            AddedTime = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddDays(2)
        }
    };

    public GetAllRefreshTokensTests()
    {
        _tokensRepository = new Mock<IVersityRefreshTokensRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExist()
    {
        _tokensRepository.Setup(x =>
                x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_payload);
        
        var command = new GetAllRefreshTokensQuery();
        var handler = new GetAllRefreshTokensQueryHandler(_tokensRepository.Object);
        
        var results = await handler.Handle(command, default);

        results.Should().Equal(_payload);
    }
}