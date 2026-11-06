using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Shared.Core;

public class IdTests
{
    [Fact]
    public void Create_Succeeds_WhenValid()
    {
        var value = Guid.NewGuid();
        var result = Id.Create(value);

        result.Should().BeSuccessful().And.WithValue(Id.Valid(value));
    }

    [Fact]
    public void Create_Fails_WhenValueIsInvalid()
    {
        var value = Guid.Empty;
        var result = Id.Create(value);

        result.Should().BeFailure().And.WithError(IdErrors.Invalid);
    }
}
