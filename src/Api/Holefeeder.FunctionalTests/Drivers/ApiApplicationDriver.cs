using System.Security.Claims;
using Bogus;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;
using Holefeeder.Infrastructure.SeedWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;

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
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<BudgetingDatabaseDriver>();
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
        try
        {
            await DatabaseDriver.ResetCheckpointAsync().ConfigureAwait(false);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
    }

    Task IAsyncLifetime.InitializeAsync() => DatabaseDriver.InitializeAsync();

    Task IAsyncLifetime.DisposeAsync() => DatabaseDriver.DisposeAsync();
}
