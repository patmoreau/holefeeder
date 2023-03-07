using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace Holefeeder.FunctionalTests.Features;

[FeatureDescription(@"In order to test the security")] //feature description
public partial class FeatureSecurity : BaseFeature
{
    public FeatureSecurity(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
    }

    [Scenario]
    [ScenarioCategory("Security")]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public async Task WhenAuthorizedUser(ApiResource endpoint)
    {
        await Runner.RunScenarioAsync(
            _ => Given_an_authorized_user(),
            _ => When_I_invoke_the_resource(endpoint),
            _ => Then_user_should_be_authorized_to_access_endpoint()
        );
    }

    [Scenario]
    [ScenarioCategory("Security")]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public async Task WhenForbiddenUser(ApiResource endpoint)
    {
        await Runner.RunScenarioAsync(
            _ => Given_a_forbidden_user(),
            _ => When_I_invoke_the_resource(endpoint),
            _ => Then_user_should_be_forbidden_to_access_endpoint()
        );
    }

    [Scenario]
    [ScenarioCategory("Security")]
    [MemberData(nameof(SecuredEndpointTestCases))]
    public async Task WhenUnauthorizedUser(ApiResource endpoint)
    {
        await Runner.RunScenarioAsync(
            _ => Given_an_unauthorized_user(),
            _ => When_I_invoke_the_resource(endpoint),
            _ => Then_user_should_not_be_authorized_to_access_endpoint()
        );
    }

    public static IEnumerable<object[]> SecuredEndpointTestCases
    {
        get { return ApiResource.List.Where(x => !x.IsOpen).Select(endpoint => new[] { endpoint }); }
    }
}
