using System.Diagnostics.CodeAnalysis;

using Hangfire;
using Hangfire.Dashboard;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Holefeeder.Api.Extensions;

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
