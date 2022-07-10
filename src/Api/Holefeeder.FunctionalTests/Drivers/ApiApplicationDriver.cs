using Dapper;

using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Context;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class ApiApplicationDriver : WebApplicationFactory<Api.Api>
{
    public HttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper) =>
        new(new Lazy<HttpClient>(CreateClient), testOutputHelper);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"))
            .AddUserSecrets<FunctionalTestMarker>()
            .AddEnvironmentVariables()
            .Build();

        builder.UseConfiguration(configuration);

        builder
            .ConfigureTestServices(services =>
            {
                services
                    .AddOptions<ObjectStoreDatabaseSettings>()
                    .Bind(configuration.GetSection(nameof(ObjectStoreDatabaseSettings)));
                services.AddOptions<HolefeederDatabaseSettings>()
                    .Bind(configuration.GetSection(nameof(HolefeederDatabaseSettings)))
                    .ValidateDataAnnotations();

                services.AddSingleton(sp =>
                    sp.GetRequiredService<IOptions<ObjectStoreDatabaseSettings>>().Value);
                services.AddSingleton(sp =>
                    sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);

                services.AddScoped<HolefeederContext>();
                services.AddScoped<ObjectStoreContext>();

                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddAuthentication(MockAuthenticationHandler.AUTHENTICATION_SCHEME)
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AUTHENTICATION_SCHEME, _ => { });

                DefaultTypeMap.MatchNamesWithUnderscores = true;
            });
    }

    public HolefeederDatabaseDriver CreateHolefeederDatabaseDriver() => new(this);

    public ObjectStoreDatabaseDriver CreateObjectStoreDatabaseDriver() => new(this);
}
