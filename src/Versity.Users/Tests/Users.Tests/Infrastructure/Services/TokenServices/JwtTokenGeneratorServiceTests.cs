using System.IdentityModel.Tokens.Jwt;
using Bogus;
using FluentAssertions;
using Infrastructure.Services.TokenServices;
using Moq;

namespace Users.Tests.Infrastructure.Services.TokenServices;

public class JwtTokenGeneratorServiceTests
{
    private readonly Mock<ITokenGenerationConfiguration> _tokenGenerationConfiguration;

    public JwtTokenGeneratorServiceTests()
    {
        _tokenGenerationConfiguration = new Mock<ITokenGenerationConfiguration>();
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken_WhenCalled()
    {
        // Arrange
        var faker = new Faker();
        var handler = new JwtSecurityTokenHandler();
        var userId = Guid.NewGuid().ToString();
        var userEmail = faker.Internet.Email();
        var roles = new[] { "role1", "role2" };
        var issuer = faker.Name.FullName();
        var audience = faker.Internet.Email();
        _tokenGenerationConfiguration.Setup(x => x.Issuer).Returns(issuer);
        _tokenGenerationConfiguration.Setup(x => x.Audience).Returns(audience);
        _tokenGenerationConfiguration.Setup(x => x.Key).Returns(Guid.NewGuid().ToString());
        
        // Act
        var token = handler.ReadJwtToken(new JwtTokenGeneratorService(_tokenGenerationConfiguration.Object)
            .GenerateToken(userId, userEmail, roles));
        
        // Assert
        token.Issuer.Should().Be(issuer);
        token.Audiences.Should().Contain(audience);
        token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value.Should().Be(userId);
        token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value.Should().Be(userEmail);
    }
}