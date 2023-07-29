using Application.Common;
using Application.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;

namespace Users.Tests.Application.Common;

public class UtilsTests
{
    [Fact]
    public void AggregateIdentityErrorsAndThrow_ShouldThrowExceptionAndAggregateErrors_WhenIdentityResultIsFailed()
    {
        var identityResult = IdentityResult.Failed(new []
        {
            new IdentityError() { Code = "200", Description = "The user does not exist." },
            new IdentityError() { Code = "201", Description = "The user already exists." },
            new IdentityError() { Code = "202", Description = "The password is incorrect." },
                new IdentityError() { Code = "203", Description = "The password is too short." }
        });

        var act = () => Utils.AggregateIdentityErrorsAndThrow(identityResult);

        act.Should().Throw<IdentityExceptionWithStatusCode>().WithMessage(
            "The user does not exist.\nThe user already exists.\nThe password is incorrect.\nThe password is too short.\n");
    }
    
    [Fact]
    public void GenerateRandomString_LengthIsCorrect()
    {
        var length = 10;

        var result = Utils.GenerateRandomString(length);

        result.Should().HaveLength(length);
    }
    
    [Fact]
    public void GenerateRandomString_GeneratesUniqueStrings()
    {
        var length = 8;

        var result1 = Utils.GenerateRandomString(length);
        var result2 = Utils.GenerateRandomString(length);

        result1.Should().NotBe(result2);
    }
}