using Application.Abstractions.Repositories;
using Application.RequestHandlers.Auth.Queries;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class GetAllRefreshTokensTests
{
    private readonly Mock<IVersityRefreshTokensRepository> _tokensRepository;
    private const int ItemsCount = 7;
    
    public GetAllRefreshTokensTests()
    {
        _tokensRepository = new Mock<IVersityRefreshTokensRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExist()
    {
        // Arrange
        var command = new GetAllRefreshTokensQuery();
        var handler = new GetAllRefreshTokensQueryHandler(_tokensRepository.Object);
        _tokensRepository.Setup(x =>
                x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeRefreshTokens());
        
        // Act
        var results = await handler.Handle(command, default);

        // Assert
        results.ToList().Count.Should().Be(ItemsCount);
    }
    
    private List<RefreshToken> GenerateFakeRefreshTokens()
    {
        return new Faker<RefreshToken>().CustomInstantiator(x => new RefreshToken()
        {
            Id = x.Random.Guid(),
            UserId = x.Random.String(),
            Token = x.Random.String(),
            IsUsed = x.Random.Bool(),
            IsRevoked = x.Random.Bool(),
            AddedTime = x.Date.Past(),
            ExpiryTime = x.Date.Future()
        }).Generate(ItemsCount);
    }
}