using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class GetAccountScenario(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly Faker _faker = new();

    [Fact]
    public Task GettingAnAccountWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsAnAccount)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingAnAccountThatDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(ARequestForAnAccountThatDoesNotExist)
            .When(TheUser.GetsAnAccount)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task GettingAnAccountWithExpenses() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.ExistsWithExpenses)
            .When(TheUser.GetsAnAccount)
            .Then(TheResultShouldBeAsExpected)
            .PlayAsync();

    [Fact]
    public Task GettingAnAccountWithGains() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.ExistsWithGains)
            .When(TheUser.GetsAnAccount)
            .Then(TheResultShouldBeAsExpected)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => Guid.Empty);

    private void ARequestForAnAccountThatDoesNotExist(IStepRunner runner) =>
        runner.Execute(() => _faker.Random.Guid());

    [AssertionMethod]
    private static void TheResultShouldBeAsExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<AccountViewModel>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var result = response.Value.Content;

            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            var transaction = runner.GetContextData<Transaction>(TransactionContext.ExistingTransaction);
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    Id = (Guid) account.Id,
                    account.Type,
                    account.Name,
                    OpenBalance = (decimal) account.OpenBalance,
                    account.OpenDate,
                    TransactionCount = 1,
                    Balance = decimal.Add(account.OpenBalance, transaction.Amount * category.Type.Multiplier),
                    Updated = transaction.Date,
                    account.Description,
                    account.Favorite,
                    account.Inactive
                });
        });
}
