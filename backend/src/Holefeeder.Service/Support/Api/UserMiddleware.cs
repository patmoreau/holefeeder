using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Support.Api;

internal class UserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, UserContextAccessor userContextAccessor, IUsers users)
    {
        var identityObjectId = IdentityObjectId.Create(httpContext.User);
        if (identityObjectId.IsFailure)
        {
            userContextAccessor.Set(User.NoUser);
        }
        else
        {
            var userResult = await users.GetByIdentityObjectId(identityObjectId.Value, httpContext.RequestAborted);
            if (userResult.IsSuccess)
            {
                userContextAccessor.Set(userResult.Value);
            }
        }

        await next(httpContext);
    }
}
