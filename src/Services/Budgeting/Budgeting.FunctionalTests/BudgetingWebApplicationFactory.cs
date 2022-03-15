using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

using Framework.Dapper.SeedWork.Extensions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests;

public class BudgetingWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly object Locker = new();
    private static bool _dataSeeded;
    private static bool _dataPrepared;

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        base.ConfigureClient(client);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests")
            .ConfigureAppConfiguration((_, conf) =>
            {
                conf.AddJsonFile(Path.Combine(
                        GetProjectPath(String.Empty, typeof(BudgetingWebApplicationFactory).Assembly),
                        "appsettings.json"))
                    .AddEnvironmentVariables();

                var c = conf.Build();
                var settings = c.GetSection(nameof(HolefeederDatabaseSettings))
                    .Get<HolefeederDatabaseSettings>();

                PrepareData(settings);
            })
            .ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
    }

    public void SeedData()
    {
        var settings = Services.GetRequiredService<HolefeederDatabaseSettings>();

        if (_dataSeeded)
        {
            return;
        }

        lock (Locker)
        {
            if (_dataSeeded)
            {
                return;
            }

            BudgetingContextSeed.SeedData(settings);
            _dataSeeded = true;
        }
    }

    private static void PrepareData(HolefeederDatabaseSettings settings)
    {
        if (_dataPrepared)
        {
            return;
        }

        lock (Locker)
        {
            if (_dataPrepared)
            {
                return;
            }

            BudgetingContextSeed.PrepareData(settings);
            _dataPrepared = true;
        }
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

    public async Task SeedAccountData(Func<IHolefeederContext, AccountEntity> predicate)
    {
        var context = Services.GetService<IHolefeederContext>() ??
                      throw new InvalidOperationException("Unable to get IHolefeederContext");

        var schema = predicate(context);

        await context.Connection.InsertAsync(schema).ConfigureAwait(false);
    }
}
