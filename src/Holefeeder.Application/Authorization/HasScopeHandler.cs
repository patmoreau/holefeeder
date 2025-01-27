using Holefeeder.Application.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Authorization;

internal class HasScopeHandler(ILogger<HasScopeHandler> logger) : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        logger.LogUserClaims(context.User.Claims.Select(c => (c.Type, c.Value, c.Issuer)));

        if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
        {
            logger.LogHasNoScopeForIssuer(requirement);
            return Task.CompletedTask;
        }

        var scopes = context.User
            .FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer)?.Value.Split(' ')
            .ToArray() ?? [];

        if (Array.Exists(scopes, s => s == requirement.Scope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
