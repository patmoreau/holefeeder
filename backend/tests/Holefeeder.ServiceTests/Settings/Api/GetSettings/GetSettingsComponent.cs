using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Settings.Api.GetSettings;

[Collection(ComponentTestMarker.Name)]
public class GetSettingsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheUserHasSettings() =>
        ScenarioRunner.Create("Getting the settings of a user who chose them", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user budgets every two weeks", TheUserBudgetsEveryTwoWeeks)
            .When("the user gets the settings", GettingTheSettings)
            .Then<HttpResponseMessage>("the chosen settings are returned", TheChosenSettingsAreReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserHasNoSettings() =>
        ScenarioRunner.Create("Getting the settings of a user who never chose them", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets the settings", GettingTheSettings)
            .Then<HttpResponseMessage>("monthly settings from today are returned", MonthlySettingsFromTodayAreReturned)
            .PlayAsync();

    [Fact]
    public Task WhenAnotherUserHasSettings() =>
        ScenarioRunner.Create("Getting the settings while another user chose theirs", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user budgets every two weeks", AnotherUserBudgetsEveryTwoWeeks)
            .When("the user gets the settings", GettingTheSettings)
            .Then<HttpResponseMessage>("monthly settings from today are returned", MonthlySettingsFromTodayAreReturned)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private Task TheUserBudgetsEveryTwoWeeks() =>
        new BudgetForTest(_user, server.Database).SettingsAsync("""{"effectiveDate":"2026-09-01","intervalType":"weekly","frequency":2}""");

    private async Task AnotherUserBudgetsEveryTwoWeeks()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(otherUser, server.Database).SettingsAsync(
            """{"effectiveDate":"2026-09-01","intervalType":"weekly","frequency":2}""");
    }

    private async Task<HttpResponseMessage> GettingTheSettings()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/settings", UriKind.Relative));
    }

    private static async Task TheChosenSettingsAreReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var settings = await response.Value.Content.ReadFromJsonAsync<SettingsJson>();
        settings.Should().Be(new SettingsJson(new DateOnly(2026, 9, 1), "weekly", 2));
    }

    private static async Task MonthlySettingsFromTodayAreReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var settings = await response.Value.Content.ReadFromJsonAsync<SettingsJson>();
        settings.Should().Be(new SettingsJson(Today, "monthly", 1));
    }

    private sealed record SettingsJson(DateOnly EffectiveDate, string IntervalType, int Frequency);
}
