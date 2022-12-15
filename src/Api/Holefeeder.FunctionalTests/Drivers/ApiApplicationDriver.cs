using AutoBogus;

using Bogus;

using Dapper;

using Holefeeder.Application.Context;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
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

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class ApiApplicationDriver : WebApplicationFactory<Api.Api>
{
    public HttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper) =>
        new(new Lazy<HttpClient>(CreateClient), testOutputHelper);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        AutoFaker.Configure(configBuilder =>
        {
            configBuilder.WithOverride<AccountType>(context =>
                context.Faker.PickRandom<AccountType>(AccountType.List));
            configBuilder.WithOverride<CategoryType>(context =>
                context.Faker.PickRandom<CategoryType>(CategoryType.List));
            configBuilder.WithOverride<DateIntervalType>(context =>
                context.Faker.PickRandom<DateIntervalType>(DateIntervalType.List));
        });

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"))
            .AddUserSecrets<FunctionalTestMarker>()
            .AddEnvironmentVariables()
            .Build();

        builder.UseConfiguration(configuration);

        builder
            .ConfigureTestServices(services =>
            {
                var holefeederConnection = configuration.GetConnectionString("HolefeederConnectionString");
                services.AddDbContext<BudgetingContext>(options =>
                    options.UseMySql(ServerVersion.AutoDetect(holefeederConnection)));
                services.AddOptions<HolefeederDatabaseSettings>()
                    .Bind(configuration.GetSection(nameof(HolefeederDatabaseSettings)))
                    .ValidateDataAnnotations();

                services.AddSingleton(sp =>
                    sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);

                services.AddScoped<HolefeederContext>();
                services.AddScoped<HolefeederDatabaseDriver>();
                services.AddScoped<BudgetingDatabaseDriver>();

                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddAuthentication(MockAuthenticationHandler.AUTHENTICATION_SCHEME)
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AUTHENTICATION_SCHEME, _ => { });

                DefaultTypeMap.MatchNamesWithUnderscores = true;
            });
    }
}
