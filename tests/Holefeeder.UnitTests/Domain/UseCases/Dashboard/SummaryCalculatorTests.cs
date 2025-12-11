using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.UseCases.Dashboard;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.UnitTests.Domain.UseCases.Dashboard;

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
        result.CurrentExpenses.Should().Be(0m);
        result.ExpenseVariation.Should().Be(0m);
        result.NetFlow.Should().Be(0m);
        result.CurrentGains.Should().Be(0m);
        result.AverageExpenses.Should().Be(0m);
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
        result.CurrentExpenses.Should().Be(600m);
        result.ExpenseVariation.Should().Be(166.67m);
        result.NetFlow.Should().Be(600m);
        result.CurrentGains.Should().Be(1200m);
        result.AverageExpenses.Should().Be(433.33m);
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
        result.CurrentExpenses.Should().Be(400m);
        result.ExpenseVariation.Should().Be(0m);
        result.NetFlow.Should().Be(400m);
        result.CurrentGains.Should().Be(800m);
        result.AverageExpenses.Should().Be(400m);
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
        result.CurrentExpenses.Should().Be(550m);
        result.ExpenseVariation.Should().Be(100m);
        result.NetFlow.Should().Be(550m);
        result.CurrentGains.Should().Be(1100m);
        result.AverageExpenses.Should().Be(450m);
    }
}

