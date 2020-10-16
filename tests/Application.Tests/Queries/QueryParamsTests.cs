using System;
using FluentAssertions;
using Xunit;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class QueryParamsTests
    {
        [Theory]
        [InlineData(-1, int.MaxValue)]
        [InlineData(0, -1)]
        [InlineData(0, 0)]
        public void GivenConstructor_WhenInvalidOffsetOrLimit_ThenThrowArgumentOutOfRangeException(int offset, int limit)
        {
            Action action = () => _ = new QueryParams(offset, limit, null, null);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
