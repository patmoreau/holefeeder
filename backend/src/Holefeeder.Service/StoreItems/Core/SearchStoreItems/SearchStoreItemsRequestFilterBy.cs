using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal abstract record SearchStoreItemsRequestFilterBy
{
    public static readonly SearchStoreItemsRequestFilterBy None = new NoFilter();

    private SearchStoreItemsRequestFilterBy() { }

    public static Result<SearchStoreItemsRequestFilterBy> Create(string field, FilterByOperator filterByOperator, string value)
    {
        if (!string.Equals(field, "code", StringComparison.OrdinalIgnoreCase))
        {
            return SearchStoreItemsRequestFilterByErrors.FieldNotSupported;
        }

        var code = Code.Create(value);
        if (code.IsFailure)
        {
            return code.Error;
        }

        return ByCode.Create(code, filterByOperator);
    }

    internal sealed record NoFilter : SearchStoreItemsRequestFilterBy;

    internal sealed record ByCode : SearchStoreItemsRequestFilterBy
    {
        public Code Code { get; }

        public FilterByOperator Operator { get; }

        public static Result<SearchStoreItemsRequestFilterBy> Create(Code code, FilterByOperator filterByOperator) =>
            filterByOperator switch
            {
                FilterByOperator.Equal or FilterByOperator.NotEqual => new ByCode(code, filterByOperator),
                _ => SearchStoreItemsRequestFilterByErrors.OperatorNotSupported
            };

        public static SearchStoreItemsRequestFilterBy Valid(Code code, FilterByOperator filterByOperator) =>
            new ByCode(code, filterByOperator);

        private ByCode(Code code, FilterByOperator filterByOperator) =>
            (Code, Operator) = (code, filterByOperator);
    }
}

internal static class SearchStoreItemsRequestFilterByErrors
{
    public const string CodeOperatorNotSupported = "search-store-items-request-filter-by-operator-not-supported";

    public const string CodeFieldNotSupported = "search-store-items-request-filter-by-field-not-supported";
    public const string CodeInvalid = "search-store-items-request-filter-by-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Filter is invalid.");

    public static ResultError OperatorNotSupported => new(CodeOperatorNotSupported, "Operator not supported.");

    public static ResultError FieldNotSupported => new(CodeFieldNotSupported, "Filter field not supported.");
}
