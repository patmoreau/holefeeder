using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Settings.Api.SaveSettings;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.ServiceTests.Settings.Api.SaveSettings;

[Collection(ComponentTestMarker.Name)]
public class SaveSettingsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheSettingsAreSaved() =>
        ScenarioRunner.Create("Saving the settings for the first time", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user budgets every two weeks", () => SavingTheSettings(new { EffectiveDate = "2026-09-01", IntervalType = "weekly", Frequency = 2 }))
            .Then<HttpResponseMessage>("the settings are every two weeks", TheSettingsAreEveryTwoWeeks)
            .PlayAsync();

    [Fact]
    public Task WhenTheSettingsAreChanged() =>
        ScenarioRunner.Create("Changing the settings", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user budgets monthly", TheUserBudgetsMonthly)
            .When("the user budgets every two weeks", () => SavingTheSettings(new { EffectiveDate = "2026-09-01", IntervalType = "weekly", Frequency = "2" }))
            .Then<HttpResponseMessage>("the settings are every two weeks", TheSettingsAreEveryTwoWeeks)
            .PlayAsync();

    [Fact]
    public Task WhenTheSettingsAreInvalid() =>
        ScenarioRunner.Create("Saving invalid settings", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user saves settings with every field invalid", () => SavingTheSettings(new { EffectiveDate = "soon", IntervalType = "often", Frequency = 0 }))
            .Then<HttpResponseMessage>("a validation error is returned on every field", AValidationErrorIsReturnedOnEveryField)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private Task TheUserBudgetsMonthly() =>
        new BudgetForTest(_user, server.Database).SettingsAsync("""{"effectiveDate":"2025-01-01","intervalType":"monthly","frequency":1}""");

    private async Task<HttpResponseMessage> SavingTheSettings(object body)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/settings/save-settings", UriKind.Relative), body);
    }

    private async Task TheSettingsAreEveryTwoWeeks(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var settings = await client.GetFromJsonAsync<SettingsJson>(new Uri("/api/v2/settings", UriKind.Relative));
        settings.Should().Be(new SettingsJson(new DateOnly(2026, 9, 1), "weekly", 2));
    }

    private static async Task AValidationErrorIsReturnedOnEveryField(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Keys.Should().BeEquivalentTo(
            SaveSettingsErrors.CodeEffectiveDateInvalid, IntervalTypeErrors.CodeInvalid, PeriodSettingsErrors.CodeFrequencyInvalid);
    }

    private sealed record SettingsJson(DateOnly EffectiveDate, string IntervalType, int Frequency);
}
