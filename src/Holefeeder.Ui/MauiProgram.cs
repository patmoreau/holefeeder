using System.Globalization;

using Auth0.OidcClient;

using CommunityToolkit.Maui;

using Holefeeder.Ui.Authentication;
using Holefeeder.Ui.Authentication.Persistence;
using Holefeeder.Ui.Services;
using Holefeeder.Ui.Shared.Authentication;
using Holefeeder.Ui.Shared.Extensions;
using Holefeeder.Ui.Shared.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

using MudBlazor.Services;

namespace Holefeeder.Ui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureEssentials()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .AddConfiguration();

        // Add device-specific services used by the Holefeeder.Ui.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        IdentityModelEventSource.ShowPII = true;
#endif

        builder.Services.AddMudServices();

        builder.Services.AddServices();

        // *** KEY CHANGE: Set the culture here ***
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = GetCurrentCulture();

        return builder.Build();
    }

    private static MauiAppBuilder AddConfiguration(this MauiAppBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth0:Domain"] = Configuration.Auth0.Domain,
                ["Auth0:Audience"] = Configuration.Auth0.Audience,
                ["Auth0:Authority"] = Configuration.Auth0.Authority,
                ["Auth0:ClientId"] = Configuration.Auth0.ClientId,
                ["DownstreamApi:BaseUrl"] = Configuration.DownstreamApi.BaseUrl,
                ["DownstreamApi:Scopes"] = Configuration.DownstreamApi.Scopes,
                ["Environment"] = Configuration.Environment
            })
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        builder.Services.Configure<Auth0Options>(configuration.GetSection(Auth0Options.Section));
        builder.Services.Configure<DownStreamApiOptions>(configuration.GetSection(DownStreamApiOptions.Section));

        return builder;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            var auth0Config = serviceProvider.GetRequiredService<IOptions<Auth0Options>>().Value;
            var downStreamApiConfig = serviceProvider.GetRequiredService<IOptions<DownStreamApiOptions>>().Value;
            return new Auth0ClientOptions
            {
                Domain = auth0Config.Domain,
                ClientId = auth0Config.ClientId,
                Scope = downStreamApiConfig.Scopes,
                RedirectUri = "myapp://callback",
                PostLogoutRedirectUri = "myapp://callback",
            };
        });
        services.AddSingleton(serviceProvider =>
        {
            var auth0ClientOptions = serviceProvider.GetRequiredService<Auth0ClientOptions>();
            return new Auth0Client(auth0ClientOptions);
        });
        services.AddAuthorizationCore();
        services.AddSingleton<AuthenticationMessageHandler>();
        services.AddSingleton<Auth0AuthenticationStateProvider>();
        services.AddSingleton<AuthenticationStateProvider>(provider =>
            provider.GetRequiredService<Auth0AuthenticationStateProvider>());
        services.AddSingleton<IAuthNavigationManager>(provider =>
            provider.GetRequiredService<Auth0AuthenticationStateProvider>());
        services.AddSingleton<ITokenProvider, SecureStorageTokenProvider>();
        services.AddTokenService();
        services.AddSingleton<IFormFactor, FormFactor>();

        services.AddRefitClients(builder => builder
            .AddHttpMessageHandler(provider => provider.GetRequiredService<AuthenticationMessageHandler>())
            .ConfigurePrimaryHttpMessageHandler(() => new NSUrlSessionHandler()));

        return services;
    }

    private static CultureInfo GetCurrentCulture()
    {
        // Platform-specific code to get the device culture
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // iOS
            var iosLocale = Foundation.NSLocale.CurrentLocale;
            return new CultureInfo(iosLocale.Identifier);
        }

        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            // Mac Catalyst
            var macLocale = Foundation.NSLocale.CurrentLocale; // Same as iOS!
            return new CultureInfo(macLocale.Identifier);
        }

        // Default fallback
        return CultureInfo.CurrentCulture;
    }
}
