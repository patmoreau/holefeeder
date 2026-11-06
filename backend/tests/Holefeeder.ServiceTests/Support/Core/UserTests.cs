using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Support.Core;

public class UserTests
{
    [Fact]
    public void Create_WhenValid_ReturnsSuccess()
    {
        var id = AnId();
        var identityObjectId = AnIdentityObjectId();
        var expected = User.Valid(id, identityObjectId);
        var result = User.Create(id, identityObjectId);

        result.Should().BeSuccessful().And.WithValue(expected);
    }

    [Fact]
    public void Create_WhenIdIsEmpty_ReturnsError()
    {
        var result = User.Create(Id.Empty, AnIdentityObjectId());

        result.Should().BeFailure().And.WithError(UserErrors.IdRequired);
    }

    [Fact]
    public void Create_WhenIdentityObjectIdIsEmpty_ReturnsError()
    {
        var result = User.Create(AnId(), IdentityObjectId.Empty);

        result.Should().BeFailure().And.WithError(UserErrors.IdentityObjectIdRequired);
    }

    [Fact]
    public void Create_SucceedsWithNoUser_WhenBothIdAreEmpty()
    {
        var result = User.Create(Id.Empty, IdentityObjectId.Empty);

        result.Should().BeSuccessful().And.WithValue(User.NoUser);
    }
}
