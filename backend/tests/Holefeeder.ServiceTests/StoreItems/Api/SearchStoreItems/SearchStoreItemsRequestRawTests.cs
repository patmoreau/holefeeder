using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Api.SearchStoreItems;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

namespace Holefeeder.ServiceTests.StoreItems.Api.SearchStoreItems;

public class SearchStoreItemsRequestRawTests
{
    [Fact]
    public void ToSearchStoreItemsRequest_UsesDefaults_WhenNoParameterIsGiven()
    {
        var user = AUser();

        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, null, null), user);

        result.Should().BeSuccessful().And.WithValue(new SearchStoreItemsRequest(
            user, Offset.Default, Limit.Default, SearchStoreItemsRequestFilterBy.None, SearchStoreItemsRequestSortBy.Default));
    }

    [Fact]
    public void ToSearchStoreItemsRequest_UsesDefaultSort_WhenSortIsCodeInUpperCase()
    {
        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, "CODE", null), AUser());

        result.Should().BeSuccessful();
        result.Value.SortBy.Should().Be(SearchStoreItemsRequestSortBy.Default);
    }

    [Fact]
    public void ToSearchStoreItemsRequest_FiltersByCode_WhenFilterIsValid()
    {
        var user = AUser();

        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, null, "code:ne:settings"), user);

        result.Should().BeSuccessful();
        result.Value.FilterBy.Should().Be(
            SearchStoreItemsRequestFilterBy.ByCode.Valid(Code.Valid("settings"), FilterByOperator.NotEqual));
    }

    [Theory]
    [InlineData("code:lt:settings")]
    [InlineData("code:le:settings")]
    [InlineData("code:gt:settings")]
    [InlineData("code:ge:settings")]
    public void ToSearchStoreItemsRequest_FailsValidation_WhenFilterOperatorIsNotSupported(string filter)
    {
        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, null, filter), AUser());

        ValidationErrorsOf(result).Should().ContainKey(SearchStoreItemsRequestFilterByErrors.CodeOperatorNotSupported);
    }

    [Fact]
    public void ToSearchStoreItemsRequest_FailsValidation_WhenFilterFieldIsNotSupported()
    {
        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, null, "name:eq:settings"), AUser());

        ValidationErrorsOf(result).Should().ContainKey(SearchStoreItemsRequestFilterByErrors.CodeFieldNotSupported);
    }

    [Theory]
    [InlineData("garbage")]
    [InlineData("code:xx:settings")]
    public void ToSearchStoreItemsRequest_FailsValidation_WhenFilterIsMalformed(string filter)
    {
        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, null, filter), AUser());

        ValidationErrorsOf(result).Should().ContainSingle()
            .Which.Key.Should().Be(SearchStoreItemsRequestFilterByErrors.CodeInvalid);
    }

    [Fact]
    public void ToSearchStoreItemsRequest_FailsValidation_WhenFilterValueIsNotAValidCode()
    {
        var filter = $"code:eq:{new string('x', 101)}";

        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new(null, null, null, filter), AUser());

        ValidationErrorsOf(result).Should().ContainKey(CodeErrors.CodeInvalid);
    }

    [Fact]
    public void ToSearchStoreItemsRequest_ReportsEveryInvalidParameter()
    {
        var result = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(new("-1", null, null, "code:lt:settings"), AUser());

        ValidationErrorsOf(result).Keys.Should().BeEquivalentTo(
            OffsetErrors.CodeInvalid, SearchStoreItemsRequestFilterByErrors.CodeOperatorNotSupported);
    }

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<SearchStoreItemsRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
