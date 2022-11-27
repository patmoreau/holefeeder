using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.FunctionalTests.Infrastructure;

using Xunit.Abstractions;

namespace Holefeeder.FunctionalTests.Drivers;

public class HttpClientDriver
{
    private readonly Lazy<HttpClient> _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    public HttpClientDriver(Lazy<HttpClient> httpClient, ITestOutputHelper testOutputHelper)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    public HttpResponseMessage? ResponseMessage { get; private set; }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        ResponseMessage = await _httpClient.Value.SendAsync(requestMessage);

        LogUnexpectedErrors();
    }

    public async Task SendGetRequest(ApiResources apiResource, string? query = null)
    {
        var baseUri = ResourceRouteAttribute.EndpointFromResource(apiResource);
        var fullUri = baseUri;
        if (query is not null)
        {
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        using HttpRequestMessage request = new(HttpMethod.Get, fullUri);
        await SendRequest(request);
    }

    public async Task SendGetRequest(ApiResources apiResource, params object?[] parameters)
    {
        var endpointUri = ResourceRouteAttribute.EndpointFromResource(apiResource, parameters);
        using HttpRequestMessage request = new(HttpMethod.Get, endpointUri);
        await SendRequest(request);
    }

    public async Task SendPostRequest(ApiResources apiResource, string? body = null)
    {
        var endpointUri = ResourceRouteAttribute.EndpointFromResource(apiResource);

        using HttpRequestMessage request = new(HttpMethod.Post, endpointUri);
        if (body is not null)
        {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        await SendRequest(request);
    }

    public void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        ResponseMessage.Should().NotBeNull();
        ResponseMessage?.StatusCode.Should().Be(httpStatus);
    }

    public void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate)
    {
        if (httpStatusPredicate == null)
        {
            throw new ArgumentNullException(nameof(httpStatusPredicate));
        }

        ResponseMessage.Should().NotBeNull();
        httpStatusPredicate(ResponseMessage?.StatusCode).Should().BeTrue();
    }

    public T? DeserializeContent<T>()
    {
        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;
        if (resultAsString is null)
        {
            return default;
        }

        var content = JsonSerializer.Deserialize<T>(resultAsString,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        return content;
    }

    public void Authenticate()
    {
        AuthenticateUser(MockAuthenticationHandler.AuthorizedUserId);
    }

    public void AuthenticateUser(Guid userId)
    {
        _httpClient.Value.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AUTHENTICATION_SCHEME, userId.ToString());
    }

    public void UnAuthenticate()
    {
        _httpClient.Value.DefaultRequestHeaders.Authorization = null;
    }

    private void LogUnexpectedErrors()
    {
        if (ResponseMessage?.StatusCode != HttpStatusCode.InternalServerError)
        {
            return;
        }

        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine($"HTTP 500 Response: {resultAsString ?? "<unknown>"}");
    }
}
