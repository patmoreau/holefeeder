using System.Reflection;

using DrifterApps.Seeds.Testing.Infrastructure;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api Security collection")]
[ComponentTest]
[Category("Security")]
public partial class FeatureSecurity(ApiApplicationSecurityDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : SecurityScenario(apiApplicationDriver, testOutputHelper)
{
    [Theory]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public Task WhenAuthorizedUser(ApiResource endpoint) =>
        ScenarioFor("an authorized user", runner =>
            runner.Given(User.IsAuthorized)
                .When(stepRunner => TheResourceIsInvoked(stepRunner, endpoint))
                .Then(UserShouldBeAuthorizedToAccessEndpoint));

    [Theory]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public Task WhenForbiddenUser(ApiResource endpoint) =>
        ScenarioFor("a forbidden user", runner =>
            runner.Given(User.IsForbidden)
                .When(stepRunner => TheResourceIsInvoked(stepRunner, endpoint))
                .Then(UserShouldBeForbiddenToAccessEndpoint));

    [Theory]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public Task WhenUnauthorizedUser(ApiResource endpoint) =>
        ScenarioFor("an unauthorized user", runner =>
            runner.Given(User.IsUnauthorized)
                .When(stepRunner => TheResourceIsInvoked(stepRunner, endpoint))
                .Then(UserShouldNotBeAuthorizedToAccessEndpoint));

    public static IEnumerable<object[]> SecuredEndpointTestCases
    {
        get
        {
            var apiResources = typeof(ApiResources).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(ApiResource) && !((ApiResource)f.GetValue(null)!).IsOpen)
                .Select(f => (ApiResource)f.GetValue(null)!)
                .ToList();

            foreach (var apiResource in apiResources)
            {
                yield return [apiResource];
            }
        }
    }
}
