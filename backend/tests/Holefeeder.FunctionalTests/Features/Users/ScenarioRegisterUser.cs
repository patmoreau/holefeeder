using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Users.Commands;
using Holefeeder.Application.Features.Users.Queries;
using Holefeeder.Domain.Features.Users;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

using Microsoft.EntityFrameworkCore;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Users;

public class ScenarioRegisterUser(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task RegisteringACallerWhoIsNotRegisteredYet() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUnregisteredUser.Registers)
            .Then(TheCallerShouldOwnTheirIdentity)
            .PlayAsync();

    [Fact]
    public Task RegisteringMakesTheCallerKnown() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(TheUnregisteredUser.Registers)
            .When(TheUnregisteredUser.GetsTheCurrentUser)
            .Then(TheCurrentUserShouldBeFound)
            .PlayAsync();

    [Fact]
    public Task RegisteringACallerWhoIsAlreadyRegistered() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUser.Registers)
            .Then(ShouldExpectBadRequest)
            .PlayAsync();

    [AssertionMethod]
    private void TheCallerShouldOwnTheirIdentity(IStepRunner runner) =>
        runner.Execute<IApiResponse<RegisterUser.Response>, Task>(async response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();

            var id = response.Value.Content!.Id;
            id.Should().NotBeEmpty();

            await using var dbContext = DatabaseDriver.CreateDbContext();

            var user = await dbContext.Users
                .Include(entity => entity.UserIdentities)
                .SingleOrDefaultAsync(entity => entity.Id == (UserId)id);

            user.Should().NotBeNull();
            user.Inactive.Should().BeFalse();
            user.UserIdentities.Should().ContainSingle()
                .Which.IdentityObjectId.Should()
                .Be(UserSteps.TestUsers[UserSteps.UnregisteredUser].IdentityObjectId);
        });

    [AssertionMethod]
    private static void TheCurrentUserShouldBeFound(IStepRunner runner) =>
        runner.Execute<IApiResponse<GetCurrentUser.Response>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();

            response.Value.Content!.Id.Should().NotBeEmpty();
        });
}
