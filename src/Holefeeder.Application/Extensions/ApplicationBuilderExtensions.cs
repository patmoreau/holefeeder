using System.Diagnostics.CodeAnalysis;

using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;

namespace Holefeeder.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseUserContext(this IApplicationBuilder app)
    {
        app.UseMiddleware<UserContextMiddleware>();
        return app;
    }
}

