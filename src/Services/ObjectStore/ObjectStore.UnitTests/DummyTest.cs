using FluentAssertions;

using Xunit;

namespace ObjectStore.UnitTests
{
    public class DummyTest
    {
        [Fact]
        public void DummyFailure()
        {
            true.Should().BeFalse();
        }
    }
}
