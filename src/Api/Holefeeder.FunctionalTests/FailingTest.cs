using FluentAssertions;

using Xunit;

namespace Holefeeder.FunctionalTests;

public class FailingTest
{
    [Fact]
    public void GivenFailingTest_Fail()
    {
        1.Should().Be(2);
    }
}
