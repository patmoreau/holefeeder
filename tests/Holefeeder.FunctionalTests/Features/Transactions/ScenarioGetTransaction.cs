using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly Faker _faker = new();

    [Fact]
    public Task GettingATransactionWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsATransaction)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingATransactionThatDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(ARequestForATransactionThatDoesNotExist)
            .When(TheUser.GetsATransaction)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task GettingATransactionThatExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(AValidRequest)
            .When(TheUser.GetsATransaction)
            .Then(TheResultShouldBeAsExpected)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => Guid.Empty);

    private void ARequestForATransactionThatDoesNotExist(IStepRunner runner) =>
        runner.Execute(() => _faker.Random.Guid());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Transaction, Guid>(transaction =>
        {
            transaction.Should().BeValid();
            return transaction.Value.Id;
        });

    [AssertionMethod]
    private static void TheResultShouldBeAsExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<TransactionInfoViewModel>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var result = response.Value.Content;

            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            var category = runner.GetContextData<Category>(CategoryContexts.ExistingCategory);
            var transaction = runner.GetContextData<Transaction>(TransactionContexts.ExistingTransaction);
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    Id = (Guid)transaction.Id,
                    transaction.Date,
                    Amount = (decimal)transaction.Amount,
                    transaction.Description,
                    transaction.Tags,
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
