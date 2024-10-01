using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Users;

namespace Holefeeder.UnitTests.Domain.Features.Users;

[UnitTest, Category("Domain")]
public class UserTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenCreate_WhenValid_ThenReturnSuccessWithObject()
    {
        // arrange

        // act
        var result = _driver.Build();

        // assert
        using var scope = new AssertionScope();
        result.Should().BeSuccessful().WithValue(result.Value);
        _driver.ShouldBeValid(result.Value);
    }

    private sealed class Driver : IDriverOf<Result<User>>
    {
        public Result<User> Build() => User.Create();

        public void ShouldBeValid(User storeItem)
        {
            using var scope = new AssertionScope();
            storeItem.Id.Should().NotBe(UserId.Empty);
        }
    }
}
