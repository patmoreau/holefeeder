using System.Net;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.FunctionalTests.Steps;

[Binding]
public sealed class HostStepDefinitions
{
    private readonly HttpClientDriver _httpClientDriver;

    public HostStepDefinitions(HttpClientDriver httpClientDriver)
    {
        _httpClientDriver = httpClientDriver;
    }

    [When("I try to ([^']*) using query params with offset '([^']+|)' and limit '([^']+|)' and sorts '([^']*)' and filters '([^']*)'")]
    public Task WhenITryToFetchUsingQueryParams(ApiResources apiResource, int? offset, int? limit, string? sorts,
        string? filters)
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

        return _httpClientDriver.SendGetRequest(apiResource,
            sb.Length == 0 ? null : sb.Remove(sb.Length - 1, 1).ToString());
    }

    [Then(@"I should ((?>not )?be authorized) to access the endpoint")]
    public void ThenIShouldNotBeAuthorizedToAccessTheEndpoint(bool isAuthorized)
    {
        bool IsExpectedStatus(HttpStatusCode? statusCode) => isAuthorized
            ? statusCode != HttpStatusCode.Unauthorized
            : statusCode == HttpStatusCode.Unauthorized;

        _httpClientDriver.ShouldHaveResponseWithStatus(IsExpectedStatus);
    }

    [StepArgumentTransformation(@"((?>not )?be authorized)")]
    public static bool IsAuthorizedTransform(string text) => text == "be authorized";

    [Then(@"I expect a '([^']+)' status code")]
    public void ThenIExpectAStatusCode(HttpStatusCode expectedStatusCode)
    {
        _httpClientDriver.ShouldHaveResponseWithStatus(expectedStatusCode);
    }

    [Then(@"I get an problem details with error message saying '([^']*)'")]
    public async Task ThenIGetAnProblemDetailsWithErrorMessageSaying(string errorMessage)
    {
        var result = await _httpClientDriver.ResponseMessage!.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(result)!;
        problemDetails.Title.Should().Be(errorMessage);
    }
}
