using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal sealed record SearchStoreItemsRequestSortBy
{
    private static readonly string[] ValidFields = ["code"];

    public string Field { get; }

    public SortByDirection Direction { get; }

    public static readonly SearchStoreItemsRequestSortBy Default = new("code", SortByDirection.Ascending);

    public static Result<SearchStoreItemsRequestSortBy> Create(string field, SortByDirection direction)
    {
        var normalizedField = field.ToLowerInvariant();
        if (!ValidFields.Contains(normalizedField))
        {
            return SearchStoreItemsRequestSortByErrors.Invalid;
        }

        return new SearchStoreItemsRequestSortBy(normalizedField, direction);
    }

    public static Result<SearchStoreItemsRequestSortBy> Create(string? value) =>
        SearchQueryParser.ParseSortBy(value, Default, Create, SearchStoreItemsRequestSortByErrors.Invalid);

    public static SearchStoreItemsRequestSortBy Valid(string field, SortByDirection direction) => new(field, direction);

    private SearchStoreItemsRequestSortBy(string field, SortByDirection direction) => (Field, Direction) = (field, direction);
}

internal static class SearchStoreItemsRequestSortByErrors
{
    public const string CodeInvalid = "search-store-items-request-sort-by-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Sort is invalid.");
}
