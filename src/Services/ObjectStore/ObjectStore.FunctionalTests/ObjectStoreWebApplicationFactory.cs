using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

using DrifterApps.Holefeeder.ObjectStore.API;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ObjectStore.FunctionalTests
{
    public class ObjectStoreWebApplicationFactory : WebApplicationFactory<Program>
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
                            GetProjectPath(String.Empty, typeof(ObjectStoreWebApplicationFactory).Assembly),
                            "appsettings.json"))
                        .AddEnvironmentVariables();

                    var c = conf.Build();
                    var settings = c.GetSection(nameof(ObjectStoreDatabaseSettings))
                        .Get<ObjectStoreDatabaseSettings>();

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
            var settings = Services.GetRequiredService<ObjectStoreDatabaseSettings>();

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

                StoreItemContextSeed.SeedData(settings);
                _dataSeeded = true;
            }
        }

        private static void PrepareData(ObjectStoreDatabaseSettings settings)
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

                StoreItemContextSeed.PrepareData(settings);
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
                    return Path.Combine(projectDirectoryInfo.FullName, projectName!);
            } while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}
