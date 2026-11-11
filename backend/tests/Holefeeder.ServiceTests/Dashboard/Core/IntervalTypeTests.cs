using Holefeeder.Service.Dashboard.Core;

namespace Holefeeder.ServiceTests.Dashboard.Core;

public class IntervalTypeTests
{
    public static TheoryData<string, string> Names => new()
    {
        { "daily", "Daily" },
        { "Weekly", "Weekly" },
        { "monthly", "Monthly" },
        { "YEARLY", "Yearly" },
        { "onetime", "OneTime" },
        { "OneTime", "OneTime" }
    };

    [Theory]
    [MemberData(nameof(Names))]
    public void Create_Succeeds_IgnoringCase(string name, string expected)
    {
        var result = IntervalType.Create(name);

        result.Should().BeSuccessful();
        result.Value.Name.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("fortnightly")]
    public void Create_Fails_WhenNameIsUnknown(string? name)
    {
        var result = IntervalType.Create(name);

        result.Should().BeFailure().And.WithError(IntervalTypeErrors.Invalid);
    }

    public static TheoryData<string, int, DateOnly> Iterations => new()
    {
        { "Daily", 3, new DateOnly(2026, 1, 18) },
        { "Weekly", 2, new DateOnly(2026, 1, 29) },
        { "Monthly", 1, new DateOnly(2026, 2, 15) },
        { "Monthly", -1, new DateOnly(2025, 12, 15) },
        { "Yearly", 1, new DateOnly(2027, 1, 15) },
        { "OneTime", 5, new DateOnly(2026, 1, 15) }
    };

    [Theory]
    [MemberData(nameof(Iterations))]
    public void AddIteration_MovesByTheIntervalLength(string intervalType, int iteration, DateOnly expected) =>
        IntervalType.Create(intervalType).Value.AddIteration(new DateOnly(2026, 1, 15), iteration).Should().Be(expected);
}
