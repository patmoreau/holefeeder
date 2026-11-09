using Holefeeder.Service.StoreItems.Api.SearchStoreItems;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.StoreItems.Persistence;

namespace Holefeeder.Service.StoreItems;

internal static class StoreItems
{
    public static IServiceCollection AddStoreItemsServices(this IServiceCollection services) =>
        services
            .AddScoped<IStoreItems, StoreItemsInDatabase>()
            .AddScoped<SearchStoreItemsUseCase>();

    public static IEndpointRouteBuilder MapStoreItemsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapSearchStoreItems();
}
