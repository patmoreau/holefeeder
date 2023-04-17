using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holefeeder.Tests.Common.SeedWork.Infrastructure;

public sealed class MockAuthenticationHandler : AuthenticationHandler<MockAuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";

    public MockAuthenticationHandler(IOptionsMonitor<MockAuthenticationSchemeOptions> options, ILoggerFactory logger,
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

        string userId = headerValue.Parameter;

        List<Claim> claims = new() { new(ClaimTypes.Name, userId), new(ClaimTypes.NameIdentifier, userId) };

        claims.AddRange(Options.ConfigureUserClaims(userId));

        ClaimsIdentity identity = new(claims, AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
