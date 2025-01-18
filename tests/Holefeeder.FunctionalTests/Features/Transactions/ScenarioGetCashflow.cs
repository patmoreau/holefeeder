using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetCashflow(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly Faker _faker = new();

    [Fact]
    public Task GettingACashflowWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsACashflow)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingACashflowThatDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(ARequestForACashflowThatDoesNotExist)
            .When(TheUser.GetsACashflow)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task GettingACashflowThatExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.Exists)
            .And(AValidRequest)
            .When(TheUser.GetsACashflow)
            .Then(TheResultShouldBeAsExpected)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => Guid.Empty);

    private void ARequestForACashflowThatDoesNotExist(IStepRunner runner) =>
        runner.Execute(() => _faker.Random.Guid());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Cashflow, Guid>(cashflow =>
        {
            cashflow.Should().BeValid();
            return cashflow.Value.Id;
        });

    [AssertionMethod]
    private static void TheResultShouldBeAsExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<CashflowInfoViewModel>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var result = response.Value.Content;

            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            var category = runner.GetContextData<Category>(CategoryContexts.ExistingCategory);
            var cashflow = runner.GetContextData<Cashflow>(CashflowContexts.ExistingCashflow);
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    Id = (Guid)cashflow.Id,
                    cashflow.EffectiveDate,
                    Amount = (decimal)cashflow.Amount,
                    cashflow.IntervalType,
                    cashflow.Frequency,
                    cashflow.Recurrence,
                    cashflow.Description,
                    cashflow.Inactive,
                    cashflow.Tags,
                    Category = new
                    {
                        Id = (Guid)category.Id,
                        category.Name,
                        category.Type,
                        Color = (string)category.Color
                    },
                    Account = new
                    {
                        Id = (Guid)account.Id,
                        account.Name
                    }
                });
        });
}
