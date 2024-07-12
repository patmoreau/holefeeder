using System.Security.Claims;

using Bogus;

using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Drivers;

using Holefeeder.Infrastructure.SeedWork;
using Holefeeder.Tests.Common.Builders.Users;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Holefeeder.FunctionalTests.Drivers;

public class ApiApplicationDriver : WebApplicationFactory<Api.Api>, IApplicationDriver, IAsyncLifetime
{
    private readonly bool _useDatabaseDriver;
    private readonly string _domain = new Faker().Internet.DomainName();
    private readonly BudgetingDatabaseDriver? _databaseDriver;

    internal BudgetingDatabaseDriver DatabaseDriver =>
        _databaseDriver ?? throw new InvalidOperationException("Database driver is not available");

    public ApiApplicationDriver() : this(true)
    {
    }

    protected ApiApplicationDriver(bool useDatabaseDriver)
    {
        _useDatabaseDriver = useDatabaseDriver;

        Faker.DefaultStrictMode = true;

        if (_useDatabaseDriver)
        {
            _databaseDriver = new BudgetingDatabaseDriver();
        }
    }

    public IHttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper) =>
        HttpClientDriver.CreateDriver(CreateClient(), testOutputHelper);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configPath = Path.Combine(GetProjectPath(string.Empty, "Holefeeder.FunctionalTests"),
            "appsettings.tests.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                new("Auth0:Domain", _domain)
            }).Build();
        builder.UseConfiguration(configuration);
        builder.ConfigureServices(collection =>
        {
            if (!_useDatabaseDriver)
            {
                return;
            }

            collection.RemoveAll<BudgetingConnectionStringBuilder>();
            collection.AddSingleton<BudgetingConnectionStringBuilder>(_ => new BudgetingConnectionStringBuilder
            {
                ConnectionString = DatabaseDriver.ConnectionString
            });
        });
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<BudgetingDatabaseDriver>();
            services.AddMockAuthentication(options =>
            {
                var authorizedUser = TestUsers[AuthorizedUser];
                options.AdditionalUserClaims.Add(authorizedUser.IdentityObjectId,
                    [new Claim("scope", authorizedUser.Scope, null, $"https://{_domain}/")]);
            });
            if (_useDatabaseDriver)
            {
                services.AddDatabaseDriver(DatabaseDriver);
            }
        });
    }

    public async Task ResetStateAsync()
    {
        try
        {
            if (_useDatabaseDriver)
            {
                await DatabaseDriver.ResetCheckpointAsync().ConfigureAwait(false);
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task InitializeAsync()
    {
        if (_useDatabaseDriver)
        {
            await DatabaseDriver.InitializeAsync().ConfigureAwait(false);

            var user = UserBuilder.GivenAUser()
                .WithId(TestUsers[AuthorizedUser].UserId)
                .Build();
            await UserIdentityBuilder.GivenAUserIdentity(user)
                .WithIdentityObjectId(TestUsers[AuthorizedUser].IdentityObjectId)
                .SavedInDbAsync(DatabaseDriver).ConfigureAwait(false);
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_useDatabaseDriver)
        {
            await DatabaseDriver.DisposeAsync();
        }
    }

    private static string GetProjectPath(string projectRelativePath, string projectName)
    {
        var applicationBasePath = AppContext.BaseDirectory;
        var directoryInfo = new DirectoryInfo(applicationBasePath);
        do
        {
            directoryInfo = directoryInfo.Parent;

            var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo!.FullName, projectRelativePath));

            if (projectDirectoryInfo.Exists &&
                new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj")).Exists)
            {
                return Path.Combine(projectDirectoryInfo.FullName, projectName);
            }
        } while (directoryInfo.Parent is not null);

        throw new InvalidOperationException(
            $"Project root could not be located using the application root {applicationBasePath}.");
    }
}
