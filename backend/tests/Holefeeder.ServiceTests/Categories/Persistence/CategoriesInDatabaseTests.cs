using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class CategoriesInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly CategoriesInDatabase _repository = new(database.DataSource, new LoggerForTest<CategoriesInDatabase>(testOutputHelper));

    [Fact]
    public async Task ListAsync_ListsTheUsersActiveCategoriesByTypeThenFavoriteThenName()
    {
        var user = await AUser().SavedInDbAsync(database);
        var salary = await ACategory("Salary", CategoryType.Gain, "#30B0C7").SavedInDbAsync(user, database);
        var dining = await ACategory("Dining", budget: 200m).SavedInDbAsync(user, database);
        var housing = await ACategory("Housing", budget: 1450.5m, favorite: true).SavedInDbAsync(user, database);
        var transferIn = await ACategory("Transfer In", CategoryType.Gain, "#607D8B", system: true).SavedInDbAsync(user, database);
        await ACategory("Old").SavedInDbAsync(user, database, inactive: true);
        var otherUser = await AUser().SavedInDbAsync(database);
        await ACategory("Someone else's").SavedInDbAsync(otherUser, database);

        var result = await _repository.ListAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(housing, dining, salary, transferIn);
    }

    [Fact]
    public async Task ListAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CategoriesInDatabase(database.BadDataSource, new LoggerForTest<CategoriesInDatabase>(testOutputHelper));

        var result = await repository.ListAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoriesErrors.ReadFailed);
    }
    [Fact]
    public async Task SaveAsync_InsertsANewCategory()
    {
        var user = await AUser().SavedInDbAsync(database);
        var dining = ACategory("Dining", color: "#FF9500", budget: 200m, favorite: true);

        var result = await _repository.SaveAsync(user, dining, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.ListAsync(user, CancellationToken.None)).Value.Should().Equal(dining);
    }

    [Fact]
    public async Task SaveAsync_ReplacesTheUsersCategory()
    {
        var user = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining").SavedInDbAsync(user, database);
        var changed = dining with
        {
            Name = CategoryName.Valid("Restaurants"),
            Type = CategoryType.Gain,
            Color = CategoryColor.Valid("#5856D6"),
            Budget = Money.Valid(75.25m),
            Favorite = true
        };

        var result = await _repository.SaveAsync(user, changed, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.ListAsync(user, CancellationToken.None)).Value.Should().Equal(changed);
    }

    [Fact]
    public async Task SaveAsync_FailsWithNotFound_WhenAnotherUserOwnsTheCategory()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining").SavedInDbAsync(owner, database);
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.SaveAsync(user, dining with { Name = CategoryName.Valid("Mine") }, CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
        (await _repository.ListAsync(owner, CancellationToken.None)).Value.Should().Equal(dining);
    }

    [Theory]
    [InlineData("Dining", true)]
    [InlineData(" dining", false)]
    [InlineData("DINING", true)]
    [InlineData("Restaurants", false)]
    public async Task IsNameUsedAsync_ComparesNamesIgnoringCase(string name, bool expected)
    {
        var user = await AUser().SavedInDbAsync(database);
        await ACategory("Dining").SavedInDbAsync(user, database);

        var result = await _repository.IsNameUsedAsync(user, CategoryName.Valid(name), AnId(), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(expected);
    }

    [Fact]
    public async Task IsNameUsedAsync_CountsDeletedCategories()
    {
        var user = await AUser().SavedInDbAsync(database);
        await ACategory("Dining").SavedInDbAsync(user, database, inactive: true);

        var result = await _repository.IsNameUsedAsync(user, CategoryName.Valid("Dining"), AnId(), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(true);
    }

    [Fact]
    public async Task IsNameUsedAsync_IgnoresTheCategoryItself()
    {
        var user = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining").SavedInDbAsync(user, database);

        var result = await _repository.IsNameUsedAsync(user, dining.Name, dining.Id, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(false);
    }

    [Fact]
    public async Task IsNameUsedAsync_IgnoresOtherUsers()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherUser = await AUser().SavedInDbAsync(database);
        await ACategory("Dining").SavedInDbAsync(otherUser, database);

        var result = await _repository.IsNameUsedAsync(user, CategoryName.Valid("Dining"), AnId(), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(false);
    }

    [Fact]
    public async Task IsNameUsedAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CategoriesInDatabase(database.BadDataSource, new LoggerForTest<CategoriesInDatabase>(testOutputHelper));

        var result = await repository.IsNameUsedAsync(AUser(), CategoryName.Valid("Dining"), AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoriesErrors.ReadFailed);
    }

    [Fact]
    public async Task SaveAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CategoriesInDatabase(database.BadDataSource, new LoggerForTest<CategoriesInDatabase>(testOutputHelper));

        var result = await repository.SaveAsync(AUser(), ACategory(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoriesErrors.WriteFailed);
    }
    [Fact]
    public async Task GetAsync_ReturnsTheUsersActiveCategory()
    {
        var user = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining", budget: 200m, favorite: true).SavedInDbAsync(user, database);

        var result = await _repository.GetAsync(user, dining.Id, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(dining);
    }

    [Fact]
    public async Task GetAsync_FailsWithNotFound_WhenTheCategoryIsDeleted()
    {
        var user = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining").SavedInDbAsync(user, database, inactive: true);

        var result = await _repository.GetAsync(user, dining.Id, CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task GetAsync_FailsWithNotFound_WhenAnotherUserOwnsTheCategory()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining").SavedInDbAsync(owner, database);

        var result = await _repository.GetAsync(await AUser().SavedInDbAsync(database), dining.Id, CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task GetAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CategoriesInDatabase(database.BadDataSource, new LoggerForTest<CategoriesInDatabase>(testOutputHelper));

        var result = await repository.GetAsync(AUser(), AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CategoriesErrors.ReadFailed);
    }
    [Fact]
    public async Task SaveAsync_KeepsADeactivatedCategoryOutOfTheList()
    {
        var user = await AUser().SavedInDbAsync(database);
        var dining = await ACategory("Dining").SavedInDbAsync(user, database);

        var result = await _repository.SaveAsync(user, dining with { Inactive = true }, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.ListAsync(user, CancellationToken.None)).Value.Should().BeEmpty();
        (await _repository.GetAsync(user, dining.Id, CancellationToken.None)).Should().BeFailure().And.WithError(CategoryErrors.NotFound);
        (await _repository.IsNameUsedAsync(user, dining.Name, AnId(), CancellationToken.None)).Value.Should().BeTrue();
    }
}
