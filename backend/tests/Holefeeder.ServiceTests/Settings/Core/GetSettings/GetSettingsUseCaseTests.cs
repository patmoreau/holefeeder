using Holefeeder.Service.Settings.Core.GetSettings;
using Holefeeder.Service.Settings.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Settings.TestKit;

namespace Holefeeder.ServiceTests.Settings.Core.GetSettings;

public class GetSettingsUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 11, 11);

    private readonly SettingsInMemory _settings = new();
    private readonly GetSettingsUseCase _useCase;

    public GetSettingsUseCaseTests() => _useCase = new GetSettingsUseCase(_settings);

    [Fact]
    public async Task HandleAsync_ReturnsTheUserSettings()
    {
        var user = AUser();
        var settings = PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.Weekly, 2);
        _settings.Add(user, settings);

        var result = await _useCase.HandleAsync(new GetSettingsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(settings);
    }

    [Fact]
    public async Task HandleAsync_ReturnsMonthlyFromToday_WhenTheUserHasNoSettings()
    {
        var result = await _useCase.HandleAsync(new GetSettingsRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(PeriodSettings.Default(Today));
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheSettingsCannotBeRead()
    {
        _settings.FailWith(SettingsErrors.ReadFailed);

        var result = await _useCase.HandleAsync(new GetSettingsRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(SettingsErrors.ReadFailed);
    }
}
