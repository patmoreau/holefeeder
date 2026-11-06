using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.TestKit.Api;

[CollectionDefinition(Name)]
public sealed class ComponentTestMarker : ICollectionFixture<ServerForTest>
{
    public const string Name = "Service collection";
}
