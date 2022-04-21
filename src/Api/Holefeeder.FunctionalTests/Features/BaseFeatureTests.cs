using System.Net;

using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.FunctionalTests.Features;

public abstract class BaseFeatureTests
{
    protected HttpClientDriver HttpClientDriver { get; }

    protected BaseFeatureTests(HttpClientDriver httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }

    protected void GivenUserIsUnauthorized()
    {
        HttpClientDriver.UnAuthenticate();
    }

    protected void GivenUserIsAuthorized()
    {
        HttpClientDriver.Authenticate();
    }

    protected void GivenForbiddenUserIsAuthorized()
    {
        HttpClientDriver.AuthenticateUser(Guid.NewGuid().ToString());
    }

    protected void ThenShouldNotHaveInternalServerError()
    {
        HttpClientDriver.ShouldHaveResponseWithStatus(statusCode => statusCode != HttpStatusCode.InternalServerError);
    }

    protected void ThenUserShouldBeAuthorizedToAccessEndpoint()
    {
        CheckAuthorizationStatus(true);
    }

    protected void ThenUserShouldBeForbiddenToAccessEndpoint()
    {
        CheckAuthorizationStatus(false);
    }

    protected void ThenUserShouldNotBeAuthorizedToAccessEndpoint()
    {
        CheckAuthorizationStatus(false);
    }

    protected void ThenShouldExpectStatusCode(HttpStatusCode expectedStatusCode)
    {
        HttpClientDriver.ShouldHaveResponseWithStatus(expectedStatusCode);
    }

    protected async Task ThenShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage)
    {
        ThenShouldExpectStatusCode(HttpStatusCode.UnprocessableEntity);

        var problemDetails = await HttpClientDriver.DeserializeContent<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails?.Title.Should().Be(errorMessage);
    }

    private void CheckAuthorizationStatus(bool isAuthorized)
    {
        bool IsExpectedStatus(HttpStatusCode? statusCode) => isAuthorized
            ? statusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
            : statusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized;

        HttpClientDriver.ShouldHaveResponseWithStatus(IsExpectedStatus);
    }
}
