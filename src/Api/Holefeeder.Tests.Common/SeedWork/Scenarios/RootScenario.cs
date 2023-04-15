using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using Holefeeder.Tests.Common.SeedWork.Drivers;
using Holefeeder.Tests.Common.SeedWork.Infrastructure;
using Holefeeder.Tests.Common.SeedWork.StepDefinitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace Holefeeder.Tests.Common.SeedWork.Scenarios;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
public abstract partial class RootScenario : IAsyncLifetime
{
    private static readonly AsyncLock _mutex = new();

    private readonly DatabaseInitializer _databaseInitializer;
    private readonly ITestOutputHelper _testOutputHelper;

    protected RootScenario(IApplicationDriver applicationDriver,
        DatabaseInitializer databaseInitializer, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(applicationDriver);

        _databaseInitializer = databaseInitializer;
        _testOutputHelper = testOutputHelper;

        Scope = applicationDriver.Services.CreateScope();

        HttpClientDriver = applicationDriver.CreateHttpClientDriver(testOutputHelper);

        User = new UserStepDefinition(HttpClientDriver);
    }

    protected UserStepDefinition User { get; }

    private IServiceScope Scope { get; }

    protected HttpClientDriver HttpClientDriver { get; }

    public virtual async Task InitializeAsync()
    {
        // Version for reset every test
        using (await _mutex.LockAsync())
        {
            await _databaseInitializer.ResetCheckpoint();
        }
    }

    public virtual Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }

    internal Task WhenUserTriesToQuery(ApiResource apiResources, int? offset = null, int? limit = null,
        string? sorts = null, string? filters = null)
    {
        StringBuilder sb = new();
        if (offset is not null)
        {
            sb.Append(CultureInfo.InvariantCulture, $"offset={offset}&");
        }

        if (limit is not null)
        {
            sb.Append(CultureInfo.InvariantCulture, $"limit={limit}&");
        }

        if (!string.IsNullOrWhiteSpace(sorts))
        {
            foreach (string sort in sorts.Split(';'))
            {
                sb.Append(CultureInfo.InvariantCulture, $"sort={sort}&");
            }
        }

        if (!string.IsNullOrWhiteSpace(filters))
        {
            foreach (string filter in filters.Split(';'))
            {
                sb.Append(CultureInfo.InvariantCulture, $"filter={filter}&");
            }
        }

        return HttpClientDriver.SendGetRequest(apiResources,
            sb.Length == 0 ? null : sb.Remove(sb.Length - 1, 1).ToString());
    }

    protected void ThenShouldNotHaveInternalServerError() =>
        HttpClientDriver.ShouldHaveResponseWithStatus(statusCode => statusCode != HttpStatusCode.InternalServerError);

    protected void ThenUserShouldBeAuthorizedToAccessEndpoint() => CheckAuthorizationStatus(true);

    protected void ShouldBeForbiddenToAccessEndpoint() => CheckAuthorizationStatus(false);

    protected void ShouldNotBeAuthorizedToAccessEndpoint() => CheckAuthorizationStatus(false);

    protected TContent ThenShouldReceive<TContent>()
    {
        TContent? result = HttpClientDriver.DeserializeContent<TContent>();
        result.Should().NotBeNull();
        return result!;
    }

    protected void ThenShouldExpectStatusCode(HttpStatusCode expectedStatusCode) =>
        HttpClientDriver.ShouldHaveResponseWithStatus(expectedStatusCode);

    protected void ThenShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode expectedStatusCode,
        string errorMessage)
    {
        ThenShouldExpectStatusCode(expectedStatusCode);

        ProblemDetails? problemDetails = HttpClientDriver.DeserializeContent<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails?.Detail.Should().Be(errorMessage);
    }

    protected void ShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage)
    {
        ThenShouldExpectStatusCode(HttpStatusCode.UnprocessableEntity);

        ValidationProblemDetails? problemDetails = HttpClientDriver.DeserializeContent<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails?.Title.Should().Be(errorMessage);
    }

    protected Guid ThenShouldGetTheRouteOfTheNewResourceInTheHeader()
    {
        HttpResponseHeaders headers = HttpClientDriver.ResponseMessage!.Headers;

        headers.Should().ContainKey("Location");

        string responseString = headers.GetValues("Location").Single();
        Match match = MyRegex().Match(responseString);

        return match.Success ? Guid.Parse(match.Value) : Guid.Empty;
    }

#pragma warning disable CA1822
    protected void ThenAssertAll(Func<Task> assertions)
#pragma warning restore CA1822
    {
        ArgumentNullException.ThrowIfNull(assertions);

        using AssertionScope scope = new();
        assertions();
    }

#pragma warning disable CA1822
    protected void ThenAssertAll(Action assertions)
#pragma warning restore CA1822
    {
        ArgumentNullException.ThrowIfNull(assertions);

        using AssertionScope scope = new();
        assertions();
    }

    private void CheckAuthorizationStatus(bool isAuthorized)
    {
        bool IsExpectedStatus(HttpStatusCode? statusCode)
        {
            return isAuthorized
                ? statusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                : statusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized;
        }

        HttpClientDriver.ShouldHaveResponseWithStatus(IsExpectedStatus);
    }

    protected async Task ScenarioFor(string description, Action<IScenarioRunner> scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        ScenarioRunner runner = ScenarioRunner.Create(description, _testOutputHelper);

        scenario(runner);

        await runner.PlayAsync();
    }

    [GeneratedRegex("[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?")]
    private static partial Regex MyRegex();
}
