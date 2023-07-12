using System.Security.Claims;
using Bogus;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;
using Holefeeder.Infrastructure.SeedWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using static Holefeeder.Infrastructure.SeedWork.BudgetingConnectionStringBuilder;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class ApiApplicationDriver : WebApplicationFactory<Api.Api>, IApplicationDriver, IAsyncLifetime
{
    internal BudgetingDatabaseDriver DatabaseDriver { get; }

    public ApiApplicationDriver()
    {
        Faker.DefaultStrictMode = true;

        DatabaseDriver = new BudgetingDatabaseDriver();
    }

    public IHttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper) =>
        HttpClientDriver.CreateDriver(CreateClient(), testOutputHelper);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<BudgetingConnectionStringBuilder>();
            services.AddSingleton<BudgetingConnectionStringBuilder>(_ => new BudgetingConnectionStringBuilder
            {
                ConnectionString = DatabaseDriver.ConnectionString
            });
        });
        // builder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
        // {
        //     IConfiguration configuration = configBuilder.Build();
        //     IConfigurationSection connectionStringsSection = configuration.GetSection("ConnectionStrings");
        //
        //     // Replace the value of a specific connection string key
        //     var myConnectionString = connectionStringsSection.GetSection(BUDGETING_CONNECTION_STRING);
        //     if (myConnectionString.Exists())
        //     {
        //         configuration[BUDGETING_CONNECTION_STRING] = DatabaseDriver.ConnectionString;
        //     }
        //
        //     // Replace the existing configuration with the modified one
        //     builder.UseConfiguration(configuration);
        // });
        builder.ConfigureTestServices(services =>
        {
            services.AddMockAuthentication(options =>
            {
                options.AdditionalUserClaims.Add(UserStepDefinition.HolefeederUserId.ToString(),
                    new List<Claim> { new(ClaimConstants.Scope, "holefeeder.user") });
            });
            services.AddDatabaseDriver(DatabaseDriver);
        });
    }

    public async Task ResetStateAsync()
    {
        await DatabaseDriver.ResetCheckpointAsync().ConfigureAwait(false);
    }

    Task IAsyncLifetime.InitializeAsync() => DatabaseDriver.InitializeAsync();

    Task IAsyncLifetime.DisposeAsync() => DatabaseDriver.DisposeAsync();
}
