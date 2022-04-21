using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holefeeder.FunctionalTests.Infrastructure;

public class MockAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserId { get; set; } = null!;
}

public class MockAuthenticationHandler : AuthenticationHandler<MockAuthenticationHandlerOptions>
{
    public const string TEST_USER_HEADER = nameof(TEST_USER_HEADER);

    public const string AUTHENTICATION_SCHEME = "Test";

    private readonly string _defaultUserId;

    public MockAuthenticationHandler(IOptionsMonitor<MockAuthenticationHandlerOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _defaultUserId = options.CurrentValue.DefaultUserId;
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
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized user"));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Test user"), new(ClaimTypes.NameIdentifier, headerValue.Parameter)
        };

        if (headerValue.Parameter.Equals(_defaultUserId, StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimConstants.Scope, "holefeeder.user"));
        }

        var identity = new ClaimsIdentity(claims, AUTHENTICATION_SCHEME);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AUTHENTICATION_SCHEME);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
