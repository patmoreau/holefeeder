using System;
using System.Security.Claims;

using Microsoft.Identity.Web;

namespace DrifterApps.Holefeeder.ObjectStore.API.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUniqueId(this ClaimsPrincipal self)
    {
        if (self is null)
        {
            throw new ArgumentNullException(nameof(self));
        }

        var value = self.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(value))
        {
            value = self.FindFirstValue(ClaimConstants.Sub);
        }

        return string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
    }
}
