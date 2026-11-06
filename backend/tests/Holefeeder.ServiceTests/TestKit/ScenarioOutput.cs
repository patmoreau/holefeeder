using DrifterApps.Seeds.FluentScenario;

namespace Holefeeder.ServiceTests.TestKit;

public class ScenarioOutput(ITestOutputHelper testOutputHelper) : IScenarioOutput
{
    public void WriteLine(string message) => testOutputHelper.WriteLine(message);

    public void WriteLine(string format, params object[] args) => testOutputHelper.WriteLine(format, args);
}
