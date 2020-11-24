using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace DrifterApps.Holefeeder.Hosting.Gateway.API
{
    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.WithProperty("Env", Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production")
                .Enrich.WithProperty("MicroServiceName", "Hosting.Gateway.API")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(Configuration["SEQ_Url"] ?? "http://localhost:5341", apiKey: Configuration["SEQ_ApiKey"])
                .CreateLogger();

            Log.Logger.Information("Hosting.Gateway.API started");
            try
            {
                CreateHostBuilder(args).Build().Run();
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

        private static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.AddJsonFile($"ocelot.json", false, true);
                })
                .UseStartup<Startup>();
    }
}
