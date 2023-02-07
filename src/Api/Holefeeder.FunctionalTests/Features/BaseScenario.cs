using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.FunctionalTests.StepDefinitions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api collection")]
public abstract class BaseScenario : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;

    protected BaseScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        _testOutputHelper = testOutputHelper;

        Scope = apiApplicationDriver.Services.CreateScope();

        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);

        StoreItem = new StoreItemStepDefinition(HttpClientDriver);
        Transaction = new TransactionStepDefinition(HttpClientDriver);
        User = new UserStepDefinition(HttpClientDriver);
    }

    private IServiceScope Scope { get; }

    private BudgetingDatabaseDriver? _databaseDriver;

    protected BudgetingDatabaseDriver DatabaseDriver
    {
        get => _databaseDriver ??= Scope.ServiceProvider.GetRequiredService<BudgetingDatabaseDriver>();
    }

    protected HttpClientDriver HttpClientDriver { get; }

    protected StoreItemStepDefinition StoreItem { get; }
    protected TransactionStepDefinition Transaction { get; }
    protected UserStepDefinition User { get; }

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
            sb.Append(CultureInfo.InvariantCulture, $"offset={offset}&");
        }

        if (limit is not null)
        {
            sb.Append(CultureInfo.InvariantCulture, $"limit={limit}&");
        }

        if (!string.IsNullOrWhiteSpace(sorts))
        {
            foreach (var sort in sorts.Split(';'))
            {
                sb.Append(CultureInfo.InvariantCulture, $"sort={sort}&");
            }
        }

        if (!string.IsNullOrWhiteSpace(filters))
        {
            foreach (var filter in filters.Split(';'))
            {
                sb.Append(CultureInfo.InvariantCulture, $"filter={filter}&");
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

    protected void ShouldBeForbiddenToAccessEndpoint()
    {
        CheckAuthorizationStatus(false);
    }

    protected void ShouldNotBeAuthorizedToAccessEndpoint()
    {
        CheckAuthorizationStatus(false);
    }

    protected TContent ThenShouldReceive<TContent>()
    {
        var result = HttpClientDriver.DeserializeContent<TContent>();
        result.Should().NotBeNull();
        return result!;
    }

    protected void ThenShouldExpectStatusCode(HttpStatusCode expectedStatusCode)
    {
        HttpClientDriver.ShouldHaveResponseWithStatus(expectedStatusCode);
    }

    protected void ThenShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode expectedStatusCode,
        string errorMessage)
    {
        ThenShouldExpectStatusCode(expectedStatusCode);

        var problemDetails = HttpClientDriver.DeserializeContent<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails?.Detail.Should().Be(errorMessage);
    }

    protected void ShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage)
    {
        ThenShouldExpectStatusCode(HttpStatusCode.UnprocessableEntity);

        var problemDetails = HttpClientDriver.DeserializeContent<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails?.Title.Should().Be(errorMessage);
    }

    protected Guid ThenShouldGetTheRouteOfTheNewResourceInTheHeader()
    {
        var headers = HttpClientDriver.ResponseMessage!.Headers;

        headers.Should().ContainKey("Location");

        var responseString = headers.GetValues("Location").Single();
        var match = Regex.Match(responseString, @"[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?");

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

        using var scope = new AssertionScope();
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

        using var scope = new AssertionScope();
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

    protected void ScenarioFor(string description, Action<ScenarioPlayer> scenario)
    {
        var player = ScenarioPlayer.Create(description, _testOutputHelper);

        scenario(player);

        player.Play();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Scope.Dispose();
        }

        _disposed = true;
    }
}
