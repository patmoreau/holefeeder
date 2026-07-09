using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.UserContext;

internal class UserContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IHttpUserContext httpUserContext,
        BudgetingContext context, IUserContext userContext)
    {
        if (userContext is UserContext concreteUserContext)
        {
            await concreteUserContext.InitializeAsync(httpUserContext, context);
        }

        await next(httpContext);
    }
}

