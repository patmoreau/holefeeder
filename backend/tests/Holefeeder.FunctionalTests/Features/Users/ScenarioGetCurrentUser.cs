using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Users.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Users;

public class ScenarioGetCurrentUser(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task GettingTheCurrentUserWhenRegistered() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUser.GetsTheCurrentUser)
            .Then(TheResultShouldBeTheRegisteredUser)
            .PlayAsync();

    [Fact]
    public Task GettingTheCurrentUserWhenNotRegistered() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUnregisteredUser.GetsTheCurrentUser)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [AssertionMethod]
    private static void TheResultShouldBeTheRegisteredUser(IStepRunner runner) =>
        runner.Execute<IApiResponse<GetCurrentUser.Response>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();

            var expected = UserSteps.TestUsers[UserSteps.AuthorizedUser].UserId;
            response.Value.Content.Should().NotBeNull().And.BeEquivalentTo(new { Id = (Guid) expected });
        });
}
