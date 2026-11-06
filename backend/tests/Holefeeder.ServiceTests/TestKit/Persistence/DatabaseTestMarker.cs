namespace Holefeeder.ServiceTests.TestKit.Persistence;

[CollectionDefinition(Name)]
public sealed class DatabaseTestMarker : ICollectionFixture<DatabaseForTest>
{
    public const string Name = "Database test collection";
}
