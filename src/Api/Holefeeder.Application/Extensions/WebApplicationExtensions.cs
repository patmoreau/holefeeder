using Microsoft.AspNetCore.Builder;

namespace Holefeeder.Application.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapCore(this WebApplication app)
    {
        //app.MapGraphQL(new PathString("/graphql"));

        return app;
    }

}
