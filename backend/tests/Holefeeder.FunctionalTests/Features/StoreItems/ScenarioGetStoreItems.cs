using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioGetStoreItems(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task GettingStoreItemsWithInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsItemsInTheStore)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingStoreItemsSortedByCodeDesc() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.CollectionExists)
            .And(ARequestSortedByCodeDesc)
            .When(TheUser.GetsItemsInTheStore)
            .Then(ShouldReceiveItemsInProperOrder)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => new GetStoreItems.Request(-1, 10, [], []));

    private static void ARequestSortedByCodeDesc(IStepRunner runner) => runner.Execute(() => new GetStoreItems.Request(0, 10, ["-code"], []));

    [AssertionMethod]
    private static void ShouldReceiveItemsInProperOrder(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<StoreItemViewModel>>>(response =>
        {
            var expected = runner.GetContextData<IEnumerable<StoreItem>>(StoreItemContext.ExistingStoreItems);
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful();
            var accounts = response.Value;
            accounts.Content.Should().HaveSameCount(expected).And.BeInDescendingOrder(x => x.Code);
        });
}
