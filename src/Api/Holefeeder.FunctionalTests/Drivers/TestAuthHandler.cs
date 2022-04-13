using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holefeeder.FunctionalTests.Drivers;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TEST_USER_ID_HEADER = nameof(TEST_USER_ID_HEADER);

    public static readonly Guid AuthenticatedUserId = new("B80B9954-3EE0-4BB0-80DA-FA202744323E");

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult result;

        if (Request.Headers.ContainsKey(TEST_USER_ID_HEADER))
        {
            var testUserId = Request.Headers[TEST_USER_ID_HEADER];

            var claims = new[] {new Claim(ClaimConstants.Name, "Test user"), new Claim(ClaimConstants.Sub, testUserId)};
            var identity = new ClaimsIdentity(claims, "Test");
            // identity.AddClaim(new Claim(ClaimConstants.Scp, Scopes.REGISTERED_USER));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            result = AuthenticateResult.Success(ticket);
        }
        else
        {
            result = AuthenticateResult.Fail("Unauthenticated user");
        }
        return Task.FromResult(result);
    }
}
