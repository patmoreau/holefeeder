using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.FunctionalTests.StepDefinitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace Holefeeder.FunctionalTests.Features;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
[Collection("Api collection")]
public abstract partial class BaseScenario : IAsyncLifetime
{
    private static readonly AsyncLock _mutex = new();

    private readonly BudgetingDatabaseInitializer _budgetingDatabaseInitializer;
    private readonly ITestOutputHelper _testOutputHelper;

    private BudgetingDatabaseDriver? _databaseDriver;

    protected BaseScenario(ApiApplicationDriver apiApplicationDriver,
        BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        _budgetingDatabaseInitializer = budgetingDatabaseInitializer;
        _testOutputHelper = testOutputHelper;

        Scope = apiApplicationDriver.Services.CreateScope();

        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);

        StoreItem = new StoreItemStepDefinition(HttpClientDriver);
        Transaction = new TransactionStepDefinition(HttpClientDriver);
        User = new UserStepDefinition(HttpClientDriver);
    }

    private IServiceScope Scope { get; }

    protected BudgetingDatabaseDriver DatabaseDriver =>
        _databaseDriver ??= Scope.ServiceProvider.GetRequiredService<BudgetingDatabaseDriver>();

    protected HttpClientDriver HttpClientDriver { get; }

    protected StoreItemStepDefinition StoreItem { get; }
    protected TransactionStepDefinition Transaction { get; }
    protected UserStepDefinition User { get; }

    public async Task InitializeAsync()
    {
        // Version for reset every test
        using (await _mutex.LockAsync())
        {
            await _budgetingDatabaseInitializer.ResetCheckpoint();
        }
    }

    public Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }

    protected void GivenUserIsUnauthorized() => HttpClientDriver.UnAuthenticate();

    protected void GivenUserIsAuthorized() => HttpClientDriver.Authenticate();

    protected void GivenForbiddenUserIsAuthorized() => HttpClientDriver.AuthenticateUser(Guid.NewGuid());

    internal Task WhenUserTriesToQuery(ApiResource apiResource, int? offset = null, int? limit = null,
        string? sorts = null, string? filters = null)
    {
        StringBuilder sb = new StringBuilder();
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

        return HttpClientDriver.SendGetRequest(apiResource,
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
        if (assertions == null)
        {
            throw new ArgumentNullException(nameof(assertions));
        }

        using AssertionScope scope = new AssertionScope();
        assertions();
    }

#pragma warning disable CA1822
    protected void ThenAssertAll(Action assertions)
#pragma warning restore CA1822
    {
        if (assertions == null)
        {
            throw new ArgumentNullException(nameof(assertions));
        }

        using AssertionScope scope = new AssertionScope();
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

    protected async Task ScenarioFor(string description, Action<ScenarioPlayer> scenario)
    {
        if (scenario == null)
        {
            throw new ArgumentNullException(nameof(scenario));
        }

        ScenarioPlayer player = ScenarioPlayer.Create(description, _testOutputHelper);

        scenario(player);

        await player.PlayAsync();
    }

    [GeneratedRegex("[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?")]
    private static partial Regex MyRegex();
}
