using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string TEST_USER_ID_HEADER = nameof(TEST_USER_ID_HEADER);
        
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var testUserId = BudgetingContextSeed.TestUserGuid1.ToString();
            if (Request.Headers.ContainsKey(TEST_USER_ID_HEADER))
            {
                testUserId = Request.Headers[TEST_USER_ID_HEADER];
            }
            var claims = new[]
            {
                new Claim(ClaimConstants.Name, "Test user"),
                new Claim(ClaimConstants.Sub, testUserId)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            identity.AddClaim(new Claim(ClaimConstants.Scp, Scopes.REGISTERED_USER));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
