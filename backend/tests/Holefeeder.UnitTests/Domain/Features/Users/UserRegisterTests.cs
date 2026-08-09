using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Users;
using Holefeeder.UnitTests.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Features.Users;

[UnitTest]
public class UserRegisterTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenRegister_WhenValid_ThenReturnSuccessWithObject()
    {
        // arrange

        // act
        var result = _driver.Build();

        // assert
        using var scope = new AssertionScope();
        result.Should().BeSuccessful();
        _driver.ShouldBeValid(result.Value);
    }

    [Fact]
    public void GivenRegister_WhenValid_ThenIdentityIsLinkedToTheUser()
    {
        // arrange

        // act
        var user = _driver.Build().Value;

        // assert
        using var scope = new AssertionScope();
        var identity = user.UserIdentities.Should().ContainSingle().Subject;
        identity.IdentityObjectId.Should().Be(Driver.IdentityObjectId);
        identity.UserId.Should().Be(user.Id);
        identity.Inactive.Should().BeFalse();
        // EF maps UserIdentities as a navigation, so the identity must point back at
        // the very same instance or saving would insert a second user.
        identity.User.Should().BeSameAs(user);
    }

    [Theory]
    [ClassData(typeof(IdentityObjectIdValidationData))]
    public void GivenRegister_WhenIdentityObjectIdIsMissing_ThenReturnFailure(string? identityObjectId)
    {
        // arrange
        var driver = _driver.WithIdentityObjectId(identityObjectId!);

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(UserErrors.IdentityObjectIdRequired);
    }

    [Fact]
    public void GivenRegister_WhenCalledTwice_ThenUsersAreDistinct()
    {
        // arrange

        // act
        var first = _driver.Build().Value;
        var second = _driver.Build().Value;

        // assert
        second.Id.Should().NotBe(first.Id);
    }

    private sealed class Driver : IDriverOf<Result<User>>
    {
        public const string IdentityObjectId = "auth0|642a4bd1c2f0e3d7a0f8b111";

        private string _identityObjectId = IdentityObjectId;

        public Driver WithIdentityObjectId(string identityObjectId)
        {
            _identityObjectId = identityObjectId;
            return this;
        }

        public Result<User> Build() => User.Register(_identityObjectId);

        public void ShouldBeValid(User user)
        {
            using var scope = new AssertionScope();
            user.Id.Should().NotBe(UserId.Empty);
            user.Inactive.Should().BeFalse();
            user.UserIdentities.Should().ContainSingle();
        }
    }

    internal sealed class IdentityObjectIdValidationData : TheoryData<string?>
    {
        public IdentityObjectIdValidationData()
        {
            Add(null!);
            Add(string.Empty);
            Add("       ");
        }
    }
}
