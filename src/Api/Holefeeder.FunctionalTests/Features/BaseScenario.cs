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

using Xunit;
using Xunit.Abstractions;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api collection")]
public abstract class BaseScenario
{
    protected BaseScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);

        User = new UserStepDefinition(HttpClientDriver);
        Transaction = new TransactionStepDefinition(HttpClientDriver);
    }

    protected HttpClientDriver HttpClientDriver { get; }

    protected UserStepDefinition User { get; }
    protected TransactionStepDefinition Transaction { get; }

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
}

public abstract class BaseScenario<T> : BaseScenario where T : BaseScenario<T>
{
    private readonly List<Func<Task>> _tasks = new();
    private readonly ITestOutputHelper _testOutputHelper;
    private int _taskCount;

    protected BaseScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    protected T Given(Action action)
    {
        return Given(string.Empty, action);
    }

    protected T Given(string message, Action action)
    {
        AddTask(nameof(Given), message, action);
        return (T)this;
    }

    protected T Given(Func<Task> action)
    {
        return Given(string.Empty, action);
    }

    protected T Given(string message, Func<Task> action)
    {
        AddTask(nameof(Given), message, action);

        return (T)this;
    }

    protected T When(Action action)
    {
        return When(string.Empty, action);
    }

    protected T When(string message, Action action)
    {
        AddTask(nameof(When), message, action);

        return (T)this;
    }

    protected T When(Func<Task> action)
    {
        return When(string.Empty, action);
    }

    protected T When(string message, Func<Task> action)
    {
        AddTask(nameof(When), message, action);

        return (T)this;
    }

    protected T Then(Action action)
    {
        return Then(string.Empty, action);
    }

    protected T Then(string message, Action action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using var scope = new AssertionScope();
            return Task.Run(action);
        });

        return (T)this;
    }

    protected T Then(Func<Task> action)
    {
        return Then(string.Empty, action);
    }

    protected T Then(string message, Func<Task> action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using var scope = new AssertionScope();
            return action();
        });

        return (T)this;
    }

    protected async Task RunScenarioAsync()
    {
        var tasks = _tasks.ToArray();
        _tasks.Clear();

        foreach (var task in tasks)
        {
            await task();
        }
    }

    private void AddTask(string command, string message, Action action)
    {
        AddTask(command, message, () => Task.Run(action));
    }

    private void AddTask(string command, string message, Func<Task> action)
    {
        _taskCount++;
        var text = string.IsNullOrWhiteSpace(message) ? $"task #{_taskCount}" : message;
        _tasks.Add(() => Task.Run(() => _testOutputHelper.WriteLine($"{command} {text}")));
        _tasks.Add(() => Task.Run(action));
    }
}
