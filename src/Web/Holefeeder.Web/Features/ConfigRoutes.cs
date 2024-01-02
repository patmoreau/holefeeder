using Carter;

using Holefeeder.Web.Config;

namespace Holefeeder.Web.Features;

// ReSharper disable once UnusedType.Global
public class ConfigRoutes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string routePrefix = "config";

        app.MapGet($"{routePrefix}", (AngularSettings settings) => new { settings.LoggingLevel });
    }
}
