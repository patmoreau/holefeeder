using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core.ListCategories;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Categories.TestKit;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Core.ListCategories;

public class ListCategoriesUseCaseTests
{
    private readonly CategoriesInMemory _categories = new();
    private readonly ListCategoriesUseCase _useCase;

    public ListCategoriesUseCaseTests() => _useCase = new ListCategoriesUseCase(_categories);

    [Fact]
    public async Task HandleAsync_ListsTheUsersCategoriesByTypeThenFavoriteThenName()
    {
        var user = AUser();
        var salary = ACategory("Salary", CategoryType.Gain);
        var dining = ACategory("Dining");
        var housing = ACategory("Housing", favorite: true);
        var bonus = ACategory("Bonus", CategoryType.Gain);
        _categories.Add(user, salary);
        _categories.Add(user, dining);
        _categories.Add(user, housing);
        _categories.Add(user, bonus);
        _categories.Add(AUser(), ACategory("Someone else's"));

        var result = await _useCase.HandleAsync(new ListCategoriesRequest(user), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(housing, dining, bonus, salary);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenCategoriesCannotBeRead()
    {
        var failure = new ResultError("categories-unavailable", "Categories are unavailable.");
        _categories.FailWith(failure);

        var result = await _useCase.HandleAsync(new ListCategoriesRequest(AUser()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
