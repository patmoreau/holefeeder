using Holefeeder.Service.Support.Persistence;
using Holefeeder.ServiceTests.Support.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Support.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class UsersInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly UsersInDatabase _repository = new(database.DataSource, new LoggerForTest<UsersInDatabase>(testOutputHelper));

    [Fact]
    public async Task GetByIdentityObjectId_Succeed_WhenFound()
    {
        var user = await AUser().SavedInDbAsync(database);
        var result = await _repository.GetByIdentityObjectId(user.IdentityObjectId, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(user);
    }

    [Fact]
    public async Task GetByIdentityObjectId_Fails_WhenNotFound()
    {
        var result = await _repository.GetByIdentityObjectId(AnIdentityObjectId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(UsersErrors.NotFound);
    }

    [Fact]
    public async Task GetByIdentityObjectId_Fails_WhenDatabaseError()
    {
        var repo = new UsersInDatabase(database.BadDataSource, new LoggerForTest<UsersInDatabase>(testOutputHelper));

        var result = await repo.GetByIdentityObjectId(AnIdentityObjectId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(UsersErrors.ReadFailed);
    }
}
