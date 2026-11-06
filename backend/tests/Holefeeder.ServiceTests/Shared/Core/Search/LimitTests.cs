using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.ServiceTests.Shared.Core.Search;

public class LimitTests
{
    [Fact]
    public void Create_Succeeds_WhenValid()
    {
        var result = Limit.Create(1);

        result.Should().BeSuccessful().And.WithValue(Limit.Valid(1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_Fails_WhenValueIsInvalid(int value)
    {
        var result = Limit.Create(value);

        result.Should().BeFailure().And.WithError(LimitErrors.Invalid);
    }
}
