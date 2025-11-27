using System.Net.Http.Headers;

using DrifterApps.Seeds.Testing.Infrastructure.Authentication;

using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Holefeeder.FunctionalTests.Drivers;

public class ApiApplicationSecurityDriver() : ApiApplicationDriver(false)
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("SECURITY_TESTS");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, TestAuthResultHandler>();
            services.AddSingleton<IForbiddenUser>(_ =>
            {
                var httpClient = CreateClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", new JwtTokenBuilder()
                        .IssuedBy($"{AuthorityDriver.Authority}")
                        .ForAudience("https://holefeeder-api.drifterapps.app")
                        .Build());
                return RestService.For<IForbiddenUser>(httpClient);
            });
            services.AddSingleton<IUnauthenticatedUser>(_ =>
            {
                var httpClient = CreateClient();
                return RestService.For<IUnauthenticatedUser>(httpClient);
            });
        });
    }
}
