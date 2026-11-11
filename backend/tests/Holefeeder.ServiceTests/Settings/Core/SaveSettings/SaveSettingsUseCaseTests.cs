using Holefeeder.Service.Settings.Core.SaveSettings;
using Holefeeder.Service.Settings.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Settings.TestKit;

namespace Holefeeder.ServiceTests.Settings.Core.SaveSettings;

public class SaveSettingsUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 11, 11);

    private readonly SettingsInMemory _settings = new();
    private readonly SaveSettingsUseCase _useCase;

    public SaveSettingsUseCaseTests() => _useCase = new SaveSettingsUseCase(_settings);

    [Fact]
    public async Task HandleAsync_ReplacesTheUserSettings()
    {
        var user = AUser();
        _settings.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        var settings = PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.Weekly, 2);

        var result = await _useCase.HandleAsync(new SaveSettingsRequest(user, settings), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _settings.GetAsync(user, Today, CancellationToken.None)).Should().BeSuccessful().And.WithValue(settings);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheSettingsCannotBeSaved()
    {
        _settings.FailWith(SettingsErrors.WriteFailed);

        var result = await _useCase.HandleAsync(
            new SaveSettingsRequest(AUser(), PeriodSettings.Default(Today)), CancellationToken.None);

        result.Should().BeFailure().And.WithError(SettingsErrors.WriteFailed);
    }
}
