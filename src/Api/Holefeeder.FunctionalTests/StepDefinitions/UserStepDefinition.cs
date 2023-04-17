using Holefeeder.Tests.Common.SeedWork.Drivers;
using Holefeeder.Tests.Common.SeedWork.Scenarios;
using Holefeeder.Tests.Common.SeedWork.StepDefinitions;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UserStepDefinition : RootStepDefinition
{
    public static readonly Guid HolefeederUserId = Guid.NewGuid();

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
        runner.Execute("the user is authorized", () => HttpClientDriver.AuthenticateUser(HolefeederUserId));
    }

    public void IsForbidden(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is unauthorized", () => HttpClientDriver.AuthenticateUser(Guid.NewGuid()));
    }
}
