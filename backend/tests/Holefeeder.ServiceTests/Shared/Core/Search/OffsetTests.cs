using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.ServiceTests.Shared.Core.Search;

public class OffsetTests
{
    [Fact]
    public void Create_Succeeds_WhenValid()
    {
        var result = Offset.Create(0);

        result.Should().BeSuccessful().And.WithValue(Offset.Valid(0));
    }

    [Fact]
    public void Create_Fails_WhenValueIsInvalid()
    {
        var result = Offset.Create(-1);

        result.Should().BeFailure().And.WithError(OffsetErrors.Invalid);
    }
}
