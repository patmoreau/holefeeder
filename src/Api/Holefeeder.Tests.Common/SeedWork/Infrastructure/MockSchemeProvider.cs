using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Holefeeder.Tests.Common.SeedWork.Infrastructure;

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
        if (name == MockAuthenticationHandler.AuthenticationScheme)
        {
            AuthenticationScheme scheme = new AuthenticationScheme(
                MockAuthenticationHandler.AuthenticationScheme,
                MockAuthenticationHandler.AuthenticationScheme,
                typeof(MockAuthenticationHandler));
            return Task.FromResult<AuthenticationScheme?>(scheme);
        }

        return base.GetSchemeAsync(name);
    }
}
