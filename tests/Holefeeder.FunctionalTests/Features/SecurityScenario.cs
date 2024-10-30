using System.Net;

using DrifterApps.Seeds.FluentScenario;

using Refit;

namespace Holefeeder.FunctionalTests.Features;

[ComponentTest, Category("Authorization")]
[Collection(FunctionalSecurityTestMarker.Name)]
public abstract class SecurityScenario(ITestOutputHelper testOutputHelper) : BaseScenario(testOutputHelper)
{
    protected Task ScenarioForAuthorizedUser(string scenarioFor, Action<IStepRunner> authorizedAction) =>
        ScenarioRunner.Create($"authorized users should be able to attempt to {scenarioFor}", ScenarioOutput)
            .When(authorizedAction)
            .Then(ShouldBeAllowedToAccessEndpoint)
            .PlayAsync();

    protected Task ScenarioForUnauthenticatedUser(string scenarioFor, Action<IStepRunner> authorizedAction) =>
        ScenarioRunner.Create($"unauthenticated users should be not be able to attempt to {scenarioFor}", ScenarioOutput)
            .When(authorizedAction)
            .Then(ShouldBeUnauthorizedToAccessEndpoint)
            .PlayAsync();

    protected Task ScenarioForForbiddenUser(string scenarioFor, Action<IStepRunner> forbiddenAction) =>
        ScenarioRunner.Create($"forbidden users should not be able to attempt to {scenarioFor}", ScenarioOutput)
            .When(forbiddenAction)
            .Then(ShouldBeForbiddenToAccessEndpoint)
            .PlayAsync();

    private static void ShouldBeAllowedToAccessEndpoint(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the user is authorized to access endpoint", response =>
            response.Should().BeValid()
                .And.Subject.Value.Should().NotHaveStatusCode(HttpStatusCode.Forbidden)
                .And.NotHaveStatusCode(HttpStatusCode.Unauthorized));

    private static void ShouldBeUnauthorizedToAccessEndpoint(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the user is not authorized to access endpoint", response =>
            response.Should().BeValid()
                .And.Subject.Value.Should().BeFailure()
                .And.HaveStatusCode(HttpStatusCode.Unauthorized));

    private static void ShouldBeForbiddenToAccessEndpoint(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the user is forbidden to access endpoint", response =>
            response.Should().BeValid()
                .And.Subject.Value.Should().BeFailure()
                .And.HaveStatusCode(HttpStatusCode.Forbidden));
}
