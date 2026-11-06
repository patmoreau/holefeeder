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
}
