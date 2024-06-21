using System.Diagnostics.CodeAnalysis;

using Hangfire;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static WebApplication UseSwagger(this WebApplication app, IHostEnvironment environment,
        IConfiguration configuration)
    {
        app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{configuration["Swagger:Prefix"]}/swagger/v2/swagger.json",
                    $"{environment.ApplicationName} {environment.EnvironmentName} v2");
                c.OAuthClientId(configuration["AzureAdB2C:SwaggerClientId"]);
                c.OAuthAppName($"{environment.ApplicationName}");
                c.OAuthUsePkce();
            });

        return app;
    }

    public static WebApplication UseHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/ready");
        app.UseHealthChecks("/health/startup");
        app.MapHealthChecks("/healthz",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        app.MapHealthChecksUI(config =>
        {
            config.UIPath = "/hc-ui";
            config.ResourcesPath = "/hc-ui/resources";
            config.ApiPath = "/hc-ui/hc-api";
            config.WebhookPath = "/hc-ui/webhooks";
            config.UseRelativeApiPath = true;
            config.UseRelativeResourcesPath = true;
            config.UseRelativeWebhookPath = true;
        });

        return app;
    }

    public static WebApplication UseHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard();
        app.MapHangfireDashboard();

        return app;
    }
}
