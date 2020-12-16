using System;
using System.Security.Claims;
using DrifterApps.Holefeeder.Framework.SeedWork;

using Microsoft.Identity.Web;

namespace DrifterApps.Holefeeder.Budgeting.API.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUniqueId(this ClaimsPrincipal self)
        {
            self.ThrowIfNull(nameof(self));

            var value = self.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value))
            {
                value = self.FindFirstValue(ClaimConstants.Sub);
            }

            return string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
        }
    }
}
