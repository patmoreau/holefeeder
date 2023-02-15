using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests;

[CollectionDefinition("Api collection")]
public sealed class FunctionalTestMarker : ICollectionFixture<ApiApplicationDriver>,
    ICollectionFixture<BudgetingDatabaseInitializer>
{
}
