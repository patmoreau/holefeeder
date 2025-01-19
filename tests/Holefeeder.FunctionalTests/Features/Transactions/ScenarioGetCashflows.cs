using System.Net;

using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Refit;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetCashflows(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task GettingCashflowsWithInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsCashflows)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingCashflowsSortedByDescriptionDesc() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.CollectionExists)
            .And(ARequestSortedByDescriptionDesc)
            .When(TheUser.GetsCashflows)
            .Then(ShouldReceiveItemsInProperOrder)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => new GetCashflows.Request(-1, 10, [], []));

    private static void ARequestSortedByDescriptionDesc(IStepRunner runner) => runner.Execute(() => new GetCashflows.Request(0, 10, ["-description"], []));

    [AssertionMethod]
    private static void ShouldReceiveItemsInProperOrder(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<CashflowInfoViewModel>>>(response =>
        {
            var expected = runner.GetContextData<IEnumerable<Cashflow>>(CashflowContext.ExistingCashflows);
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful();
            var accounts = response.Value;
            accounts.Content.Should()
                .BeEquivalentTo(expected.OrderByDescending(x => x.Description),
                    options => options.WithStrictOrdering().ExcludingMissingMembers());
        });
}
