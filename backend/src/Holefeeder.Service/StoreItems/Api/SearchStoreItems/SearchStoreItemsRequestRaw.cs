using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

using Microsoft.AspNetCore.Mvc;

using SearchQueryParser = Holefeeder.Service.Shared.Core.Search.SearchQueryParser;
using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Api.SearchStoreItems;

internal sealed record SearchStoreItemsRequestRaw(
    [FromQuery(Name = "offset")] string? OffsetRaw,
    [FromQuery(Name = "limit")] string? LimitRaw,
    [FromQuery(Name = "sort")] string? SortRaw,
    [FromQuery(Name = "filter")] string? FilterRaw
)
{
    public static Result<SearchStoreItemsRequest> ToSearchStoreItemsRequest(SearchStoreItemsRequestRaw raw, User user)
    {
        var offset = SearchQueryParser.ParseOrDefault(raw.OffsetRaw, Offset.Default, Offset.Create, OffsetErrors.Invalid);
        var limit = SearchQueryParser.ParseOrDefault(raw.LimitRaw, Limit.Default, Limit.Create, LimitErrors.Invalid);
        var filter = SearchQueryParser.ParseFilterBy(raw.FilterRaw, SearchStoreItemsRequestFilterBy.None, SearchStoreItemsRequestFilterBy.Create, SearchStoreItemsRequestFilterByErrors.Invalid);
        var sort = SearchQueryParser.ParseSortBy(raw.SortRaw,SearchStoreItemsRequestSortBy.Default, SearchStoreItemsRequestSortBy.Create, SearchStoreItemsRequestSortByErrors.Invalid);

        var validation = ResultAggregate.Create();
        if (offset.IsFailure)
        {
            validation.AddResult(offset.Error);
        }

        if (limit.IsFailure)
        {
            validation.AddResult(limit.Error);
        }

        if (filter.IsFailure)
        {
            validation.AddResult(filter.Error);
        }

        if (sort.IsFailure)
        {
            validation.AddResult(sort.Error);
        }

        return validation.OnSuccess<SearchStoreItemsRequest>(() =>
            new SearchStoreItemsRequest(user, offset.Value, limit.Value, filter.Value, sort.Value)
        );
    }
}
