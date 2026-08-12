using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Users.Commands;
using Holefeeder.Application.Features.Users.Queries;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

using Microsoft.EntityFrameworkCore;

using Refit;

// The scenario base class exposes a Category steps property, which would otherwise win.
using DomainCategory = Holefeeder.Domain.Features.Categories.Category;

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
    public Task RegisteringSeedsTheTransferCategories() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUnregisteredUser.Registers)
            .Then(TheCallerShouldOwnTheTransferCategories)
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
    private void TheCallerShouldOwnTheTransferCategories(IStepRunner runner) =>
        runner.Execute<IApiResponse<RegisterUser.Response>, Task>(async response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();

            var userId = (UserId)response.Value.Content!.Id;

            await using var dbContext = DatabaseDriver.CreateDbContext();

            var categories = await dbContext.Categories
                .Where(category => category.UserId == userId)
                .ToListAsync();

            categories.Should().HaveCount(2)
                .And.AllSatisfy(category =>
                {
                    category.System.Should().BeTrue();
                    category.Favorite.Should().BeFalse();
                    category.Inactive.Should().BeFalse();
                    category.BudgetAmount.Should().Be(Money.Zero);
                });
            categories.Should().ContainSingle(category => category.Name == DomainCategory.TransferOutName)
                .Which.Type.Should().Be(CategoryType.Expense);
            categories.Should().ContainSingle(category => category.Name == DomainCategory.TransferInName)
                .Which.Type.Should().Be(CategoryType.Gain);
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
