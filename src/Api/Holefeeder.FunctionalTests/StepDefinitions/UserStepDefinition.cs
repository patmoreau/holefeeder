using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UserStepDefinition
{
    private HttpClientDriver HttpClientDriver { get; }

    public UserStepDefinition(HttpClientDriver httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }

    public void IsUnauthorized()
    {
        HttpClientDriver.UnAuthenticate();
    }

    public void IsAuthorized()
    {
        HttpClientDriver.Authenticate();
    }

    public void IsForbidden()
    {
        HttpClientDriver.AuthenticateUser(Guid.NewGuid());
    }
}
