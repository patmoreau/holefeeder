using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Holefeeder.Tests.Common.SeedWork.Infrastructure;
using Xunit.Abstractions;

namespace Holefeeder.Tests.Common.SeedWork.Drivers;

public class HttpClientDriver
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    public HttpClientDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    public HttpResponseMessage? ResponseMessage { get; private set; }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        ResponseMessage = await _httpClient.SendAsync(requestMessage);

        LogUnexpectedErrors();
    }

    internal async Task SendGetRequest(ApiResource apiResources, string? query = null)
    {
        Uri baseUri = apiResources.EndpointFromResource();
        Uri fullUri = baseUri;
        if (query is not null)
        {
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        using HttpRequestMessage request = new(HttpMethod.Get, fullUri);
        await SendRequest(request);
    }

    internal async Task SendGetRequest(ApiResource apiResources, params object[] parameters)
    {
        Uri endpointUri = apiResources.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Get, endpointUri);
        await SendRequest(request);
    }

    internal async Task SendPostRequest(ApiResource apiResources, string? body = null)
    {
        Uri endpointUri = apiResources.EndpointFromResource();

        using HttpRequestMessage request = new(HttpMethod.Post, endpointUri);
        if (body is not null)
        {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        await SendRequest(request);
    }

    internal async Task SendDeleteRequest(ApiResource apiResources, params object[] parameters)
    {
        Uri endpointUri = apiResources.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Delete, endpointUri);
        await SendRequest(request);
    }

    public void ShouldBeUnauthorized()
    {
        ResponseMessage.Should().NotBeNull()
            .And.HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    public void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        LogUnexpectedContent(httpStatus);
        ResponseMessage.Should().NotBeNull();
        ResponseMessage?.StatusCode.Should().Be(httpStatus);
    }

    public void ShouldNotHaveResponseWithOneOfStatuses(params HttpStatusCode[] httpStatuses)
    {
        ResponseMessage.Should().NotBeNull();
        ResponseMessage?.StatusCode.Should().BeOneOf(httpStatuses);
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
        string? resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;
        if (resultAsString is null)
        {
            return default;
        }

        T? content = JsonSerializer.Deserialize<T>(resultAsString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return content;
    }

    public void Authenticate() => AuthenticateUser(MockAuthenticationHandler.AuthorizedUserId);

    public void AuthenticateUser(Guid userId) =>
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId.ToString());

    public void UnAuthenticate() => _httpClient.DefaultRequestHeaders.Authorization = null;

    private void LogUnexpectedErrors()
    {
        if (ResponseMessage?.StatusCode != HttpStatusCode.InternalServerError)
        {
            return;
        }

        string? resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine($"HTTP 500 Response: {resultAsString ?? "<unknown>"}");
    }

    private void LogUnexpectedContent(HttpStatusCode expectedStatusCode)
    {
        if (ResponseMessage?.StatusCode == expectedStatusCode)
        {
            return;
        }

        string? resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine(
            $"Unexpected HTTP {ResponseMessage?.StatusCode} Code with Response: {resultAsString ?? "<unknown>"}");
    }
}
