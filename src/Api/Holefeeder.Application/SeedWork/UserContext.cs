using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

namespace Holefeeder.Application.SeedWork;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => GetUserId();

    private Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is null)
        {
            return Guid.Empty;
        }

        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(value))
        {
            value = user.FindFirstValue(ClaimConstants.Sub);
        }

        return string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
    }
}
