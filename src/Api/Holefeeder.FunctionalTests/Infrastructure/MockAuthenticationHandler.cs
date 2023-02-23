using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holefeeder.FunctionalTests.Infrastructure;

public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AUTHENTICATION_SCHEME = "Test";

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

        if (!AUTHENTICATION_SCHEME.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
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

        ClaimsIdentity identity = new ClaimsIdentity(claims, AUTHENTICATION_SCHEME);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, AUTHENTICATION_SCHEME);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
