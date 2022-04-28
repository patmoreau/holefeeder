using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Holefeeder.FunctionalTests.Infrastructure;

public class MockSchemeProvider : AuthenticationSchemeProvider
{
    public MockSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    protected MockSchemeProvider(IOptions<AuthenticationOptions> options,
        IDictionary<string, AuthenticationScheme> schemes) : base(options, schemes)
    {
    }

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        if (name == MockAuthenticationHandler.AUTHENTICATION_SCHEME)
        {
            var scheme = new AuthenticationScheme(
                MockAuthenticationHandler.AUTHENTICATION_SCHEME,
                MockAuthenticationHandler.AUTHENTICATION_SCHEME,
                typeof(MockAuthenticationHandler));
            return Task.FromResult<AuthenticationScheme?>(scheme);
        }

        return base.GetSchemeAsync(name);
    }
}
