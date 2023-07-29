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
    
    public GetAllRefreshTokensTests()
    {
        _tokensRepository = new Mock<IVersityRefreshTokensRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExist()
    {
        // Arrange
        var itemsCount = new Random().Next(1, 10);
        var command = new GetAllRefreshTokensQuery();
        var handler = new GetAllRefreshTokensQueryHandler(_tokensRepository.Object);
        
        _tokensRepository.Setup(versityRefreshTokensRepository =>
                versityRefreshTokensRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeRefreshTokens(itemsCount));
        
        // Act
        var results = await handler.Handle(command, default);

        // Assert
        results.ToList().Count.Should().Be(itemsCount);
    }
    
    private List<RefreshToken> GenerateFakeRefreshTokens(int count)
    {
        return new Faker<RefreshToken>().CustomInstantiator(faker => new RefreshToken()
        {
            Id = faker.Random.Guid(),
            UserId = faker.Random.String(),
            Token = faker.Random.String(),
            IsUsed = faker.Random.Bool(),
            IsRevoked = faker.Random.Bool(),
            AddedTime = faker.Date.Past(),
            ExpiryTime = faker.Date.Future()
        }).Generate(count);
    }
}