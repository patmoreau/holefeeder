using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests;

[CollectionDefinition("Api collection")]
public sealed class FunctionalTestMarker : ICollectionFixture<ApiApplicationDriver>;

[CollectionDefinition("Api Security collection")]
public sealed class FunctionalSecurityTestMarker : ICollectionFixture<ApiApplicationSecurityDriver>;
