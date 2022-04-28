using Holefeeder.Web.Config;

namespace Holefeeder.Web.Controllers;

public static class ConfigRoutes
{
    public static void AddConfigRoutes(this WebApplication app)
    {
        const string routePrefix = "config";

        app.MapGet($"{routePrefix}", (AngularSettings settings) => new {settings.LoggingLevel});
    }
}
