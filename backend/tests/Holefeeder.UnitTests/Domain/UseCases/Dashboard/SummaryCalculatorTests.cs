using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.UseCases.Dashboard;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.UnitTests.Domain.UseCases.Dashboard;

[UnitTest]
public class SummaryCalculatorTests
{
    [Fact]
    public void Calculate_WithEmptyData_ReturnsZeroValues()
    {
        // Arrange
        var data = Array.Empty<SummaryData>();
        var dateInterval = new DateInterval(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));
        var intervalType = DateIntervalType.Monthly;
        const int frequency = 1;

        // Act
        var result = SummaryCalculator.Calculate(data, dateInterval, intervalType, frequency);

        // Assert
        result.Should().Be(new SummaryResult(
            0m,
            0m,
            0m,
            0m,
            0m,
            0m
        ));
    }

    [Fact]
    public void Calculate_WithGainsAndExpenses_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2025, 2, 1);
        var data = new List<SummaryData>
        {
            // Current period (Feb 2025)
            new(CategoryType.Gain, new DateOnly(2025, 2, 15), 1200m),
            new(CategoryType.Expense, new DateOnly(2025, 2, 20), 600m),

            // Last period (Jan 2025)
            new(CategoryType.Gain, new DateOnly(2025, 1, 15), 800m),
            new(CategoryType.Expense, new DateOnly(2025, 1, 20), 400m),

            // Dec 2024 for average
            new(CategoryType.Gain, new DateOnly(2024, 12, 15), 900m),
            new(CategoryType.Expense, new DateOnly(2024, 12, 20), 300m)
        };
        var dateInterval = new DateInterval(startDate, new DateOnly(2025, 2, 28));
        var intervalType = DateIntervalType.Monthly;
        const int frequency = 1;

        // Act
        var result = SummaryCalculator.Calculate(data, dateInterval, intervalType, frequency);

        // Assert
        result.Should().Be(new SummaryResult(
            600m,
            166.67m,
            38.46m,
            600m,
            1200m,
            433.33m
        ));
    }

    [Fact]
    public void Calculate_WithMultipleTransactionsSameDay_AggregatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2025, 1, 1);
        var data = new List<SummaryData>
        {
            new(CategoryType.Gain, new DateOnly(2025, 1, 15), 350m),
            new(CategoryType.Gain, new DateOnly(2025, 1, 15), 450m),
            new(CategoryType.Expense, new DateOnly(2025, 1, 15), 150m),
            new(CategoryType.Expense, new DateOnly(2025, 1, 15), 250m)
        };
        var dateInterval = new DateInterval(startDate, new DateOnly(2025, 1, 31));
        var intervalType = DateIntervalType.Monthly;
        const int frequency = 1;

        // Act
        var result = SummaryCalculator.Calculate(data, dateInterval, intervalType, frequency);

        // Assert
        result.Should().Be(new SummaryResult(
            400m,
            0m,
            0m,
            400m,
            800m,
            400m
        ));
    }

    [Fact]
    public void Calculate_WithWeeklyInterval_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2025, 1, 6); // Monday
        var data = new List<SummaryData>
        {
            new(CategoryType.Gain, new DateOnly(2025, 1, 8), 1100m), // Current week
            new(CategoryType.Expense, new DateOnly(2025, 1, 9), 550m),
            new(CategoryType.Gain, new DateOnly(2024, 12, 30), 750m), // Previous week
            new(CategoryType.Expense, new DateOnly(2024, 12, 31), 350m)
        };
        var dateInterval = new DateInterval(startDate, new DateOnly(2025, 1, 12));
        var intervalType = DateIntervalType.Weekly;
        const int frequency = 1;

        // Act
        var result = SummaryCalculator.Calculate(data, dateInterval, intervalType, frequency);

        // Assert
        result.Should().Be(new SummaryResult(
            550m,
            100m,
            22.22m,
            550m,
            1100m,
            450m
        ));
    }
}

