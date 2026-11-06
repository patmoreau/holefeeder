using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.ServiceTests.Shared.TestKit;

internal static class HttpContextForTest
{
    public static HttpContext AHttpContext(string? nameIdentifier = null)
    {
        var id = nameIdentifier ?? AnIdentityObjectId().Value;
        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim("sub", id)
            ]))
        };
    }

    public static HttpContext ASubHttpContext(string? sub = null)
    {
        var id = sub ?? AnIdentityObjectId().Value;
        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("sub", id)
            ]))
        };
    }

    public static HttpContext AnEmptyHttpContext() => new DefaultHttpContext {User = new ClaimsPrincipal(new ClaimsIdentity())};
}
