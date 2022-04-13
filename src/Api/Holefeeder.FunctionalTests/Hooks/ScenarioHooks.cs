using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests.Hooks;

[Binding]
public sealed class ScenarioHooks
{
    private readonly AuthenticationSystemDriver _authenticationSystemDriver;
    private readonly DatabaseDriver _databaseDriver;

    public ScenarioHooks(AuthenticationSystemDriver authenticationSystemDriver, DatabaseDriver databaseDriver)
    {
        _authenticationSystemDriver = authenticationSystemDriver;
        _databaseDriver = databaseDriver;
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        _authenticationSystemDriver.Start();
        await _databaseDriver.ResetState();
    }

    [AfterScenario]
    public Task AfterScenario()
    {
        _authenticationSystemDriver.Stop();
        return Task.CompletedTask;
    }
}

