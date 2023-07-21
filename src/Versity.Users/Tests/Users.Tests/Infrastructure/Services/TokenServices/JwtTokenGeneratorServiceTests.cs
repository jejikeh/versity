using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Infrastructure.Services.TokenServices;
using Moq;

namespace Users.Tests.Infrastructure.TokenServices;

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
        var handler = new JwtSecurityTokenHandler();
        var userId = "userId";
        var userEmail = "userEmail";
        var roles = new[] { "role1", "role2" };
        _tokenGenerationConfiguration.Setup(x => x.Issuer).Returns("issuer");
        _tokenGenerationConfiguration.Setup(x => x.Audience).Returns("audience");
        _tokenGenerationConfiguration.Setup(x => x.Key).Returns("7e0d35bd-c2ca-49b6-8478-4f16b8f40fdc");
        
        var token = handler.ReadJwtToken(new JwtTokenGeneratorService(_tokenGenerationConfiguration.Object)
            .GenerateToken(userId, userEmail, roles));
        
        token.Issuer.Should().Be("issuer");
        token.Audiences.Should().Contain("audience");
        token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value.Should().Be(userId);
        token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value.Should().Be(userEmail);
    }
}