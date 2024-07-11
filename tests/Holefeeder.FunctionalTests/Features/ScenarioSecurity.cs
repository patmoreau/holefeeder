using System.Reflection;

using DrifterApps.Seeds.Testing.Infrastructure;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace Holefeeder.FunctionalTests.Features;

[FeatureDescription(@"In order to test the security")]
[ComponentTest]
public partial class FeatureSecurity(ApiApplicationSecurityDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : BaseFeature(apiApplicationDriver, testOutputHelper)
{
    [Scenario]
    [ScenarioCategory("Security")]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public async Task WhenAuthorizedUser(ApiResource endpoint) =>
        await Runner.RunScenarioAsync(
            _ => Given_an_authorized_user(),
            _ => When_I_invoke_the_resource(endpoint),
            _ => Then_user_should_be_authorized_to_access_endpoint()
        );

    [Scenario]
    [ScenarioCategory("Security")]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public async Task WhenForbiddenUser(ApiResource endpoint) =>
        await Runner.RunScenarioAsync(
            _ => Given_a_forbidden_user(),
            _ => When_I_invoke_the_resource(endpoint),
            _ => Then_user_should_be_forbidden_to_access_endpoint()
        );

    [Scenario]
    [ScenarioCategory("Security")]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public async Task WhenUnauthorizedUser(ApiResource endpoint) =>
        await Runner.RunScenarioAsync(
            _ => Given_an_unauthorized_user(),
            _ => When_I_invoke_the_resource(endpoint),
            _ => Then_user_should_not_be_authorized_to_access_endpoint()
        );

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
