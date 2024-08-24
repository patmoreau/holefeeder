using DrifterApps.Seeds.Testing.Scenarios;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features;

public abstract class SecurityScenario : Scenario
{
    protected override Task ResetStateAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected SecurityScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : base(
        apiApplicationDriver, testOutputHelper)
    {
        User = new UserStepDefinition(HttpClientDriver);
    }

    internal UserStepDefinition User { get; }
}
