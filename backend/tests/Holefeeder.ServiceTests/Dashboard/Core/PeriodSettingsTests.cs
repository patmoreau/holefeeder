using Holefeeder.Service.Dashboard.Core;

namespace Holefeeder.ServiceTests.Dashboard.Core;

public class PeriodSettingsTests
{
    [Fact]
    public void Create_Succeeds_WhenFrequencyIsPositive()
    {
        var result = PeriodSettings.Create(new DateOnly(2026, 1, 1), IntervalType.Weekly, 2);

        result.Should().BeSuccessful()
            .And.WithValue(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Weekly, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Fails_WhenFrequencyIsNotPositive(int frequency)
    {
        var result = PeriodSettings.Create(new DateOnly(2026, 1, 1), IntervalType.Monthly, frequency);

        result.Should().BeFailure().And.WithError(PeriodSettingsErrors.FrequencyInvalid);
    }

    [Fact]
    public void Default_IsMonthlyFromToday() =>
        PeriodSettings.Default(new DateOnly(2026, 11, 10))
            .Should().Be(PeriodSettings.Valid(new DateOnly(2026, 11, 10), IntervalType.Monthly, 1));
}
