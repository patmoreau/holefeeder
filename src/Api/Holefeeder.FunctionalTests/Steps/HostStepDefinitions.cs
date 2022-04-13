using System.Net;
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

    [When(
        "I try to fetch (StoreItemsQuery|Budgeting) using query params with offset '([^']+)' and limit '([^']+)' and sort '([^']*)' and filter '([^']*)'")]
    public Task WhenITryToFetchUsingQueryParamsOfOffsetAndSorts(ApiResources apiResource, int offset, int limit,
        string sort, string filter)
    {
        return _httpClientDriver.SendGetRequest(apiResource, offset, limit, sort, filter);
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
