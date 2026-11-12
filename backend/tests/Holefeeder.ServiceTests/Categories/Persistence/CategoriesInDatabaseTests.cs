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
}
