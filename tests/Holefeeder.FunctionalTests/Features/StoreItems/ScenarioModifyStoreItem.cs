using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioModifyStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.ModifiesAnItemInTheStore)
            .Then(ShouldExpectBadRequest)
            .PlayAsync();

    [Fact]
    public Task WhenModifyStoreItem() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.Exists)
            .And(AValidRequest)
            .When(TheUser.ModifiesAnItemInTheStore)
            .Then(StoreItem.ShouldBeModified)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAModifyStoreItemRequest().WithId(StoreItemId.Empty).Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<StoreItem, ModifyStoreItem.Request>(item =>
        {
            item.Should().BeValid();
            var request = GivenAModifyStoreItemRequest().WithId(item.Value.Id).Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
