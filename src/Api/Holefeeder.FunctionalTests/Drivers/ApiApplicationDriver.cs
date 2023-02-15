using AutoBogus;

using Dapper;

using Holefeeder.Application.Context;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            configBuilder.WithOverride(context => context.Faker.Date.Soon().Date);
            configBuilder.WithOverride(context => context.Faker.Finance.Amount());
        });

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"))
            .AddUserSecrets<ApiApplicationDriver>()
            .AddEnvironmentVariables()
            .Build();

        builder.UseConfiguration(configuration);

        builder
            .ConfigureTestServices(services =>
            {
                var holefeederConnection =
                    configuration.GetConnectionString(BudgetingConnectionStringBuilder.BUDGETING_CONNECTION_STRING);
                services.AddDbContext<BudgetingContext>(options =>
                    options.UseMySql(ServerVersion.AutoDetect(holefeederConnection)));

                services.AddScoped<BudgetingDatabaseDriver>();

                services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddAuthentication(MockAuthenticationHandler.AUTHENTICATION_SCHEME)
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AUTHENTICATION_SCHEME, _ => { });

                DefaultTypeMap.MatchNamesWithUnderscores = true;
            });
    }
}
