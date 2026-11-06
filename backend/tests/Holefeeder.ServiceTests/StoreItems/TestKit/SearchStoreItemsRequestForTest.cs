using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal static class SearchStoreItemsRequestForTest
{
    public static SearchStoreItemsRequest ASearchStoreItemsRequest(
        User? user = null,
        Offset? offset = null,
        Limit? limit = null,
        SearchStoreItemsRequestFilterBy? filterBy = null,
        SearchStoreItemsRequestSortBy? sortBy = null) =>
        new(user ?? AUser(), offset ?? Offset.Default, limit ?? Limit.Default, filterBy ?? ASearchStoreItemsRequestFilterBy(), sortBy ?? ASearchStoreItemsRequestSortBy());
}

internal static class SearchStoreItemsRequestFilterByForTest
{
    public static SearchStoreItemsRequestFilterBy ASearchStoreItemsRequestFilterBy(Code? code = null, FilterByOperator? filterByOperator = null) =>
        SearchStoreItemsRequestFilterBy.ByCode.Valid(code ?? ACode(), filterByOperator ?? FilterByOperator.Equal);
}

internal static class SearchStoreItemsRequestSortByForTest
{
    public static SearchStoreItemsRequestSortBy ASearchStoreItemsRequestSortBy(string? field = null, SortByDirection? direction = null) =>
        SearchStoreItemsRequestSortBy.Valid(field ?? "code", direction ?? SortByDirection.Ascending);
}
