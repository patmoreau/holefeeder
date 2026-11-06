using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Persistence;

namespace Holefeeder.ServiceTests.StoreItems.Persistence;

public class SearchStoreItemsRequestOrderByClauseTests
{
    [Theory]
    [InlineData("Ascending", "ORDER BY s.code ASC")]
    [InlineData("Descending", "ORDER BY s.code DESC")]
    public void Create_OrdersByCodeColumn_InRequestedDirection(string direction, string expected)
    {
        var sortBy = ASearchStoreItemsRequestSortBy("code", Enum.Parse<SortByDirection>(direction));

        var clause = SearchStoreItemsRequestOrderByClause.Create(sortBy);

        clause.Should().Be(expected);
    }
}
