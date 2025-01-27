using DrifterApps.Seeds.Testing.Drivers;

using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests;

[CollectionDefinition(Name)]
public sealed class FunctionalTestMarker : ICollectionFixture<ApiApplicationDriver>
{
    public const string Name = "Api collection";
}

[CollectionDefinition(Name)]
public sealed class FunctionalSecurityTestMarker : ICollectionFixture<ApiApplicationSecurityDriver>
{
    public const string Name = "Api Security collection";
}
