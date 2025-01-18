using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly Faker _faker = new();

    [Fact]
    public Task GettingAnItemWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsAnItemInTheStore)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingAnItemThatDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(ARequestForAnItemThatDoesNotExist)
            .When(TheUser.GetsAnItemInTheStore)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task GettingAnItemThatExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.Exists)
            .And(AValidRequest)
            .When(TheUser.GetsAnItemInTheStore)
            .Then(TheResultShouldBeAsExpected)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => Guid.Empty);

    private void ARequestForAnItemThatDoesNotExist(IStepRunner runner) =>
        runner.Execute(() => _faker.Random.Guid());

    private void AValidRequest(IStepRunner runner) =>
        runner.Execute<StoreItem, Guid>(item =>
        {
            item.Should().BeValid();
            return item.Value.Id;
        });

    [AssertionMethod]
    private static void TheResultShouldBeAsExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<StoreItemViewModel>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var result = response.Value.Content;

            var item = runner.GetContextData<StoreItem>(StoreItemContext.ExistingStoreItem);
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    Id = (Guid) item.Id,
                    item.Code,
                    item.Data
                });
        });
}
