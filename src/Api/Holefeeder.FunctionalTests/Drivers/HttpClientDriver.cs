using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.FunctionalTests.Drivers;

[Binding]
public sealed class HttpClientDriver : WebApplicationFactory<Api.Api>
{
    private readonly Lazy<HttpClient> _httpClient;

    public HttpClientDriver()
    {
        _httpClient = new Lazy<HttpClient>(CreateClient);
    }

    public string DefaultUserId { get; set; } = Guid.NewGuid().ToString();

    public HttpResponseMessage? ResponseMessage { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration((_, conf) =>
            {
                conf.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"))
                    .AddEnvironmentVariables();
            })
            .ConfigureTestServices(services =>
            {
                services.Configure<MockAuthenticationHandlerOptions>(options => options.DefaultUserId = DefaultUserId);
                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddAuthentication(MockAuthenticationHandler.AUTHENTICATION_SCHEME)
                    .AddScheme<MockAuthenticationHandlerOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AUTHENTICATION_SCHEME, _ => { });
                // services.AddAuthentication("Test")
                //     .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
    }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        ResponseMessage = await _httpClient.Value.SendAsync(requestMessage);
    }

    internal Task SendGetRequest(ApiResources apiResource, string? query = null)
    {
        var baseUri = ResourceRouteAttribute.EndpointFromResource(apiResource);
        var fullUri = baseUri;
        if (query is not null)
        {
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        HttpRequestMessage request = new(HttpMethod.Get, fullUri);

        return SendRequest(request);
    }

    internal Task SendGetRequest(ApiResources apiResource, params object?[] parameters)
    {
        var endpointUri = ResourceRouteAttribute.EndpointFromResource(apiResource, parameters);
        HttpRequestMessage request = new(HttpMethod.Get, endpointUri);

        return SendRequest(request);
    }

    internal Task SendPostRequest(ApiResources apiResource, string? body = null)
    {
        var endpointUri = ResourceRouteAttribute.EndpointFromResource(apiResource);

        HttpRequestMessage request = new(HttpMethod.Post, endpointUri);
        if (body is not null)
        {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        return SendRequest(request);
    }

    public void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        ResponseMessage.Should().NotBeNull();
        ResponseMessage?.StatusCode.Should().Be(httpStatus);
    }

    public void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate)
    {
        ResponseMessage.Should().NotBeNull();
        httpStatusPredicate(ResponseMessage?.StatusCode).Should().BeTrue();
    }

    public async Task<T?> DeserializeContent<T>()
    {
        var resultAsString = await ResponseMessage!.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<T>(resultAsString,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        return content;
    }

    public void Authenticate()
    {
        AuthenticateUser(DefaultUserId);
    }

    public void AuthenticateUser(string userId)
    {
        UnAuthenticate();

        _httpClient.Value.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AUTHENTICATION_SCHEME, userId);
    }

    public void UnAuthenticate()
    {
        _httpClient.Value.DefaultRequestHeaders.Authorization = null;
    }

    private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
    {
        var projectName = startupAssembly.GetName().Name;

        var applicationBasePath = AppContext.BaseDirectory;

        var directoryInfo = new DirectoryInfo(applicationBasePath);

        do
        {
            directoryInfo = directoryInfo.Parent;

            var projectDirectoryInfo =
                new DirectoryInfo(Path.Combine(directoryInfo!.FullName, projectRelativePath));

            if (projectDirectoryInfo.Exists &&
                new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName!, $"{projectName}.csproj"))
                    .Exists)
            {
                return Path.Combine(projectDirectoryInfo.FullName, projectName!);
            }
        } while (directoryInfo.Parent != null);

        throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
    }
}
