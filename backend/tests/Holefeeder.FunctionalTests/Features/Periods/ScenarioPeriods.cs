using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Periods;

public class ScenarioPeriods(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task ComputeTheirPeriod() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(UserSettingsExists)
            .When(TheUserComputeTheirPeriod)
            .Then(ShouldReceiveExpectedDateInterval)
            .PlayAsync();

    private void UserSettingsExists(IStepRunner runner) =>
        StoreItem.HasUserSettings(runner, new UserSettings
            {
                EffectiveDate = new DateOnly(2023, 1, 1),
                IntervalType = DateIntervalType.Monthly,
                Frequency = 1
            });

    private void TheUserComputeTheirPeriod(IStepRunner runner) =>
        TheUser.ComputesTheirPeriod(runner, new DateOnly(2025, 2, 5), 1);

    [AssertionMethod]
    private static void ShouldReceiveExpectedDateInterval(IStepRunner runner) =>
        runner.Execute<IApiResponse<DateInterval>>(response =>
        {
            var storeItem = runner.GetContextData<StoreItem>(StoreItemContext.ExistingStoreItem);

            response.Should().BeValid();
            response.Value.Should().BeSuccessful()
                .And.HaveContent();
            response.Value.Content.Should().Be(new DateInterval(new DateOnly(2025, 3, 1), new DateOnly(2025, 3, 31)));
        });

}
