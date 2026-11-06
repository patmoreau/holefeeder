using Bogus;

using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Support.Core;

public class IdentityObjectIdTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_Succeeds_WithValidNameIdentifier()
    {
        var id = _faker.Random.AlphaNumeric(10);
        var httpContext = AHttpContext(id);
        var identityObjectId = IdentityObjectId.Create(httpContext.User);

        identityObjectId.Should().BeSuccessful().And.WithValue(IdentityObjectId.Valid(id));
    }

    [Fact]
    public void Create_Succeeds_WithValidSub()
    {
        var id = _faker.Random.AlphaNumeric(10);
        var httpContext = ASubHttpContext(id);
        var identityObjectId = IdentityObjectId.Create(httpContext.User);

        identityObjectId.Should().BeSuccessful().And.WithValue(IdentityObjectId.Valid(id));
    }

    [Fact]
    public void Create_Fails_WithNoNameIdentifier()
    {
        var httpContext = AnEmptyHttpContext();
        var identityObjectId = IdentityObjectId.Create(httpContext.User);

        identityObjectId.Should().BeFailure().And.WithError(IdentityObjectIdErrors.Invalid);
    }
}
