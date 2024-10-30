using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

using Bogus;

using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;

using FluentAssertionsEquivalency;

using Holefeeder.Application.Converters;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.SeedWork;
using Holefeeder.Tests.Common.Builders.Users;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Refit;

namespace Holefeeder.FunctionalTests.Drivers;

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public class ApiApplicationDriver : WebApplicationFactory<Api.Api>, IApplicationDriver, IAsyncLifetime
{
    private readonly bool _useDatabaseDriver;
    private readonly BudgetingDatabaseDriver? _databaseDriver;
    private readonly RefitSettings _refitSettings;

    internal BudgetingDatabaseDriver DatabaseDriver =>
        _databaseDriver ?? throw new InvalidOperationException("Database driver is not available");

    public ApiApplicationDriver() : this(true)
    {
    }

    protected ApiApplicationDriver(bool useDatabaseDriver)
    {
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new StronglyTypedIdJsonConverterFactory(),
                new MoneyJsonConverterFactory(),
                new CategoryColorJsonConverterFactory()
            }
        };
        _refitSettings = new()
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings)
        };

        _useDatabaseDriver = useDatabaseDriver;

        Faker.DefaultStrictMode = true;
        FluentAssertionsExtensions.RegisterGlobalEquivalencySteps();

        if (_useDatabaseDriver)
        {
            _databaseDriver = new BudgetingDatabaseDriver();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configPath = Path.Combine(GetProjectPath(string.Empty, "Holefeeder.FunctionalTests"),
            "appsettings.tests.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                new("Auth0:Domain", AuthorityDriver.AuthorityDomain),
                new("Auth0:Audience", "default-audience")
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
            services.AddSingleton<IUser>(_ =>
            {
                var userToken = new JwtTokenBuilder()
                    .WithScopes("read:user write:user")
                    .WithClaim(ClaimTypes.NameIdentifier, TestUsers[AuthorizedUser].IdentityObjectId)
                    .Build();
                var httpClient = CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                return RestService.For<IUser>(httpClient, _refitSettings);
            });

            services.AddScoped<BudgetingDatabaseDriver>();
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
                await DatabaseDriver.ResetCheckpointAsync();
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
            await DatabaseDriver.InitializeAsync();

            var user = UserBuilder.GivenAUser()
                .WithId(TestUsers[AuthorizedUser].UserId)
                .Build();
            await UserIdentityBuilder.GivenAUserIdentity(user)
                .WithIdentityObjectId(TestUsers[AuthorizedUser].IdentityObjectId)
                .SavedInDbAsync(DatabaseDriver);
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
