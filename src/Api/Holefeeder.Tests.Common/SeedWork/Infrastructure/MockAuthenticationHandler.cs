using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holefeeder.Tests.Common.SeedWork.Infrastructure;

public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";

    public static readonly Guid AuthorizedUserId = Guid.NewGuid();
    public static readonly Guid ForbiddenUserId = Guid.NewGuid();

    public MockAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!AuthenticationHeaderValue.TryParse(Context.Request.Headers["Authorization"],
                out AuthenticationHeaderValue? headerValue))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!AuthenticationScheme.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (headerValue.Parameter is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Test user"), new(ClaimTypes.NameIdentifier, headerValue.Parameter)
        };

        if (headerValue.Parameter.Equals(AuthorizedUserId.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimConstants.Scope, "holefeeder.user"));
        }

        ClaimsIdentity identity = new(claims, AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
