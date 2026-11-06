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
}
