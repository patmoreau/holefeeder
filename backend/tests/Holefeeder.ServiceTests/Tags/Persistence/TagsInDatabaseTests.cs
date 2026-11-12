using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.ListTags;
using Holefeeder.Service.Tags.Persistence;
using Holefeeder.ServiceTests.Tags.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Tags.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class TagsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly TagsInDatabase _repository = new(database.DataSource, new LoggerForTest<TagsInDatabase>(testOutputHelper));

    [Fact]
    public async Task ListAsync_CountsTheTransactionsAndActiveCashflowsOfEachTagByName()
    {
        var user = await AUser().SavedInDbAsync(database);
        await database.TaggedTransactionInDbAsync(user, "groceries,costco");
        await database.TaggedTransactionInDbAsync(user, "groceries");
        await database.TaggedTransactionInDbAsync(user, "home");
        await database.TaggedCashflowInDbAsync(user, "home,rent");
        await database.TaggedCashflowInDbAsync(user, "rent");
        await database.TaggedCashflowInDbAsync(user, "rent,old", inactive: true);
        await database.TaggedTransactionInDbAsync(user, null);
        await database.TaggedTransactionInDbAsync(user, "");
        await database.TaggedTransactionInDbAsync(await AUser().SavedInDbAsync(database), "someone-else");

        var result = await _repository.ListAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(
            new TagSummary("costco", 1, 0),
            new TagSummary("groceries", 2, 0),
            new TagSummary("home", 1, 1),
            new TagSummary("rent", 0, 2));
    }

    [Fact]
    public async Task ListAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new TagsInDatabase(database.BadDataSource, new LoggerForTest<TagsInDatabase>(testOutputHelper));

        var result = await repository.ListAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TagsErrors.ReadFailed);
    }

    [Fact]
    public async Task RenameAsync_RenamesTheExactTagOnTheUsersTransactionsAndCashflows()
    {
        var user = await AUser().SavedInDbAsync(database);
        var transaction = await database.TaggedTransactionInDbAsync(user, "home,car,rent");
        var merged = await database.TaggedTransactionInDbAsync(user, "car,auto");
        var lookalike = await database.TaggedTransactionInDbAsync(user, "carwash");
        var untagged = await database.TaggedTransactionInDbAsync(user, null);
        var cashflow = await database.TaggedCashflowInDbAsync(user, "car");
        var cancelled = await database.TaggedCashflowInDbAsync(user, "car,old", inactive: true);
        var someoneElse = await database.TaggedTransactionInDbAsync(await AUser().SavedInDbAsync(database), "car");

        var result = await _repository.RenameAsync(user, TagName.Valid("car"), TagName.Valid("auto"), CancellationToken.None);

        result.Should().BeSuccessful();
        (await database.TagsOfAsync("transactions", transaction)).Should().Be("home,auto,rent");
        (await database.TagsOfAsync("transactions", merged)).Should().Be("auto");
        (await database.TagsOfAsync("transactions", lookalike)).Should().Be("carwash");
        (await database.TagsOfAsync("transactions", untagged)).Should().BeNull();
        (await database.TagsOfAsync("cashflows", cashflow)).Should().Be("auto");
        (await database.TagsOfAsync("cashflows", cancelled)).Should().Be("auto,old");
        (await database.TagsOfAsync("transactions", someoneElse)).Should().Be("car");
    }

    [Fact]
    public async Task RenameAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new TagsInDatabase(database.BadDataSource, new LoggerForTest<TagsInDatabase>(testOutputHelper));

        var result = await repository.RenameAsync(AUser(), TagName.Valid("car"), TagName.Valid("auto"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TagsErrors.WriteFailed);
    }
}
