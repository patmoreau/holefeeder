using System.Net;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.ServiceTests.Shared.Api;

[Collection(ComponentTestMarker.Name)]
public class AuthorizationComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly Dictionary<string, string> ScopeOfPolicy = new()
    {
        ["ReadUser"] = "read:user",
        ["WriteUser"] = "write:user"
    };

    private static readonly string[] PublicRoutes =
    [
        "/",
        "/healthz",
        "/hangfire/{**path}",
        "/openapi/{documentName}.json",
        "/scalar/{documentName?}",
        "/scalar/favicon.svg",
        "/scalar/scalar.aspnetcore.js",
        "/scalar/scalar.js"
    ];

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheServiceMapsItsRoutes() =>
        ScenarioRunner.Create("Every route requires a user scope or is public", _scenarioOutput)
            .Given("the running service", () => { })
            .When("its routes are listed", RoutesNeitherProtectedNorPublic)
            .Then<IReadOnlyList<string>>("none is left unprotected by mistake", NoRouteIsListed)
            .PlayAsync();

    [Fact]
    public Task WhenNoTokenIsSent() =>
        ScenarioRunner.Create("Calling the protected routes without a token", _scenarioOutput)
            .Given("the running service", () => { })
            .When("each protected route is called without a token", CallingEachProtectedRouteWithoutAToken)
            .Then<IReadOnlyList<RouteCall>>("each answers 401", calls => EachAnswers(calls, HttpStatusCode.Unauthorized))
            .PlayAsync();

    [Fact]
    public Task WhenTheTokenLacksTheRoutesScope() =>
        ScenarioRunner.Create("Calling the protected routes without their scope", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("each protected route is called with every scope but its own", CallingEachProtectedRouteWithoutItsScope)
            .Then<IReadOnlyList<RouteCall>>("each answers 403", calls => EachAnswers(calls, HttpStatusCode.Forbidden))
            .PlayAsync();

    [Fact]
    public Task WhenTheTokenHasTheRoutesScope() =>
        ScenarioRunner.Create("Calling the protected routes with their scope", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("each protected route is called with its own scope only", CallingEachProtectedRouteWithItsScope)
            .Then<IReadOnlyList<RouteCall>>("none is refused", NoneIsRefused)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private IReadOnlyList<string> RoutesNeitherProtectedNorPublic() =>
        MappedRoutes()
            .Where(endpoint => PoliciesOf(endpoint).Length == 0)
            .Select(endpoint => endpoint.RoutePattern.RawText ?? string.Empty)
            .Where(route => !PublicRoutes.Contains(route))
            .Distinct()
            .ToList();

    private Task<IReadOnlyList<RouteCall>> CallingEachProtectedRouteWithoutAToken() =>
        CallEachProtectedRouteAsync(_ => server.CreateClient());

    private Task<IReadOnlyList<RouteCall>> CallingEachProtectedRouteWithoutItsScope() =>
        CallEachProtectedRouteAsync(route => server.CreateClientFor(_user.IdentityObjectId,
            string.Join(' ', ScopeOfPolicy.Values.Except(route.Scopes))));

    private Task<IReadOnlyList<RouteCall>> CallingEachProtectedRouteWithItsScope() =>
        CallEachProtectedRouteAsync(route => server.CreateClientFor(_user.IdentityObjectId, string.Join(' ', route.Scopes)));

    private async Task<IReadOnlyList<RouteCall>> CallEachProtectedRouteAsync(Func<ProtectedRoute, HttpClient> clientFor)
    {
        var calls = new List<RouteCall>();
        foreach (var route in ProtectedRoutes())
        {
            using var client = clientFor(route);
            using var request = new HttpRequestMessage(new HttpMethod(route.Method), route.Url);
            using var response = await client.SendAsync(request);
            calls.Add(new RouteCall(route, response.StatusCode));
        }

        return calls;
    }

    private static void NoRouteIsListed(Ensure<IReadOnlyList<string>> routes)
    {
        routes.Should().BeValid();
        routes.Value.Should().BeEmpty("a route must require a user scope or be listed in {0}", nameof(PublicRoutes));
    }

    private static void EachAnswers(Ensure<IReadOnlyList<RouteCall>> calls, HttpStatusCode expected)
    {
        calls.Should().BeValid();
        calls.Value.Should().NotBeEmpty();
        calls.Value.Where(call => call.StatusCode != expected).Select(call => call.ToString()).Should().BeEmpty();
    }

    private static void NoneIsRefused(Ensure<IReadOnlyList<RouteCall>> calls)
    {
        calls.Should().BeValid();
        calls.Value.Should().NotBeEmpty();
        calls.Value
            .Where(call => call.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            .Select(call => call.ToString())
            .Should().BeEmpty();
    }

    private IEnumerable<RouteEndpoint> MappedRoutes() =>
        server.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>();

    private IEnumerable<ProtectedRoute> ProtectedRoutes() =>
        from endpoint in MappedRoutes()
        let policies = PoliciesOf(endpoint)
        where policies.Length > 0
        from method in endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? [HttpMethods.Get]
        select new ProtectedRoute(method, UrlFor(endpoint.RoutePattern), policies.Select(policy => ScopeOfPolicy[policy]).ToList());

    private static string[] PoliciesOf(Endpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>()
            .Select(data => data.Policy)
            .OfType<string>()
            .ToArray();

    private static Uri UrlFor(RoutePattern pattern) =>
        new("/" + string.Join('/', pattern.PathSegments.Select(segment => string.Concat(segment.Parts.Select(ValueFor)))),
            UriKind.Relative);

    private static string ValueFor(RoutePatternPart part) =>
        part switch
        {
            RoutePatternLiteralPart literal => literal.Content,
            RoutePatternSeparatorPart separator => separator.Content,
            RoutePatternParameterPart parameter when HasConstraint(parameter, "guid") => Guid.NewGuid().ToString(),
            RoutePatternParameterPart parameter when HasConstraint(parameter, "int") || HasConstraint(parameter, "long") => "1",
            _ => "value"
        };

    private static bool HasConstraint(RoutePatternParameterPart parameter, string constraint) =>
        parameter.ParameterPolicies.Any(policy => policy.Content == constraint);

    private sealed record ProtectedRoute(string Method, Uri Url, IReadOnlyList<string> Scopes);

    private sealed record RouteCall(ProtectedRoute Route, HttpStatusCode StatusCode)
    {
        public override string ToString() => $"{Route.Method} {Route.Url} answered {(int)StatusCode}";
    }
}
