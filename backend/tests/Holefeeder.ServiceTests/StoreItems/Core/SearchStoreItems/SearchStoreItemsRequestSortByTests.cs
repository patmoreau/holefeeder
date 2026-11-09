using Bogus;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

namespace Holefeeder.ServiceTests.StoreItems.Core.SearchStoreItems;

public class SearchStoreItemsRequestSortByTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_Succeeds_WhenFieldIsValid()
    {
        var direction = _faker.PickRandom(SortByDirection.Ascending, SortByDirection.Descending);
        var sortBy = SearchStoreItemsRequestSortBy.Create("code", direction);

        sortBy.Should().BeSuccessful().And.WithValue(SearchStoreItemsRequestSortBy.Valid("code", direction));
    }

    [Fact]
    public void Create_NormalizesFieldToLowerCase_WhenFieldIsValidInAnotherCase()
    {
        var direction = _faker.PickRandom(SortByDirection.Ascending, SortByDirection.Descending);
        var sortBy = SearchStoreItemsRequestSortBy.Create("CODE", direction);

        sortBy.Should().BeSuccessful().And.WithValue(SearchStoreItemsRequestSortBy.Valid("code", direction));
    }

    [Fact]
    public void Create_Fails_WhenFieldIsUnknown()
    {
        var field = _faker.Random.AlphaNumeric(10);
        var direction = _faker.PickRandom(SortByDirection.Ascending, SortByDirection.Descending);
        var sortBy = SearchStoreItemsRequestSortBy.Create(field, direction);

        sortBy.Should().BeFailure().And.WithError(SearchStoreItemsRequestSortByErrors.Invalid);
    }

    [Fact]
    public void Invalid_IsNamedAfterTheSortParameter()
    {
        SearchStoreItemsRequestSortByErrors.Invalid.Code.Should().Be("search-store-items-request-sort-by-invalid");
        SearchStoreItemsRequestSortByErrors.Invalid.Description.Should().Be("Sort is invalid.");
    }

    [Fact]
    public void Create_ReturnsDefault_WhenTextIsMissing()
    {
        var result = SearchStoreItemsRequestSortBy.Create(null);

        result.Should().BeSuccessful().And.WithValue(SearchStoreItemsRequestSortBy.Default);
    }

    [Theory]
    [InlineData("code", "Ascending")]
    [InlineData("-code", "Descending")]
    [InlineData("CODE", "Ascending")]
    public void Create_Succeeds_WhenTextSortsOnAKnownField(string value, string direction)
    {
        var result = SearchStoreItemsRequestSortBy.Create(value);

        result.Should().BeSuccessful().And.WithValue(
            SearchStoreItemsRequestSortBy.Valid("code", Enum.Parse<SortByDirection>(direction)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("--code")]
    [InlineData("code:asc")]
    [InlineData("name")]
    public void Create_Fails_WhenTextIsNotAValidSort(string value)
    {
        var result = SearchStoreItemsRequestSortBy.Create(value);

        result.Should().BeFailure().And.WithError(SearchStoreItemsRequestSortByErrors.Invalid);
    }
}
