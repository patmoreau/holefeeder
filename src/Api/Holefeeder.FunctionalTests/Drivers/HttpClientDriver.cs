using System.Net;
using System.Reflection;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.FunctionalTests.Infrastructure;

using IdentityModel.Client;

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
    private HttpResponseMessage? _responseMessage;

    public HttpClientDriver()
    {
        _httpClient = new Lazy<HttpClient>(CreateClient);
    }

    public HttpResponseMessage? ResponseMessage => _responseMessage;

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
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
    }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        _responseMessage = await _httpClient.Value.SendAsync(requestMessage);
    }

    internal Task SendGetRequest(ApiResources apiResource, string? query = null)
    {
        Uri baseUri = ResourceRouteAttribute.EndpointFromResource(apiResource);
        Uri fullUri = baseUri;
        if (query is not null)
        {
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        HttpRequestMessage request = new(HttpMethod.Get, fullUri);

        return SendRequest(request);
    }

    internal Task SendGetRequest(ApiResources apiResource, params object?[] parameters)
    {
        Uri endpointUri = ResourceRouteAttribute.EndpointFromResource(apiResource, parameters);
        HttpRequestMessage request = new(HttpMethod.Get, endpointUri);

        return SendRequest(request);
    }

    public void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        _responseMessage.Should().NotBeNull();
        _responseMessage?.StatusCode.Should().Be(httpStatus);
    }

    public void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate)
    {
        _responseMessage.Should().NotBeNull();
        httpStatusPredicate(_responseMessage?.StatusCode).Should().BeTrue();
    }

    public async Task<T?> DeserializeContent<T>()
    {
        var resultAsString = await _responseMessage!.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<T>(resultAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return content;
    }

    public async Task Authenticate()
    {
        var tokenRequest = new PasswordTokenRequest
        {
            Address = "https://localhost:9999/accesstoken",
            Method = HttpMethod.Post,
            GrantType = "password",
            UserName = "admin",
            Password = "admin",
            ClientId = "client_id",
            ClientSecret = "client_secret"
        };
        var response = await _httpClient.Value.RequestPasswordTokenAsync(tokenRequest);

        _httpClient.Value.SetBearerToken(response?.AccessToken);

        _httpClient.Value.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
            TestAuthHandler.AuthenticatedUserId.ToString());

        // return Task.CompletedTask;
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
