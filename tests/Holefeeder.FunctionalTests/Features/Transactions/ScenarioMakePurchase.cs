using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Transactions.MakePurchaseRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioMakePurchase(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task AUserMakesAnInvalidPurchase() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.MakesAPurchase)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task AUserMakesAValidPurchase() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(AValidRequest)
            .When(TheUser.MakesAPurchase)
            .Then(Transaction.ShouldBeCreated)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAPurchase().WithNoDate().Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            var request = GivenAPurchase()
                .ForAccount(account)
                .ForCategory(category)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
