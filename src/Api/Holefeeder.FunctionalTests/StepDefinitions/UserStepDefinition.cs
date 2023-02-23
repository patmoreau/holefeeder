using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UserStepDefinition
{
    public UserStepDefinition(HttpClientDriver httpClientDriver) => HttpClientDriver = httpClientDriver;

    private HttpClientDriver HttpClientDriver { get; }

    public void IsUnauthorized() => HttpClientDriver.UnAuthenticate();

    public void IsAuthorized() => HttpClientDriver.Authenticate();

    public void IsForbidden() => HttpClientDriver.AuthenticateUser(Guid.NewGuid());
}
