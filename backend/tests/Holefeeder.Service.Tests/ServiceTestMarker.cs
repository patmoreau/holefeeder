using Holefeeder.Service.Tests.Drivers;

namespace Holefeeder.Service.Tests;

[CollectionDefinition(Name)]
public sealed class ServiceTestMarker : ICollectionFixture<ServiceDriver>
{
    public const string Name = "Service collection";
}
