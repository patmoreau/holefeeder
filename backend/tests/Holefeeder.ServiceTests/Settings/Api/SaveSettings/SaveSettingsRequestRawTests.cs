using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Settings.Api.SaveSettings;
using Holefeeder.Service.Settings.Core.SaveSettings;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Settings.Api.SaveSettings;

public class SaveSettingsRequestRawTests
{
    private static SaveSettingsBody ABody(string? effectiveDate = "2026-09-01", string? intervalType = "weekly", string? frequency = "2") =>
        new(effectiveDate, intervalType, frequency);

    [Fact]
    public void ToSaveSettingsRequest_ReturnsTheSettingsForTheUser()
    {
        var user = AUser();

        var result = SaveSettingsRequestRaw.ToSaveSettingsRequest(new(ABody()), user);

        result.Should().BeSuccessful().And.WithValue(
            new SaveSettingsRequest(user, PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.Weekly, 2)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("2026-13-01")]
    [InlineData("Sep 1, 2026")]
    public void ToSaveSettingsRequest_Fails_WhenTheEffectiveDateIsInvalid(string? effectiveDate)
    {
        var result = SaveSettingsRequestRaw.ToSaveSettingsRequest(new(ABody(effectiveDate: effectiveDate)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should()
            .BeEquivalentTo(SaveSettingsErrors.CodeEffectiveDateInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("fortnightly")]
    public void ToSaveSettingsRequest_Fails_WhenTheIntervalTypeIsInvalid(string? intervalType)
    {
        var result = SaveSettingsRequestRaw.ToSaveSettingsRequest(new(ABody(intervalType: intervalType)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should()
            .BeEquivalentTo(IntervalTypeErrors.CodeInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1.5")]
    [InlineData("two")]
    public void ToSaveSettingsRequest_Fails_WhenTheFrequencyIsInvalid(string? frequency)
    {
        var result = SaveSettingsRequestRaw.ToSaveSettingsRequest(new(ABody(frequency: frequency)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should()
            .BeEquivalentTo(PeriodSettingsErrors.CodeFrequencyInvalid);
    }

    [Fact]
    public void ToSaveSettingsRequest_Fails_WithEveryInvalidField()
    {
        var result = SaveSettingsRequestRaw.ToSaveSettingsRequest(new(new SaveSettingsBody("tomorrow", "often", "0")), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            SaveSettingsErrors.CodeEffectiveDateInvalid,
            IntervalTypeErrors.CodeInvalid,
            PeriodSettingsErrors.CodeFrequencyInvalid);
    }
}
