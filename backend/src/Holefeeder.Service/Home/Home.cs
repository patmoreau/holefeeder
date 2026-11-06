using Holefeeder.Service.Home.Api;

namespace Holefeeder.Service.Home;

internal static class Home
{
    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder app) =>
        app.MapHome();
}
