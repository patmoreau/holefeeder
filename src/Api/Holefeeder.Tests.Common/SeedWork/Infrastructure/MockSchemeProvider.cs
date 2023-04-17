using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Holefeeder.Tests.Common.SeedWork.Infrastructure;

public sealed class MockSchemeProvider : AuthenticationSchemeProvider
{
    public MockSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    public MockSchemeProvider(IOptions<AuthenticationOptions> options,
        IDictionary<string, AuthenticationScheme> schemes) : base(options, schemes)
    {
    }

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        switch (name)
        {
            case MockAuthenticationHandler.AuthenticationScheme:
                {
                    AuthenticationScheme scheme = new(
                        MockAuthenticationHandler.AuthenticationScheme,
                        MockAuthenticationHandler.AuthenticationScheme,
                        typeof(MockAuthenticationHandler));
                    return Task.FromResult<AuthenticationScheme?>(scheme);
                }
            default:
                return base.GetSchemeAsync(name);
        }
    }
}
