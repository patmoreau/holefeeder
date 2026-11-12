using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Users.Core;
using Holefeeder.Service.Users.Persistence;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Users.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class RegisteredUsersInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly RegisteredUsersInDatabase _repository =
        new(database.DataSource, new LoggerForTest<RegisteredUsersInDatabase>(testOutputHelper));

    [Fact]
    public async Task FindIdAsync_ReturnsTheUserId_WhenTheIdentityIsRegistered()
    {
        var user = await AUser().SavedInDbAsync(database);
        await AUser().SavedInDbAsync(database);

        var result = await _repository.FindIdAsync(user.IdentityObjectId, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(user.Id);
    }

    [Fact]
    public async Task FindIdAsync_FailsWithNotFound_WhenTheIdentityIsNotRegistered()
    {
        var result = await _repository.FindIdAsync(AnIdentityObjectId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(RegisteredUserErrors.NotFound);
    }

    [Fact]
    public async Task FindIdAsync_FailsWithReadFailed_WhenTheDatabaseIsUnreachable()
    {
        var repository = new RegisteredUsersInDatabase(
            database.BadDataSource, new LoggerForTest<RegisteredUsersInDatabase>(testOutputHelper));

        var result = await repository.FindIdAsync(AnIdentityObjectId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(RegisteredUsersDatabaseErrors.ReadFailed);
    }

    [Fact]
    public void ReadFailed_IsAnUnexpectedError() =>
        RegisteredUsersDatabaseErrors.ReadFailed.Should().BeOfType<UnexpectedError>();

    [Fact]
    public void NotFound_IsANotFoundError() =>
        RegisteredUserErrors.NotFound.Should().BeOfType<NotFoundError>();
}
