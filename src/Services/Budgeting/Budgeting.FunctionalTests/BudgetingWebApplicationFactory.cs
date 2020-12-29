using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests
{
    public class BudgetingWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private bool _dataSeeded;

        protected override void ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            base.ConfigureClient(client);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Tests")
                .ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(Path.Combine(
                        GetProjectPath(String.Empty, typeof(BudgetingWebApplicationFactory).Assembly),
                        "appsettings.json"))
                        .AddEnvironmentVariables();
                })
                .ConfigureTestServices(services =>
                {
                    services.Remove(new ServiceDescriptor(typeof(IMongoDbContext),
                        a => a.GetService(typeof(IMongoDbContext)), ServiceLifetime.Singleton));

                    services.AddSingleton<IMongoDbContext, MongoDbContext>(provider =>
                    {
                        var settings = provider.GetService<IHolefeederDatabaseSettings>();

                        var context = new MongoDbContext(settings);

                        if (_dataSeeded)
                        {
                            return context;
                        }

                        BudgetingContextSeed.SeedData(settings);
                        _dataSeeded = true;

                        return context;
                    });

                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
        }

        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;

            var applicationBasePath = AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);

            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));

                if (projectDirectoryInfo.Exists)
                    if (new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"))
                        .Exists)
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
            } while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}
