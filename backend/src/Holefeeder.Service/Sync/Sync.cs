using Holefeeder.Service.Sync.Api.SyncData;
using Holefeeder.Service.Sync.Core;
using Holefeeder.Service.Sync.Core.SyncData;
using Holefeeder.Service.Sync.Persistence;

namespace Holefeeder.Service.Sync;

internal static class Sync
{
    public static IServiceCollection AddSyncServices(this IServiceCollection services) =>
        services
            .AddScoped<ISyncStore, SyncInDatabase>()
            .AddScoped<SyncDataUseCase>();

    public static IEndpointRouteBuilder MapSyncEndpoints(this IEndpointRouteBuilder app) =>
        app.MapSyncData();
}
