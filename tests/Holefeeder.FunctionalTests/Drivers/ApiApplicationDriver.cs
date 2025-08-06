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
    [MemberNotNullWhen(true, nameof(_databaseDriver))]
    private bool UseDatabaseDriver { get; }

    private readonly BudgetingDatabaseDriver? _databaseDriver;
    protected AuthorityDriver AuthorityDriver { get; } = new();
    private readonly RefitSettings _refitSettings;

    internal BudgetingDatabaseDriver DatabaseDriver =>
        _databaseDriver ?? throw new InvalidOperationException("Database driver is not available");

    public ApiApplicationDriver() : this(true)
    {
    }

    protected ApiApplicationDriver(bool useDatabaseDriver)
    {
        var jsonSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new StronglyTypedIdJsonConverterFactory(),
                new MoneyJsonConverterFactory(),
                new CategoryColorJsonConverterFactory(),
            }
        };
        _refitSettings = new()
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings)
        };

        UseDatabaseDriver = useDatabaseDriver;

        Faker.DefaultStrictMode = true;
        FluentAssertionsExtensions.RegisterGlobalEquivalencySteps();

        if (UseDatabaseDriver)
        {
            _databaseDriver = new BudgetingDatabaseDriver();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("FUNCTIONAL_TESTS");
        var configPath = Path.Combine(GetProjectPath(string.Empty, "Holefeeder.FunctionalTests"),
            "appsettings.tests.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                new("Authorization:Auth0:MetadataAddress",
                    $"{AuthorityDriver.Authority}.well-known/openid-configuration"),
            }).Build();
        builder.UseConfiguration(configuration);
        builder.ConfigureServices(collection =>
        {
            if (!UseDatabaseDriver)
            {
                return;
            }

            collection.RemoveAll<BudgetingConnectionStringBuilder>();
            collection.AddSingleton<BudgetingConnectionStringBuilder>(_ => new BudgetingConnectionStringBuilder
            {
                ConnectionString = _databaseDriver.ConnectionString
            });
        });
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IUser>(_ =>
            {
                var userToken = new JwtTokenBuilder()
                    .IssuedBy($"{AuthorityDriver.Authority}")
                    .ForAudience("https://holefeeder-api.drifterapps.app")
                    .WithScopes("read:user write:user")
                    .WithClaim(ClaimTypes.NameIdentifier, TestUsers[AuthorizedUser].IdentityObjectId)
                    .Build();
                var httpClient = CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                return RestService.For<IUser>(httpClient, _refitSettings);
            });

            services.AddScoped<BudgetingDatabaseDriver>();
            if (UseDatabaseDriver)
            {
                services.AddDatabaseDriver(_databaseDriver);
            }
        });
    }

    public async Task ResetStateAsync()
    {
        try
        {
            if (UseDatabaseDriver)
            {
                await _databaseDriver.ResetCheckpointAsync();
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task InitializeAsync()
    {
        await AuthorityDriver.InitializeAsync();
        if (UseDatabaseDriver)
        {
            await _databaseDriver.InitializeAsync();

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
        if (UseDatabaseDriver)
        {
            await _databaseDriver.DisposeAsync();
        }

        await AuthorityDriver.DisposeAsync();
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
