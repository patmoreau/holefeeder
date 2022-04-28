using Holefeeder.FunctionalTests.Drivers;

using Xunit;

namespace Holefeeder.FunctionalTests;

[CollectionDefinition("Api collection")]
public sealed class FunctionalTestMarker : ICollectionFixture<ApiApplicationDriver>
{
}
