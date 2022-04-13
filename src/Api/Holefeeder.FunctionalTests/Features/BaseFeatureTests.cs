using System.Net;

using Holefeeder.FunctionalTests.Drivers;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.FunctionalTests.Features;

public abstract class BaseFeatureTests
{
    protected HttpClientDriver HttpClientDriver { get; }

    protected BaseFeatureTests(HttpClientDriver httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }

    protected async Task GivenAuthorizationSet(bool isAuthorized)
    {
        if (isAuthorized)
        {
            await HttpClientDriver.Authenticate();
        }
    }

    protected void ThenShouldHaveProperAuthorization(bool isAuthorized)
    {
        bool IsExpectedStatus(HttpStatusCode? statusCode) => isAuthorized
            ? statusCode != HttpStatusCode.Unauthorized
            : statusCode == HttpStatusCode.Unauthorized;

        HttpClientDriver.ShouldHaveResponseWithStatus(IsExpectedStatus);
    }
}
