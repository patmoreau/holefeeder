using System.Collections.Generic;
using System.Linq;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Transactions;

using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.UnitTests.Domain.Features.Transactions;

[UnitTest]
public class CashflowTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new();
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    public static IEnumerable<object[]> GetUpcomingWithNonePaidTestCases
    {
        get
        {
            yield return new object[]
            {
                "OneTime Cashflow effective after date returns nothing",
                (IntervalType: DateIntervalType.OneTime, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2022, 12, 31), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                "OneTime Cashflow effective before date returns single date",
                (IntervalType: DateIntervalType.OneTime, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2023, 12, 31), new[] {new DateOnly(2023, 1, 1)}
            };
            yield return new object[]
            {
                "Weekly Cashflow effective after date returns nothing",
                (IntervalType: DateIntervalType.Weekly, Frequency: 2, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2022, 12, 31), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                "Weekly Cashflow effective before date returns 27 iterations",
                (IntervalType: DateIntervalType.Weekly, Frequency: 2, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2023, 12, 31),
                new[]
                {
                    new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 15), new DateOnly(2023, 1, 29),
                    new DateOnly(2023, 2, 12), new DateOnly(2023, 2, 26), new DateOnly(2023, 3, 12),
                    new DateOnly(2023, 3, 26), new DateOnly(2023, 4, 9), new DateOnly(2023, 4, 23),
                    new DateOnly(2023, 5, 7), new DateOnly(2023, 5, 21), new DateOnly(2023, 6, 4),
                    new DateOnly(2023, 6, 18), new DateOnly(2023, 7, 2), new DateOnly(2023, 7, 16),
                    new DateOnly(2023, 7, 30), new DateOnly(2023, 8, 13), new DateOnly(2023, 8, 27),
                    new DateOnly(2023, 9, 10), new DateOnly(2023, 9, 24), new DateOnly(2023, 10, 8),
                    new DateOnly(2023, 10, 22), new DateOnly(2023, 11, 5), new DateOnly(2023, 11, 19),
                    new DateOnly(2023, 12, 3), new DateOnly(2023, 12, 17), new DateOnly(2023, 12, 31)
                }
            };
            yield return new object[]
            {
                "Monthly Cashflow effective after date returns nothing",
                (IntervalType: DateIntervalType.Monthly, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2022, 12, 31), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                "Monthly Cashflow effective before date returns 12 iterations",
                (IntervalType: DateIntervalType.Monthly, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2023, 12, 31),
                new[]
                {
                    new DateOnly(2023, 1, 1), new DateOnly(2023, 2, 1), new DateOnly(2023, 3, 1),
                    new DateOnly(2023, 4, 1), new DateOnly(2023, 5, 1), new DateOnly(2023, 6, 1),
                    new DateOnly(2023, 7, 1), new DateOnly(2023, 8, 1), new DateOnly(2023, 9, 1),
                    new DateOnly(2023, 10, 1), new DateOnly(2023, 11, 1), new DateOnly(2023, 12, 1)
                }
            };
            yield return new object[]
            {
                "Yearly Cashflow effective after date returns nothing",
                (IntervalType: DateIntervalType.Yearly, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2022, 12, 31), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                "Yearly Cashflow effective before date returns 2 iterations",
                (IntervalType: DateIntervalType.Yearly, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2024, 12, 31), new[] {new DateOnly(2023, 1, 1), new DateOnly(2024, 1, 1)}
            };
        }
    }

    public static IEnumerable<object[]> GetUpcomingWithPaidTestCases
    {
        get
        {
            yield return new object[]
            {
                "OneTime Cashflow already paid returns nothing",
                (IntervalType: DateIntervalType.OneTime, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2023, 12, 31), new DateOnly(2023, 1, 1), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                "Weekly Cashflow effective before date returns 2 iterations",
                (IntervalType: DateIntervalType.Weekly, Frequency: 2, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2023, 12, 31), new DateOnly(2023, 12, 3),
                new[] {new DateOnly(2023, 12, 17), new DateOnly(2023, 12, 31)}
            };
            yield return new object[]
            {
                "Monthly Cashflow effective before date returns 12 iterations",
                (IntervalType: DateIntervalType.Monthly, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2023, 12, 31), new DateOnly(2023, 7, 1),
                new[]
                {
                    new DateOnly(2023, 8, 1), new DateOnly(2023, 9, 1), new DateOnly(2023, 10, 1),
                    new DateOnly(2023, 11, 1), new DateOnly(2023, 12, 1)
                }
            };
            yield return new object[]
            {
                "Yearly Cashflow effective before date returns 2 iterations",
                (IntervalType: DateIntervalType.Yearly, Frequency: 1, EffectiveDate: new DateOnly(2023, 1, 1)),
                new DateOnly(2024, 12, 31), new DateOnly(2023, 1, 1), new[] {new DateOnly(2024, 1, 1)}
            };
        }
    }

    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().WithId(Guid.Empty);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Id is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenEffectiveDateIsMissing_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().OnEffectiveDate(default);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("EffectiveDate is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenAmountIsNegative_ThenThrowException()
    {
        // arrange
        var builder =
            GivenAnActiveCashflow().OfAmount(_faker.Finance.Amount(decimal.MinValue, decimal.MinusOne));

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Amount cannot be negative")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenFrequencyIsNotPositive_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().OfFrequency(_faker.Random.Int(int.MinValue, 0));

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Frequency must be positive")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenRecurrenceIsNegative_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().Recurring(_faker.Random.Int(int.MinValue, 0));

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Recurrence cannot be negative")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenAccountIdEmpty_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().ForAccount(Guid.Empty);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("AccountId is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenCategoryIdEmpty_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().ForCategory(Guid.Empty);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("CategoryId is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenUserIdEmpty_ThenThrowException()
    {
        // arrange
        var builder = GivenAnActiveCashflow().ForUser(Guid.Empty);

        // act
        Action action = () => _ = builder.Build();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenCancel_WhenCashflowInactive_ThenThrowException()
    {
        // arrange
        var cashflow = GivenAnInactiveCashflow().Build();

        // act
        Action action = () => _ = cashflow.Cancel();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage($"Cashflow {cashflow.Id} already inactive")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenCancel_WhenCashflowActive_ThenCashflowIsInactive()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow().Build();

        // act
        var result = cashflow.Cancel();

        // assert
        using var scope = new AssertionScope();
        result.Should().NotBeNull();
        result.Inactive.Should().BeTrue();
    }

    [Fact]
    public void GivenSetTags_WhenEmptyList_ThenTagListIsEmpty()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow().Build();

        // act
        var result = cashflow.SetTags(Array.Empty<string>());

        // assert
        result.Tags.Should().BeEmpty();
    }

    [Fact]
    public void GivenSetTags_WhenAddingTags_ThenTagListIsSet()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow().Build();
        string[] newTags = _faker.Lorem.Words(_faker.Random.Int(1, 10)).Distinct().ToArray();

        // act
        var result = cashflow.SetTags(newTags);

        // assert
        result.Tags.Should().Contain(newTags);
    }

    [Fact]
    public void GivenLastPaidDate_WhenNoTransactions_ThenReturnNull()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow().Build();

        // act
        var result = cashflow.LastPaidDate;

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public void GivenLastPaidDate_WhenTransactions_ThenReturnMaxDate()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow()
            .WithTransactions()
            .Build();

        // act
        var result = cashflow.LastPaidDate;

        // assert
        result.Should().Be(cashflow.Transactions.OrderByDescending(x => x.Date).First().Date);
    }

    [Fact]
    public void GivenLastCashflowDate_WhenNoTransactions_ThenReturnNull()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow().Build();

        // act
        var result = cashflow.LastCashflowDate;

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public void GivenLastCashflowDate_WhenTransactions_ThenReturnMaxCashflowDate()
    {
        // arrange
        var cashflow = GivenAnActiveCashflow()
            .WithTransactions()
            .Build();

        // act
        var result = cashflow.LastCashflowDate;

        // assert
        result.Should().Be(cashflow.Transactions.OrderByDescending(x => x.Date).First().CashflowDate);
    }

    [Fact]
    public void GivenGetUpcoming_WhenInactiveCashflow_ThenReturnEmptyList()
    {
        // arrange
        var cashflow = GivenAnInactiveCashflow().Build();

        // act
        var result = cashflow.GetUpcoming(_faker.Date.FutureDateOnly());

        // assert
        result.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(GetUpcomingWithNonePaidTestCases))]
    public void GivenGetUpcoming_WhenNoCashflowsPaid_ThenReturnExpectedDates(
        string testName,
        (DateIntervalType IntervalType, int Frequency, DateOnly EffectiveDate) info,
        DateOnly to,
        IReadOnlyCollection<DateOnly> expected)
    {
        _testOutputHelper.WriteLine(testName);

        ArgumentNullException.ThrowIfNull(expected);

        // arrange
        var cashflow = GivenAnActiveCashflow()
            .OfFrequency(info.IntervalType, info.Frequency)
            .OnEffectiveDate(info.EffectiveDate)
            .Build();

        // act
        var result = cashflow.GetUpcoming(to);

        // assert
        result.Should().HaveCount(expected.Count).And.Equal(expected);
    }

    [Theory]
    [MemberData(nameof(GetUpcomingWithPaidTestCases))]
    public void GivenGetUpcoming_WhenSomeCashflowsPaid_ThenReturnExpectedDates(
        string testName,
        (DateIntervalType IntervalType, int Frequency, DateOnly EffectiveDate) info,
        DateOnly to,
        DateOnly lastPaidDate,
        IReadOnlyCollection<DateOnly> expected)
    {
        _testOutputHelper.WriteLine(testName);

        ArgumentNullException.ThrowIfNull(expected);

        // arrange
        var transaction = GivenATransaction()
            .OnDate(lastPaidDate.AddDays(_faker.Random.Int(0, 10)))
            .ForCashflowDate(lastPaidDate)
            .Build();
        var cashflow = GivenAnActiveCashflow()
            .OfFrequency(info.IntervalType, info.Frequency)
            .OnEffectiveDate(info.EffectiveDate)
            .WithTransactions(transaction)
            .Build();

        // act
        var result = cashflow.GetUpcoming(to);

        // assert
        result.Should().HaveCount(expected.Count).And.Equal(expected);
    }
}
