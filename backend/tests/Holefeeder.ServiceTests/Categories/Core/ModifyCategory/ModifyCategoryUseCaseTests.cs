using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.ModifyCategory;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Categories.TestKit;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Core.ModifyCategory;

public class ModifyCategoryUseCaseTests
{
    private readonly CategoriesInMemory _categories = new();
    private readonly ModifyCategoryUseCase _useCase;

    public ModifyCategoryUseCaseTests() => _useCase = new ModifyCategoryUseCase(_categories);

    [Fact]
    public async Task HandleAsync_ReplacesEveryValue_WhenTheUserOwnsTheCategory()
    {
        var user = AUser();
        var dining = ACategory("Dining");
        _categories.Add(user, dining);

        var result = await _useCase.HandleAsync(
            new ModifyCategoryRequest(user, dining.Id, CategoryName.Valid("Bonus"), CategoryType.Gain, CategoryColor.Valid("#30B0C7"),
                Money.Valid(80m), Favorite: true),
            CancellationToken.None);

        result.Should().BeSuccessful();
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().Equal(
            new Category(dining.Id, CategoryName.Valid("Bonus"), CategoryType.Gain, CategoryColor.Valid("#30B0C7"), Money.Valid(80m),
                Favorite: true, System: false));
    }

    [Fact]
    public async Task HandleAsync_KeepsItsOwnName_InAnotherCase()
    {
        var user = AUser();
        var dining = ACategory("dining");
        _categories.Add(user, dining);

        var result = await _useCase.HandleAsync(AModifyCategoryRequest(user, dining.Id, "Dining"), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_FailsWithNameTaken_WhenAnotherCategoryHasTheName()
    {
        var user = AUser();
        var dining = ACategory("Dining");
        _categories.Add(user, dining);
        _categories.Add(user, ACategory("Restaurants"));

        var result = await _useCase.HandleAsync(AModifyCategoryRequest(user, dining.Id, "restaurants"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NameTaken);
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().Contain(dining);
    }

    [Fact]
    public async Task HandleAsync_FailsWithSystem_WhenTheCategoryIsASystemOne()
    {
        var user = AUser();
        var transferIn = ACategory("Transfer In", CategoryType.Gain, system: true);
        _categories.Add(user, transferIn);

        var result = await _useCase.HandleAsync(AModifyCategoryRequest(user, transferIn.Id, "Money In"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.System);
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().Equal(transferIn);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheCategoryBelongsToAnotherUser()
    {
        var dining = ACategory("Dining");
        _categories.Add(AUser(), dining);

        var result = await _useCase.HandleAsync(AModifyCategoryRequest(AUser(), dining.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheCategoryIsDeleted()
    {
        var user = AUser();
        var dining = ACategory("Dining");
        _categories.Add(user, dining, inactive: true);

        var result = await _useCase.HandleAsync(AModifyCategoryRequest(user, dining.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenCategoriesCannotBeRead()
    {
        var failure = new ResultError("categories-unavailable", "Categories are unavailable.");
        _categories.FailWith(failure);

        var result = await _useCase.HandleAsync(AModifyCategoryRequest(AUser(), AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }

    [Fact]
    public void System_DescribesTheSystemCategoryRule()
    {
        CategoryErrors.System.Code.Should().Be("category-system");
        CategoryErrors.System.Description.Should().Be("System categories cannot be changed.");
    }

    private static ModifyCategoryRequest AModifyCategoryRequest(User user, Id id, string? name = null) =>
        new(user, id, CategoryName.Valid(name ?? $"category-{Guid.NewGuid()}"), CategoryType.Expense, CategoryColor.Valid("#34C759"),
            Money.Zero, Favorite: false);
}
