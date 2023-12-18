using Holefeeder.FunctionalTests.Drivers;
using LightBDD.XUnit2;

[assembly: LightBddScope]

namespace Holefeeder.FunctionalTests;

[CollectionDefinition("Api collection")]
public sealed class FunctionalTestMarker : ICollectionFixture<ApiApplicationDriver>;
