using Dapper;

using Holefeeder.Application.Context;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Context;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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
                var connection = configuration.GetConnectionString("ObjectStoreConnectionString");
                services.AddDbContext<StoreItemContext>(options =>
                    options.UseMySql(ServerVersion.AutoDetect(connection)));
                var holefeederConnection = configuration.GetConnectionString("HolefeederConnectionString");
                services.AddDbContext<BudgetingContext>(options =>
                    options.UseMySql(ServerVersion.AutoDetect(holefeederConnection)));
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
                services.AddScoped<HolefeederDatabaseDriver>();
                services.AddScoped<BudgetingDatabaseDriver>();
                services.AddScoped<ObjectStoreDatabaseDriver>();

                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddAuthentication(MockAuthenticationHandler.AUTHENTICATION_SCHEME)
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AUTHENTICATION_SCHEME, _ => { });

                DefaultTypeMap.MatchNamesWithUnderscores = true;
            });
    }
}
