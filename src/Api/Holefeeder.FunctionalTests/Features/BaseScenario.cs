using System.Net;
using System.Text;

using FluentAssertions;
using FluentAssertions.Execution;

using Holefeeder.Application.SeedWork;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api collection")]
public abstract class BaseScenario
{
    protected HttpClientDriver HttpClientDriver { get; }

    protected BaseScenario(ApiApplicationDriver apiApplicationDriver)
    {
        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver();
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
        HttpClientDriver.AuthenticateUser(Guid.NewGuid());
    }

    protected Task WhenUserTriesToQuery(ApiResources apiResource, int? offset = null, int? limit = null,
        string? sorts = null, string? filters = null)
    {
        var sb = new StringBuilder();
        if (offset is not null)
        {
            sb.Append($"offset={offset}&");
        }

        if (limit is not null)
        {
            sb.Append($"limit={limit}&");
        }

        if (!string.IsNullOrWhiteSpace(sorts))
        {
            foreach (var sort in sorts.Split(';'))
            {
                sb.Append($"sort={sort}&");
            }
        }

        if (!string.IsNullOrWhiteSpace(filters))
        {
            foreach (var filter in filters.Split(';'))
            {
                sb.Append($"filter={filter}&");
            }
        }

        return HttpClientDriver.SendGetRequest(apiResource,
            sb.Length == 0 ? null : sb.Remove(sb.Length - 1, 1).ToString());
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

    protected void ThenShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage)
    {
        ThenShouldExpectStatusCode(HttpStatusCode.UnprocessableEntity);

        var problemDetails = HttpClientDriver.DeserializeContent<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails?.Title.Should().Be(errorMessage);
    }

    public void ThenShouldGetTheRouteOfTheNewResourceInTheHeader()
    {
        var headers = HttpClientDriver.ResponseMessage!.Headers;

        headers.Should()
            .ContainKey(
                "Location"); //.And.ContainValue(new[] {"http://localhost/api/v2/store-items/df865211-60cc-4dcd-ad59-865352e446df"});
    }

    protected void ThenAssertAll(Action assertions)
    {
        using var scope = new AssertionScope();
        assertions();
    }

    private void CheckAuthorizationStatus(bool isAuthorized)
    {
        bool IsExpectedStatus(HttpStatusCode? statusCode) => isAuthorized
            ? statusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
            : statusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized;

        HttpClientDriver.ShouldHaveResponseWithStatus(IsExpectedStatus);
    }
}
