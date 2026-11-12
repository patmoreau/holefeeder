using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.CreateCategory;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Categories.TestKit;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Core.CreateCategory;

public class CreateCategoryUseCaseTests
{
    private readonly CategoriesInMemory _categories = new();
    private readonly CreateCategoryUseCase _useCase;

    public CreateCategoryUseCaseTests() => _useCase = new CreateCategoryUseCase(_categories);

    [Fact]
    public async Task HandleAsync_CreatesAnActiveUserCategory()
    {
        var user = AUser();

        var result = await _useCase.HandleAsync(
            new CreateCategoryRequest(user, CategoryName.Valid("Dining"), CategoryType.Expense, CategoryColor.Valid("#FF9500"),
                Money.Valid(200m), Favorite: true),
            CancellationToken.None);

        result.Should().BeSuccessful();
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().Equal(
            new Category(result.Value, CategoryName.Valid("Dining"), CategoryType.Expense, CategoryColor.Valid("#FF9500"),
                Money.Valid(200m), Favorite: true, System: false));
    }

    [Fact]
    public async Task HandleAsync_CreatesTheCategory_WhenAnotherUserHasTheName()
    {
        _categories.Add(AUser(), ACategory("Dining"));

        var result = await _useCase.HandleAsync(ACreateCategoryRequest(AUser(), "Dining"), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_FailsWithNameTaken_WhenTheUserHasTheName()
    {
        var user = AUser();
        _categories.Add(user, ACategory("Dining"));

        var result = await _useCase.HandleAsync(ACreateCategoryRequest(user, "Dining"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NameTaken);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNameTaken_WhenTheUserHasTheNameInAnotherCase()
    {
        var user = AUser();
        _categories.Add(user, ACategory("Dining"));

        var result = await _useCase.HandleAsync(ACreateCategoryRequest(user, "DINING"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NameTaken);
        (await _categories.ListAsync(user, CancellationToken.None)).Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNameTaken_WhenADeletedCategoryHasTheName()
    {
        var user = AUser();
        _categories.Add(user, ACategory("Dining"), inactive: true);

        var result = await _useCase.HandleAsync(ACreateCategoryRequest(user, "Dining"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NameTaken);
    }

    [Fact]
    public async Task HandleAsync_FailsWithUserNotRegistered_WhenThereIsNoUser()
    {
        var result = await _useCase.HandleAsync(ACreateCategoryRequest(User.NoUser), CancellationToken.None);

        result.Should().BeFailure().And.WithError(UserErrors.NotRegistered);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenCategoriesCannotBeWritten()
    {
        var failure = new ResultError("categories-unavailable", "Categories are unavailable.");
        _categories.FailWith(failure);

        var result = await _useCase.HandleAsync(ACreateCategoryRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }

    [Fact]
    public void NameTaken_DescribesTheUniqueNameRule()
    {
        CategoryErrors.NameTaken.Code.Should().Be("category-name-taken");
        CategoryErrors.NameTaken.Description.Should().Be("Category name is already used.");
    }

    private static CreateCategoryRequest ACreateCategoryRequest(User? user = null, string? name = null) =>
        new(user ?? AUser(), CategoryName.Valid(name ?? $"category-{Guid.NewGuid()}"), CategoryType.Expense,
            CategoryColor.Valid("#34C759"), Money.Zero, Favorite: false);
}
