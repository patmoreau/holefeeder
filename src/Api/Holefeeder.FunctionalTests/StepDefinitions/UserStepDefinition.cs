using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UserStepDefinition : StepDefinition
{
    public static readonly Guid HolefeederUserId = Fakerizer.Random.Guid();

    public UserStepDefinition(IHttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
    }

    public void IsUnauthorized(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is unauthorized", () => HttpClientDriver.UnAuthenticate());
    }

    public void IsAuthorized(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is authorized", () => HttpClientDriver.AuthenticateUser(HolefeederUserId.ToString()));
    }

    public void IsForbidden(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is unauthorized",
            () => HttpClientDriver.AuthenticateUser(Fakerizer.Random.Hash()));
    }
}
