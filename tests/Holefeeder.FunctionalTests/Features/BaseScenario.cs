using DrifterApps.Seeds.FluentScenario;

using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features;

public class BaseScenario(ITestOutputHelper testOutputHelper)
{
    protected IScenarioOutput ScenarioOutput { get; } = new ScenarioOutput(testOutputHelper);
}
