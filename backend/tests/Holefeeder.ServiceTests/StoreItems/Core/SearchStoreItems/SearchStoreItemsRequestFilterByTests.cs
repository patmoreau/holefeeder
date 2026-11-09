using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

namespace Holefeeder.ServiceTests.StoreItems.Core.SearchStoreItems;

public class SearchStoreItemsRequestFilterByTests
{
    [Theory]
    [InlineData("code", "Equal")]
    [InlineData("code", "NotEqual")]
    [InlineData("CODE", "Equal")]
    [InlineData("Code", "NotEqual")]
    public void Create_FiltersByCode_WhenFieldIsCode(string field, string filterByOperator)
    {
        var expectedOperator = Enum.Parse<FilterByOperator>(filterByOperator);

        var result = SearchStoreItemsRequestFilterBy.Create(field, expectedOperator, "settings");

        result.Should().BeSuccessful().And.WithValue(
            SearchStoreItemsRequestFilterBy.ByCode.Valid(Code.Valid("settings"), expectedOperator));
    }

    [Theory]
    [InlineData("name")]
    [InlineData("data")]
    [InlineData("codes")]
    public void Create_Fails_WhenFieldIsNotSupported(string field)
    {
        var result = SearchStoreItemsRequestFilterBy.Create(field, FilterByOperator.Equal, "settings");

        result.Should().BeFailure().And.WithError(SearchStoreItemsRequestFilterByErrors.FieldNotSupported);
    }

    [Theory]
    [InlineData("LessThan")]
    [InlineData("LessThanOrEqual")]
    [InlineData("GreaterThan")]
    [InlineData("GreaterThanOrEqual")]
    public void Create_Fails_WhenOperatorIsNotSupported(string filterByOperator)
    {
        var result = SearchStoreItemsRequestFilterBy.Create("code", Enum.Parse<FilterByOperator>(filterByOperator), "settings");

        result.Should().BeFailure().And.WithError(SearchStoreItemsRequestFilterByErrors.OperatorNotSupported);
    }

    [Fact]
    public void Create_Fails_WhenValueIsNotAValidCode()
    {
        var result = SearchStoreItemsRequestFilterBy.Create("code", FilterByOperator.Equal, new string('x', 101));

        result.Should().BeFailure().And.WithError(CodeErrors.Invalid);
    }

    [Fact]
    public void Errors_DescribeWhatIsNotSupported()
    {
        SearchStoreItemsRequestFilterByErrors.OperatorNotSupported.Description.Should().Be("Filter operator is not supported.");
        SearchStoreItemsRequestFilterByErrors.FieldNotSupported.Description.Should().Be("Filter field is not supported.");
    }

    [Fact]
    public void Create_ReturnsNone_WhenTextIsMissing()
    {
        var result = SearchStoreItemsRequestFilterBy.Create(null);

        result.Should().BeSuccessful().And.WithValue(SearchStoreItemsRequestFilterBy.None);
    }

    [Theory]
    [InlineData("code:eq:settings", "Equal")]
    [InlineData("code:ne:settings", "NotEqual")]
    public void Create_FiltersByCode_WhenTextFiltersOnCode(string value, string filterByOperator)
    {
        var result = SearchStoreItemsRequestFilterBy.Create(value);

        result.Should().BeSuccessful().And.WithValue(SearchStoreItemsRequestFilterBy.ByCode.Valid(
            Code.Valid("settings"), Enum.Parse<FilterByOperator>(filterByOperator)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("code")]
    [InlineData("code:eq:")]
    [InlineData("code:xx:settings")]
    public void Create_Fails_WhenTextIsMalformed(string value)
    {
        var result = SearchStoreItemsRequestFilterBy.Create(value);

        result.Should().BeFailure().And.WithError(SearchStoreItemsRequestFilterByErrors.Invalid);
    }

    [Fact]
    public void Create_Fails_WhenTextUsesAnUnsupportedOperator()
    {
        var result = SearchStoreItemsRequestFilterBy.Create("code:lt:settings");

        result.Should().BeFailure().And.WithError(SearchStoreItemsRequestFilterByErrors.OperatorNotSupported);
    }

    [Fact]
    public void Create_Fails_WhenTextUsesAnUnsupportedField()
    {
        var result = SearchStoreItemsRequestFilterBy.Create("name:eq:settings");

        result.Should().BeFailure().And.WithError(SearchStoreItemsRequestFilterByErrors.FieldNotSupported);
    }
}
