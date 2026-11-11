using Holefeeder.Service.Settings.Api.GetSettings;
using Holefeeder.Service.Settings.Api.SaveSettings;
using Holefeeder.Service.Settings.Core;
using Holefeeder.Service.Settings.Core.GetSettings;
using Holefeeder.Service.Settings.Core.SaveSettings;
using Holefeeder.Service.Settings.Persistence;

namespace Holefeeder.Service.Settings;

internal static class Settings
{
    public static IServiceCollection AddSettingsServices(this IServiceCollection services) =>
        services
            .AddScoped<ISettings, SettingsInDatabase>()
            .AddScoped<GetSettingsUseCase>()
            .AddScoped<SaveSettingsUseCase>();

    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetSettings()
            .MapSaveSettings();
}
