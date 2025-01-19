using System.Reflection;

using Auth0.OidcClient;

using CommunityToolkit.Maui;

using Holefeeder.Ui.Authentication;
using Holefeeder.Ui.Authentication.Persistence;
using Holefeeder.Ui.Common.Authentication;
using Holefeeder.Ui.Common.Extensions;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using MudBlazor.Services;

namespace Holefeeder.Ui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureLifecycleEvents(events =>
                {
#if IOS || MACCATALYST
                    events.AddiOS(ios => ios
                        .OnActivated((app) => LogEvent(nameof(iOSLifecycle.OnActivated)))
                        .OnResignActivation((app) => LogEvent(nameof(iOSLifecycle.OnResignActivation)))
                        .DidEnterBackground((app) => LogEvent(nameof(iOSLifecycle.DidEnterBackground)))
                        .WillTerminate((app) => LogEvent(nameof(iOSLifecycle.WillTerminate))));
#endif
                    static void LogEvent(string eventName, string? type = null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
                    }
                })

            .UseMauiCommunityToolkit()
            .ConfigureEssentials()
            .BuildConfiguration()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG || STAGING
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        builder.Services.AddMudServices();

        var auth0Config = builder.Configuration.GetSection("Auth0").Get<Auth0Config>() ??
                          throw new InvalidOperationException("Auth0 configuration is missing");
        var downStreamApiConfig = builder.Configuration.GetSection("DownstreamApi").Get<DownStreamApiConfig>() ??
                                  throw new InvalidOperationException("DownstreamApi configuration is missing");
        builder.Services.AddSingleton(auth0Config);
        builder.Services.AddSingleton(downStreamApiConfig);

        builder.Services.AddSingleton(new Auth0Client(new Auth0ClientOptions()
        {
            Domain = auth0Config.Domain,
            ClientId = auth0Config.ClientId,
            Scope = downStreamApiConfig.Scopes,
            RedirectUri = "myapp://callback",
            PostLogoutRedirectUri = "myapp://callback",
        }));
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationMessageHandler>();
        builder.Services.AddScoped<Auth0AuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(serviceProvider => serviceProvider.GetRequiredService<Auth0AuthenticationStateProvider>());
        builder.Services.AddScoped<IAuthNavigationManager>(serviceProvider => serviceProvider.GetRequiredService<Auth0AuthenticationStateProvider>());
        builder.Services.AddScoped<ITokenProvider, SecureStorageTokenProvider>();
        builder.Services.AddTokenService();
        builder.Services.AddRefitClients(provider => provider.GetRequiredService<AuthenticationMessageHandler>());

        return builder.Build();
    }

    private static MauiAppBuilder BuildConfiguration(this MauiAppBuilder builder)
    {
        // Load environment-specific configuration
        var environment = GetEnvironment();

        // Get the executing assembly to access the embedded resources
        var assembly = Assembly.GetExecutingAssembly();

        using var streamBaseFile =
            assembly.GetManifestResourceStream("Holefeeder.Ui.appsettings.json") ??
            throw new InvalidOperationException("Could not find appsettings.json");
        using var streamEnvFile =
            assembly.GetManifestResourceStream($"Holefeeder.Ui.appsettings.{environment}.json") ??
            throw new InvalidOperationException($"Could not find appsettings.{environment}.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(streamBaseFile)
            .AddJsonStream(streamEnvFile)
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        return builder;
    }

#pragma warning disable S3400 // Methods should not return constants
    private static string GetEnvironment()
    {
#if DEBUG
        const string env = "Development";
#elif STAGING
        const string env = "Staging";
#else
        const string env = "Production";
#endif
        return env;
    }
#pragma warning restore S3400 // Methods should not return constants
}
