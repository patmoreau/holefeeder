using Carter;
using Holefeeder.Web.Config;

namespace Holefeeder.Web.Features;

public class ConfigRoutes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string routePrefix = "config";

        app.MapGet($"{routePrefix}", (AngularSettings settings) => new { settings.LoggingLevel });
    }
}
