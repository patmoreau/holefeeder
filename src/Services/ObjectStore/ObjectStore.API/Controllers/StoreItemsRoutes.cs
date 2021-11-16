using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.ObjectStore.API.Controllers;

public static class StoreItemsRoutes
{
    public static WebApplication AddStoreItemsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/store-items";

        app.MapGet($"{routePrefix}", DispatchRequest.QueryAsync<GetStoreItems.Request>)
            .WithName(nameof(GetStoreItems))
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}", DispatchRequest.WithIdAsync<GetStoreItem.Request>)
            .WithName(nameof(GetStoreItem))
            .AddOptions();

        app.MapPost($"{routePrefix}/create-store-item",
                DispatchRequest.CreateAsync<CreateStoreItem.Request>)
            .WithName(nameof(CreateStoreItem))
            .AddOptions();

        app.MapPost($"{routePrefix}/modify-store-item", DispatchRequest.ModifyAsync<ModifyStoreItem.Request>)
            .WithName(nameof(ModifyStoreItem))
            .AddOptions();

        return app;
    }

    private static void AddOptions(this RouteHandlerBuilder builder)
    {
        builder
            .WithTags("Store Items")
            .RequireAuthorization()
            .WithGroupName("v2");
    }
}
