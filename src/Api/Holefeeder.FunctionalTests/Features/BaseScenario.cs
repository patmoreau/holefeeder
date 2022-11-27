using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using FluentAssertions;
using FluentAssertions.Execution;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.FunctionalTests.StepDefinitions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api collection")]
public abstract class BaseScenario : IDisposable
{
    protected BaseScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        Scope = apiApplicationDriver.Services.CreateScope();

        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);

        StoreItem = new StoreItemStepDefinition(HttpClientDriver);
        Transaction = new TransactionStepDefinition(HttpClientDriver);
        User = new UserStepDefinition(HttpClientDriver);
    }

    protected IServiceScope Scope { get; }

    private HolefeederDatabaseDriver? _holefeederDatabaseDriver = null;
    protected HolefeederDatabaseDriver HolefeederDatabaseDriver
    {
        get => _holefeederDatabaseDriver ??= Scope.ServiceProvider.GetRequiredService<HolefeederDatabaseDriver>();
    }

    private BudgetingDatabaseDriver? _budgetingDatabaseDriver = null;
    protected BudgetingDatabaseDriver BudgetingDatabaseDriver
    {
        get => _budgetingDatabaseDriver ??= Scope.ServiceProvider.GetRequiredService<BudgetingDatabaseDriver>();
    }

    private ObjectStoreDatabaseDriver? _objectStoreDatabaseDriver = null;
    protected ObjectStoreDatabaseDriver ObjectStoreDatabaseDriver
    {
        get => _objectStoreDatabaseDriver ??= Scope.ServiceProvider.GetRequiredService<ObjectStoreDatabaseDriver>();
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

    protected void ThenShouldExpectStatusCode(HttpStatusCode expectedStatusCode)
    {
        HttpClientDriver.ShouldHaveResponseWithStatus(expectedStatusCode);
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

    public void Dispose()
    {
        Scope.Dispose();
        GC.SuppressFinalize(Scope);
    }
}
