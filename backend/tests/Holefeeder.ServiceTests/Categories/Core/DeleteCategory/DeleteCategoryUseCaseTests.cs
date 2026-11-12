using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.DeleteCategory;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Categories.TestKit;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Core.DeleteCategory;

public class DeleteCategoryUseCaseTests
{
    private readonly CategoriesInMemory _categories = new();
    private readonly DeleteCategoryUseCase _useCase;

    public DeleteCategoryUseCaseTests() => _useCase = new DeleteCategoryUseCase(_categories);

    [Fact]
    public async Task HandleAsync_DeactivatesTheCategory_WhenTheUserOwnsIt()
    {
        var user = AUser();
        var dining = ACategory("Dining");
        var groceries = ACategory("Groceries");
        _categories.Add(user, dining);
        _categories.Add(user, groceries);

        var result = await _useCase.HandleAsync(new DeleteCategoryRequest(user, dining.Id), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().Equal(groceries);
        (await _categories.IsNameUsedAsync(user, dining.Name, AnId(), CancellationToken.None)).Value.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_FailsWithSystem_WhenTheCategoryIsASystemOne()
    {
        var user = AUser();
        var transferOut = ACategory("Transfer Out", system: true);
        _categories.Add(user, transferOut);

        var result = await _useCase.HandleAsync(new DeleteCategoryRequest(user, transferOut.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.System);
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().Equal(transferOut);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheCategoryBelongsToAnotherUser()
    {
        var owner = AUser();
        var dining = ACategory("Dining");
        _categories.Add(owner, dining);

        var result = await _useCase.HandleAsync(new DeleteCategoryRequest(AUser(), dining.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
        (await _categories.ListAsync(owner, CancellationToken.None)).Value.Should().Equal(dining);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheCategoryIsAlreadyDeleted()
    {
        var user = AUser();
        var dining = ACategory("Dining");
        _categories.Add(user, dining, inactive: true);

        var result = await _useCase.HandleAsync(new DeleteCategoryRequest(user, dining.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenCategoriesCannotBeRead()
    {
        var failure = new ResultError("categories-unavailable", "Categories are unavailable.");
        _categories.FailWith(failure);

        var result = await _useCase.HandleAsync(new DeleteCategoryRequest(AUser(), AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
