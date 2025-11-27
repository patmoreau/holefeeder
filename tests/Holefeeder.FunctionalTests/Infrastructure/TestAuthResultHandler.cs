using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Holefeeder.FunctionalTests.Infrastructure;

public class TestAuthResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        // If Authorization FAILED (401 or 403), let the default handler return the error
        if (!authorizeResult.Succeeded)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        // If Authorization SUCCEEDED, short-circuit!
        // We write a specific response and DO NOT call 'next(context)'
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Auth Success - Endpoint Not Called");
    }
}
