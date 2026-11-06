using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

namespace Holefeeder.Service.StoreItems.Persistence;

internal class SearchStoreItemsRequestOrderByClause
{
    private static readonly Dictionary<string, string> Column = new(StringComparer.OrdinalIgnoreCase)
    {
        ["code"] = "s.code"
    };

    private static readonly Dictionary<SortByDirection, string> Direction = new()
    {
        [SortByDirection.Ascending] = "ASC",
        [SortByDirection.Descending] = "DESC"
    };

    public static string Create(SearchStoreItemsRequestSortBy sortBy)
    {
        var orderByField = Column[sortBy.Field];
        var orderByDirection = Direction[sortBy.Direction];
        return $"ORDER BY {orderByField} {orderByDirection}";
    }
}
