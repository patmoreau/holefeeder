using DrifterApps.Seeds.FluentScenario;

namespace Holefeeder.FunctionalTests.Infrastructure;

public class ScenarioOutput : IScenarioOutput
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ScenarioOutput(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public void WriteLine(string message) => _testOutputHelper.WriteLine(message);

    public void WriteLine(string format, params object[] args) => _testOutputHelper.WriteLine(format, args);
}
