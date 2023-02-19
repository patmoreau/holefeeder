using System.Diagnostics.CodeAnalysis;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Serilog;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static WebApplication MapSwagger(this WebApplication app, IHostEnvironment environment)
    {
        app
            .UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", $"{environment.ApplicationName} v2");
                c.OAuthClientId("9814ecda-b8db-4775-a361-714af29fe486");
                c.OAuthAppName($"{environment.ApplicationName}");
                c.OAuthScopeSeparator(" ");
                c.OAuthUsePkce();
            });

        return app;
    }

    public static WebApplication MapHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/healthz",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }

    public static WebApplication UseSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}
