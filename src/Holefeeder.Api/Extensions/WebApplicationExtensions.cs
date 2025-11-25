using System.Diagnostics.CodeAnalysis;

using Hangfire;
using Hangfire.Dashboard;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class WebApplicationExtensions
{
    internal static WebApplication UseSwagger(this WebApplication app, IHostEnvironment environment,
        IConfiguration configuration)
    {
        app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{configuration[Constants.ProxyPrefix]}/swagger/v2/swagger.json",
                    $"{environment.ApplicationName} {environment.EnvironmentName} v2");
                c.OAuthAppName($"{environment.ApplicationName}");
                c.OAuthClientId("YOUR_AUTH0_CLIENT_ID");
                c.OAuthClientSecret("YOUR_AUTH0_CLIENT_SECRET");
                c.OAuthUsePkce();
            });

        return app;
    }

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

    internal static WebApplication UseHangfire(this WebApplication app, IConfiguration configuration)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
        {
            PrefixPath = configuration[Constants.ProxyPrefix],
            Authorization =
            [
                new HangFireAuthorizationFilter()
            ]
        });
        app.MapHangfireDashboard();

        return app;
    }


    private class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context) =>
            // var httpCtx = context.GetHttpContext();
            // return httpCtx.User.Identity.IsAuthenticated; //is always false
            true;
    }
}
