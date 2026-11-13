using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Holefeeder.Service.Extensions;

[ExcludeFromCodeCoverage]
internal static class WebApplicationExtensions
{
    internal static WebApplication UseHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/ready");
        app.UseHealthChecks("/health/startup");
        app.MapHealthChecks("/healthz",
            new HealthCheckOptions
            {
                Predicate = _ => true,
            });

        return app;
    }
}
