using System.Net;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioCreateStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.CreatesAnItemInTheStore)
            .Then(ShouldExpectBadRequest)
            .PlayAsync();

    [Fact]
    public Task WhenCreateStoreItem() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AValidRequest)
            .When(TheUser.CreatesAnItemInTheStore)
            .Then(ShouldExpectStatusCodeCreated)
            .And(StoreItem.ShouldBeCreated)
            .PlayAsync();

    [Fact]
    public Task WhenCodeAlreadyExist() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.Exists)
            .And(ARequestWithExistingCode)
            .When(TheUser.CreatesAnItemInTheStore)
            .Then(ShouldExpectBadRequest)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenACreateStoreItemRequest().WithCode(string.Empty).Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenACreateStoreItemRequest().Build());

    private static void ARequestWithExistingCode(IStepRunner runner) =>
        runner.Execute<StoreItem, CreateStoreItem.Request>(item =>
        {
            item.Should().BeValid();
            var request = GivenACreateStoreItemRequest().WithCode(item.Value.Code).Build();

            runner.SetContextData(RequestContext.CurrentRequest, request);

            return request;
        });

    private static void ShouldExpectStatusCodeCreated(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value
                .Should().BeSuccessful()
                .And.HaveStatusCode(HttpStatusCode.Created);
        });
}
