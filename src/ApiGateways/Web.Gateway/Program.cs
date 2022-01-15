using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;

namespace DrifterApps.Holefeeder.Web.Gateway
{
    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.WithProperty("Env", Configuration["ASPNETCORE_ENVIRONMENT"])
                .Enrich.WithProperty("MicroServiceName", "Web.Gateway")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(Configuration["SEQ_Url"], apiKey: Configuration["SEQ_ApiKey"])
                .CreateLogger();

            Log.Logger.Information("Web.Gateway started");
            try
            {
                BuildWebHost(args).Run();
            }
            // ReSharper disable once CA1031
#pragma warning disable CA1031
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
#pragma warning restore
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("appsettings.json", true)
                        .AddJsonFile($"ocelot.json", false, true)
                        .AddEnvironmentVariables();
                })
                .UseSerilog()
                .UseDefaultServiceProvider((ctx, opts) =>
                {
                    /* elided for brevity */
                })
                .UseStartup<Startup>()
                .Build();
    }
}
