using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests.Steps;

[Binding]
public sealed class SecurityStepDefinitions
{
    private readonly HttpClientDriver _httpClientDriver;
    private readonly AuthenticationSystemDriver _authenticationSystemDriver;

    public SecurityStepDefinitions(HttpClientDriver httpClientDriver, AuthenticationSystemDriver authenticationSystemDriver)
    {
        _httpClientDriver = httpClientDriver;
        _authenticationSystemDriver = authenticationSystemDriver;
    }

    [Given(@"I am ((?>not )?authorized)")]
    public async Task GivenAuthorizationSet(bool isAuthorized)
    {
        if (isAuthorized)
        {
            _authenticationSystemDriver.SetupValidToken();
            await _httpClientDriver.Authenticate();
        }
    }

    [StepArgumentTransformation(@"((?>not )?authorized)")]
    public static bool HttpMethodTransform(string text) => text == "authorized";
}
