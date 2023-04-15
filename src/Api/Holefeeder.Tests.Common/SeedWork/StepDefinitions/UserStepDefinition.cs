using Holefeeder.Tests.Common.SeedWork.Drivers;
using Holefeeder.Tests.Common.SeedWork.Scenarios;

namespace Holefeeder.Tests.Common.SeedWork.StepDefinitions;

public class UserStepDefinition : RootStepDefinition
{
    public UserStepDefinition(HttpClientDriver httpClientDriver) : base(httpClientDriver)
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
        runner.Execute("the user is authorized", () => HttpClientDriver.Authenticate());
    }

    public void IsForbidden(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is unauthorized", () => HttpClientDriver.AuthenticateUser(Guid.NewGuid()));
    }
}
