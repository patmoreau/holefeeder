using Holefeeder.Service.Support.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Support.Persistence;

namespace Holefeeder.Service.Support;

internal static class Support
{
    public static IApplicationBuilder UseSupportMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<UserMiddleware>();
        return app;
    }

    public static IServiceCollection AddSupportServices(this IServiceCollection services) =>
        services
            .AddScoped<UserContextAccessor>()
            .AddScoped<IUsers, UsersInDatabase>();

}
